using FURPMS.Application.Common;
using FURPMS.Application.Constants;
using FURPMS.Application.DTOs.Progress;
using FURPMS.Application.Interfaces;
using FURPMS.Application.Interfaces.Repositories;
using FURPMS.Application.Interfaces.Services;
using FURPMS.Domain.Entities.Progress;
using Microsoft.EntityFrameworkCore;

namespace FURPMS.Infrastructure.Services;

public class ProgressReportService : IProgressReportService
{
    private readonly IContractRepository _contracts;
    private readonly IProposalRepository _proposals;
    // Dùng IClock (không phải DateTime.UtcNow) để test tua được thời gian — các service khác
    // trong dự án đã theo quy ước này.
    private readonly IClock _clock;
    private readonly IDocumentRepository _documents;
    private readonly INotifier _notifier;
    private readonly IDecisionLogger _decisions;

    public ProgressReportService(IContractRepository contracts, IProposalRepository proposals, IClock clock,
        IDocumentRepository documents,
        INotifier notifier,
        IDecisionLogger decisions)
    {
        _decisions = decisions;
        _notifier = notifier;
        _contracts = contracts;
        _proposals = proposals;
        _clock = clock;
        _documents = documents;
    }

    /// <summary>Kết quả đánh giá tiến độ ra tiếng Việt — dùng chung cho thông báo và sổ quyết định.</summary>
    private static string ProgressResultVi(string? result) => result switch
    {
        "PASS" => "Đạt",
        "FAIL" => "Không đạt",
        "CONDITIONAL" => "Đạt có điều kiện",   // chỉ còn ở dữ liệu cũ (trước 03/10)
        _ => result ?? ""
    };

    // QĐ543 Điều 10.1: Ứng dụng 2 kỳ, Cơ bản 1 kỳ — theo MÃ loại cố định (03/10).
    private static int RequiredRounds(FURPMS.Domain.Entities.MasterData.ResearchType? type) =>
        ResearchKind.ProgressRounds(ResearchKind.IsApplied(type?.Code));

    private static DateOnly Min(DateOnly a, DateOnly b) => a < b ? a : b;

    // Staff sinh sẵn các kỳ báo cáo — chia đều theo mốc hợp đồng; bỏ qua kỳ đã có.
    // roundCount: Staff tự chọn số kỳ; bỏ trống → mặc định theo loại (Ứng dụng 2 / Cơ bản 1).
    public async Task<IEnumerable<ProgressReportSummaryDto>> GenerateScheduledRoundsAsync(
        Guid contractId, int? roundCount = null, Guid? staffId = null)
    {
        var contract = await _contracts.Query()
            .Include(c => c.Project).ThenInclude(p => p.ResearchType)
            .FirstOrDefaultAsync(c => c.Id == contractId)
            ?? throw new KeyNotFoundException("Không tìm thấy hợp đồng.");

        if (roundCount is < 1 or > 12)
            throw new ArgumentException("Số kỳ báo cáo phải từ 1 đến 12.");

        var required = roundCount ?? RequiredRounds(contract.Project.ResearchType);
        var existing = await _contracts.ProgressReports
            .Where(r => r.ContractId == contractId)
            .Select(r => r.ReportRound)
            .ToListAsync();

        var start = contract.StartDate;
        var end = contract.EndDate > start ? contract.EndDate : start.AddMonths(6);
        var totalDays = end.DayNumber - start.DayNumber;
        var chunk = required > 0 ? totalDays / required : totalDays;

        var added = 0;
        for (int round = 1; round <= required; round++)
        {
            if (existing.Contains(round)) continue;
            added++;
            var pStart = start.AddDays(chunk * (round - 1));
            var pEnd = round == required ? end : start.AddDays(chunk * round);
            await _contracts.AddProgressReportAsync(new ProgressReport
            {
                ContractId = contractId,
                ReportRound = round,
                ReportingPeriodStart = pStart,
                ReportingPeriodEnd = pEnd,
                DueDate = pEnd,
                // Đặt sẵn buổi họp Hội đồng đánh giá tiến độ (QĐ543 Điều 10.1) — 5 ngày làm việc sau hạn
                // nộp, 9h sáng giờ Việt Nam — để kỳ vừa tạo đã có lịch; Staff đổi ở "Đặt lịch họp" (03/10).
                // 04/10: kỳ CUỐI có hạn nộp = ngày kết thúc hợp đồng ⇒ +5 ngày làm việc vượt hợp đồng, mà luật
                // "Đặt lịch họp" bắt buổi họp nằm trong thời gian hợp đồng ⇒ mở ra lưu là lỗi. Kéo về ngày kết thúc.
                ScheduledMeetingAt = Min(DeadlineMath.AddWorkingDays(pEnd, 5), contract.EndDate)
                    .ToDateTime(new TimeOnly(2, 0), DateTimeKind.Utc),
                CompletedContent = "",   // NOT NULL — PI điền sau; để rỗng cho kỳ vừa mở
                Status = ProgressReportStatus.Draft
            });
        }
        // Lịch tiến độ là quyết định của Phòng QLKH (mặc định theo loại, chỉnh được) ⇒ ghi sổ (03/10).
        if (added > 0 && staffId.HasValue)
            _decisions.Log(
                contract.ProjectId, DecisionTypes.ResultReopened,
                roundCount.HasValue
                    ? $"Lập lịch báo cáo tiến độ tuỳ chỉnh: {required} kỳ (thêm {added} kỳ)"
                    : $"Lập lịch báo cáo tiến độ theo mẫu {(ResearchKind.IsApplied(contract.Project.ResearchType?.Code) ? "ứng dụng" : "cơ bản")}: {required} kỳ (thêm {added} kỳ)",
                "Contract", contract.Id.ToString(),
                result: "SCHEDULED", decidedBy: staffId.Value, decidedByRole: "Phòng QLKH");
        await _contracts.SaveChangesAsync();
        return await GetByContractAsync(contractId);
    }

    /// <summary>
    /// Xoá hết các kỳ còn NHÁP rồi lập lại theo mẫu của loại đề tài (03/10) — khi lịch đã lập sai (loại đề tài
    /// từng cấu hình sai trên deploy, hay chỉnh tay rồi muốn về mặc định). Có kỳ đã nộp / đã có kết luận thì chặn:
    /// xoá từng kỳ (kèm lý do) hoặc "Mở lại" trước. Ghi sổ quyết định.
    /// </summary>
    public async Task<IEnumerable<ProgressReportSummaryDto>> ResetToTemplateAsync(Guid contractId, string reason, Guid staffId)
    {
        if (string.IsNullOrWhiteSpace(reason))
            throw new ArgumentException("Lập lại lịch báo cáo tiến độ phải ghi lý do.");
        var contract = await _contracts.Query()
            .Include(c => c.Project).ThenInclude(p => p.ResearchType)
            .FirstOrDefaultAsync(c => c.Id == contractId)
            ?? throw new KeyNotFoundException("Không tìm thấy hợp đồng.");

        var reports = await _contracts.ProgressReports.Where(r => r.ContractId == contractId).ToListAsync();
        if (reports.Any(r => r.Status != ProgressReportStatus.Draft || !string.IsNullOrWhiteSpace(r.EvaluationResult)))
            throw new InvalidOperationException(
                "Đã có kỳ báo cáo được nộp hoặc có kết luận — không xoá cả lịch được. Xoá từng kỳ (ghi lý do) hoặc " +
                "\"Mở lại\" kết luận trước.");

        var ids = reports.Select(r => r.Id).ToList();
        var items = await _contracts.ProgressReportItems.Where(i => ids.Contains(i.ReportId)).ToListAsync();
        if (items.Count > 0) _contracts.RemoveProgressReportItemsRange(items);
        _contracts.RemoveProgressReportsRange(reports);
        await _contracts.SaveChangesAsync();

        var result = (await GenerateScheduledRoundsAsync(contractId)).ToList();
        _decisions.Log(
            contract.ProjectId, DecisionTypes.ResultReopened,
            $"Lập lại lịch báo cáo tiến độ theo mẫu {(ResearchKind.IsApplied(contract.Project.ResearchType?.Code) ? "ứng dụng" : "cơ bản")}: {reports.Count} kỳ → {result.Count} kỳ",
            "Contract", contract.Id.ToString(),
            result: "REGENERATED", reason: reason.Trim(), decidedBy: staffId, decidedByRole: "Phòng QLKH");
        await _contracts.SaveChangesAsync();
        return result;
    }

    public async Task<IEnumerable<ProgressReportSummaryDto>> GetByContractAsync(Guid contractId)
    {
        _ = await _contracts.Query().FirstOrDefaultAsync(c => c.Id == contractId)
            ?? throw new KeyNotFoundException("Không tìm thấy hợp đồng.");

        var reports = await _contracts.ProgressReports
            .Where(r => r.ContractId == contractId)
            .OrderBy(r => r.ReportRound)
            .ToListAsync();

        return reports.Select(MapSummary);
    }

    public async Task<ProgressReportDto> GetByIdAsync(Guid reportId)
    {
        var report = await _contracts.ProgressReports
            .Include(r => r.Items).ThenInclude(i => i.Activity)
            .FirstOrDefaultAsync(r => r.Id == reportId)
            ?? throw new KeyNotFoundException("Không tìm thấy báo cáo tiến độ.");

        return MapDetail(report);
    }

    public async Task<ProgressReportDto> CreateAsync(Guid contractId, CreateProgressReportRequest request, Guid userId)
    {
        var contract = await _contracts.Query()
            .Include(c => c.Project).ThenInclude(p => p.Proposals.Where(x => x.IsCurrent))
            .Include(c => c.Project).ThenInclude(p => p.ResearchType)
            .FirstOrDefaultAsync(c => c.Id == contractId)
            ?? throw new KeyNotFoundException("Không tìm thấy hợp đồng.");

        if (contract.Project.PiUserId != userId)
            throw new ForbiddenException("Chỉ chủ nhiệm đề tài của hợp đồng này mới tạo được báo cáo tiến độ.");

        // QĐ543 Điều 10.1 gợi ý Ứng dụng 2 / Cơ bản 1 kỳ, NHƯNG không chặn cứng (thầy 29/07:
        // Staff phải chỉnh được số lần) — chỉ dùng làm mặc định khi sinh kỳ tự động.

        if (!DateOnly.TryParse(request.ReportingPeriodStart, out var periodStart))
            throw new ArgumentException("Ngày bắt đầu kỳ báo cáo không hợp lệ (định dạng yyyy-MM-dd).");
        if (!DateOnly.TryParse(request.ReportingPeriodEnd, out var periodEnd))
            throw new ArgumentException("Ngày kết thúc kỳ báo cáo không hợp lệ (định dạng yyyy-MM-dd).");
        if (periodEnd <= periodStart)
            throw new ArgumentException("Ngày kết thúc kỳ báo cáo phải sau ngày bắt đầu.");
        if (periodStart < contract.StartDate || periodEnd > contract.EndDate)
            throw new ArgumentException(
                $"Kỳ báo cáo phải nằm trong thời gian hợp đồng từ {contract.StartDate:dd/MM/yyyy} đến {contract.EndDate:dd/MM/yyyy}.");
        if (request.OverallCompletionPct is < 0 or > 100)
            throw new ArgumentException("Tỷ lệ hoàn thành phải nằm trong khoảng 0–100%.");

        var nextRound = await _contracts.ProgressReports
            .Where(r => r.ContractId == contractId)
            .CountAsync() + 1;

        var report = new ProgressReport
        {
            ContractId = contractId,
            ReportRound = nextRound,
            ReportingPeriodStart = periodStart,
            ReportingPeriodEnd = periodEnd,
            CompletedContent = request.CompletedContent ?? "",   // NOT NULL
            PendingContent = request.PendingContent,
            OverallCompletionPct = request.OverallCompletionPct,
            ExpenditureToDate = request.ExpenditureToDate,
            NextPeriodPlan = request.NextPeriodPlan,
            PiRecommendations = request.PiRecommendations,
            Status = ProgressReportStatus.Draft
        };

        await _contracts.AddProgressReportAsync(report);
        await _contracts.SaveChangesAsync();

        if (request.Items.Count > 0)
        {
            var activityIds = request.Items.Select(i => i.ActivityId).ToHashSet();
            var activities = await _proposals.Activities
                .Where(a => a.Proposal.ProjectId == contract.ProjectId && activityIds.Contains(a.Id))
                .ToDictionaryAsync(a => a.Id);

            _contracts.AddProgressReportItemsRange(request.Items.Select(i =>
            {
                if (!activities.ContainsKey(i.ActivityId))
                    throw new KeyNotFoundException("Không tìm thấy hoạt động trong đề cương này.");
                return new ProgressReportItem
                {
                    ReportId = report.Id,
                    ActivityId = i.ActivityId,
                    CompletionRate = i.CompletionRate,
                    CompletionStatus = i.CompletionStatus,
                    EvidenceDescription = i.EvidenceDescription,
                    Notes = i.Notes
                };
            }));
            await _contracts.SaveChangesAsync();
        }

        return await GetByIdAsync(report.Id);
    }

    public async Task<ProgressReportDto> UpdateAsync(Guid reportId, UpdateProgressReportRequest request, Guid userId)
    {
        var report = await _contracts.ProgressReports
            .Include(r => r.Contract).ThenInclude(c => c.Project)
            .FirstOrDefaultAsync(r => r.Id == reportId)
            ?? throw new KeyNotFoundException("Không tìm thấy báo cáo tiến độ.");

        if (report.Contract.Project.PiUserId != userId)
            throw new ForbiddenException("Chỉ chủ nhiệm đề tài mới sửa được báo cáo này.");

        // Cho sửa đến khi Staff ĐÃ ĐÁNH GIÁ, không phải khoá ngay lúc nộp.
        // Khoá ngay lúc nộp là bất nhất với sản phẩm (cho nộp lại tới khi ĐẠT) và báo cáo
        // tổng kết (cho nộp lại + yêu cầu chỉnh sửa) — PI lỡ sai một chữ là kẹt luôn.
        if (!string.IsNullOrWhiteSpace(report.EvaluationResult))
            throw new InvalidOperationException(
                "Kỳ báo cáo này đã được đánh giá — không sửa được nữa. Liên hệ phòng QLKH nếu cần điều chỉnh.");

        if (request.OverallCompletionPct is < 0 or > 100)
            throw new ArgumentException("Tỷ lệ hoàn thành phải nằm trong khoảng 0–100%.");

        report.CompletedContent = request.CompletedContent ?? "";   // NOT NULL
        report.PendingContent = request.PendingContent;
        report.OverallCompletionPct = request.OverallCompletionPct;
        report.ExpenditureToDate = request.ExpenditureToDate;
        report.NextPeriodPlan = request.NextPeriodPlan;
        report.PiRecommendations = request.PiRecommendations;
        // Bỏ trống thì GIỮ link cũ — nộp lại mà không dán lại link không được mất bản cũ.
        if (!string.IsNullOrWhiteSpace(request.ReportFileUrl))
        {
            // Dán "drive.google.com/..." thiếu "https://" thì tự thêm — giống sản phẩm và báo cáo tổng
            // kết. Trước 03/10 chỗ này bắt gõ đủ scheme nên chỉ riêng báo cáo tiến độ báo lỗi.
            var reportUrl = request.ReportFileUrl.Trim();
            if (!reportUrl.StartsWith("http://", StringComparison.OrdinalIgnoreCase)
                && !reportUrl.StartsWith("https://", StringComparison.OrdinalIgnoreCase))
                reportUrl = "https://" + reportUrl;
            if (!Uri.TryCreate(reportUrl, UriKind.Absolute, out var uri) || !uri.Host.Contains('.'))
                throw new ArgumentException($"Link báo cáo không hợp lệ: \"{request.ReportFileUrl.Trim()}\".");
            report.ReportFileUrl = uri.ToString();
        }

        // Bảng tiến độ theo hoạt động (BM06): gửi lên thì THAY toàn bộ bảng cũ.
        if (request.Items != null)
        {
            var old = await _contracts.ProgressReportItems.Where(i => i.ReportId == report.Id).ToListAsync();
            _contracts.RemoveProgressReportItemsRange(old);
            _contracts.AddProgressReportItemsRange(request.Items.Select(i => new ProgressReportItem
            {
                ReportId = report.Id,
                ActivityId = i.ActivityId,
                CompletionRate = i.CompletionRate,
                CompletionStatus = string.IsNullOrWhiteSpace(i.CompletionStatus) ? "IN_PROGRESS" : i.CompletionStatus,
                EvidenceDescription = i.EvidenceDescription,
                Notes = i.Notes
            }));
        }

        await _contracts.SaveChangesAsync();
        return await GetByIdAsync(report.Id);
    }

    public async Task<ProgressReportDto> SubmitAsync(Guid reportId, Guid userId)
    {
        var report = await _contracts.ProgressReports
            .Include(r => r.Contract).ThenInclude(c => c.Project)
            .FirstOrDefaultAsync(r => r.Id == reportId)
            ?? throw new KeyNotFoundException("Không tìm thấy báo cáo tiến độ.");

        if (report.Contract.Project.PiUserId != userId)
            throw new ForbiddenException("Chỉ chủ nhiệm đề tài mới nộp được báo cáo này.");

        // PI được cập nhật/nộp lại cho tới khi Staff đánh giá. UpdateAsync đã cho phép sửa bản
        // SUBMITTED chưa có kết quả, vì vậy SubmitAsync cũng phải chấp nhận đúng trạng thái đó.
        // Trước đây FE cập nhật thành công rồi gọi submit lần nữa và bị chính kiểm tra DRAFT này chặn.
        if (report.Status is not ProgressReportStatus.Draft and not ProgressReportStatus.Submitted)
            throw new InvalidOperationException(
                $"Báo cáo đang ở trạng thái {StatusText.Vi(report.Status)} — chỉ nộp hoặc nộp lại được trước khi có kết quả đánh giá.");

        /*
         * QĐ543 Điều 10.1: báo cáo tiến độ là báo cáo **ĐỊNH KỲ** — kỳ sau chỉ có nghĩa khi kỳ
         * trước đã chốt. Thầy 05/08: *"nộp báo cáo phải được duyệt xong phải qua 1 thời gian mới
         * cho nộp lần 2, hiện tại đang có thể nộp 2 lần cùng lúc"*.
         *
         * Trước đây `SubmitAsync` chỉ kiểm DRAFT ⇒ PI nộp kỳ 1 xong **nộp dồn luôn kỳ 2, kỳ 3**
         * dù kỳ trước Staff chưa đánh giá và kỳ sau còn chưa tới ngày bắt đầu.
         *
         * Chặn theo KỲ chứ không theo số ngày — QĐ543 không quy định khoảng cách ngày nào cả.
         */
        var earlier = await _contracts.ProgressReports
            .Where(r => r.ContractId == report.ContractId && r.ReportRound < report.ReportRound)
            .OrderByDescending(r => r.ReportRound)
            .FirstOrDefaultAsync();

        if (earlier != null && string.IsNullOrWhiteSpace(earlier.EvaluationResult))
        {
            var name = earlier.RoundName ?? $"Kỳ {earlier.ReportRound}";
            throw new InvalidOperationException(
                earlier.SubmittedAt == null
                    ? $"Chưa nộp {name} — phải nộp và được đánh giá xong kỳ trước rồi mới nộp kỳ này."
                    : $"{name} đã nộp nhưng phòng QLKH chưa đánh giá — chờ có kết quả kỳ trước rồi mới nộp kỳ này.");
        }

        /*
         * KHÔNG chặn "chưa tới ngày bắt đầu kỳ".
         *
         * QĐ543 Điều 10.1 chỉ định *khi nào Trường tổ chức đánh giá tiến độ* (cuối giai đoạn 1 và 2
         * với đề tài ứng dụng, giữa kỳ với đề tài cơ bản) — **không có câu nào cấm chủ nhiệm nộp
         * sớm**. Chủ nhiệm làm xong trước hạn mà bị chặn lại thì chốt chặn đó chỉ gây phiền, không
         * bảo vệ điều gì.
         *
         * Cái thật sự cần giữ là **thứ tự kỳ** (đã chặn ở trên): kỳ trước phải có kết quả rồi mới
         * tới kỳ sau — đó mới là ý "định kỳ" của Điều 10.1.
         */

        report.Status = ProgressReportStatus.Submitted;
        report.SubmittedAt = _clock.UtcNow;
        report.UpdatedAt = _clock.UtcNow;
        await _contracts.SaveChangesAsync();

        return await GetByIdAsync(reportId);
    }

    public async Task<bool> IsPiOfReportAsync(Guid reportId, Guid userId)
    {
        return await _contracts.ProgressReports
            .Include(r => r.Contract).ThenInclude(c => c.Project)
            .AnyAsync(r => r.Id == reportId && r.Contract.Project.PiUserId == userId);
    }

    /// <summary>
    /// Xoá một kỳ báo cáo lỡ tạo nhầm. Đã nộp hoặc đã đánh giá thì KHÔNG xoá — báo cáo tiến độ là
    /// căn cứ mở đợt giải ngân và là một mục trong hồ sơ nghiệm thu; xoá đi thì hội đồng thấy đề
    /// tài "nhảy cóc" một kỳ mà không ai giải thích được.
    /// <para>Chủ nhiệm xoá bản nháp của mình; Staff xoá kỳ do chính Staff sinh nhầm bằng lịch.</para>
    /// </summary>
    public async Task DeleteAsync(Guid reportId, Guid actingUserId, bool isStaff, string? reason = null)
    {
        var report = await _contracts.ProgressReports
            .Include(r => r.Contract).ThenInclude(c => c.Project)
            .FirstOrDefaultAsync(r => r.Id == reportId)
            ?? throw new KeyNotFoundException($"Không tìm thấy báo cáo tiến độ {reportId}.");

        if (!isStaff && report.Contract.Project.PiUserId != actingUserId)
            throw new ForbiddenException("Chỉ chủ nhiệm đề tài hoặc phòng QLKH mới xoá được báo cáo này.");

        // Đã có kết luận hội đồng ⇒ không xoá thẳng: phải "Mở lại" (ghi lý do) trước, để sổ quyết
        // định còn dấu vết vì sao một kết luận biến mất.
        if (!string.IsNullOrWhiteSpace(report.EvaluationResult))
            throw new InvalidOperationException(
                "Kỳ báo cáo này đã có kết luận của hội đồng — bấm \"Mở lại\" (ghi lý do) trước rồi mới xoá được.");

        // PI chỉ xoá bản nháp. Phòng QLKH xoá được cả bản ĐÃ NỘP (nộp nhầm, nộp sai kỳ). 03/10: mọi lần
        // Phòng QLKH xoá một kỳ (kể cả kỳ nháp — tức là bớt một kỳ khỏi lịch) đều phải ghi lý do và vào sổ
        // quyết định, vì số kỳ tiến độ quyết định cả việc giải ngân các đợt giữa.
        if (report.Status != ProgressReportStatus.Draft && !isStaff)
            throw new InvalidOperationException(
                $"Báo cáo đang ở trạng thái {StatusText.Vi(report.Status)} — chủ nhiệm chỉ xoá được bản nháp. " +
                "Nếu nộp nhầm, liên hệ phòng QLKH.");
        if (isStaff)
        {
            if (string.IsNullOrWhiteSpace(reason))
                throw new ArgumentException("Phòng QLKH xoá kỳ báo cáo tiến độ phải ghi lý do.");
            var ky = string.IsNullOrWhiteSpace(report.RoundName) ? $"kỳ {report.ReportRound}" : report.RoundName;
            _decisions.Log(
                report.Contract.ProjectId, DecisionTypes.ResultReopened,
                report.Status == ProgressReportStatus.Draft
                    ? $"Phòng QLKH bỏ kỳ báo cáo tiến độ {ky} khỏi lịch"
                    : $"Phòng QLKH xoá báo cáo tiến độ {ky} đã nộp",
                "ProgressReport", report.Id.ToString(),
                result: "DELETED", reason: reason.Trim(), decidedBy: actingUserId, decidedByRole: "Phòng QLKH");
        }

        var items = await _contracts.ProgressReportItems.Where(i => i.ReportId == reportId).ToListAsync();
        if (items.Count > 0) _contracts.RemoveProgressReportItemsRange(items);

        _contracts.RemoveProgressReportsRange(new[] { report });
        await _contracts.SaveChangesAsync();
    }

    /// <summary>
    /// Mở lại kết luận đã ghi (03/10): ghi nhầm kết luận, tải nhầm biên bản, hay hội đồng họp lại.
    /// Báo cáo trở về "Đã nộp" — PI sửa và nộp lại được, Phòng QLKH ghi nhận kết luận mới. Kết luận
    /// cũ không mất dấu: sổ quyết định giữ cả dòng cũ lẫn dòng "mở lại" kèm lý do.
    /// Chặn khi kết luận "Đạt" đã dùng để giải ngân đợt kế tiếp — tiền đã chi thì không rút lại bằng
    /// một cú bấm.
    /// </summary>
    public async Task<ProgressReportDto> ReopenAsync(Guid reportId, string reason, Guid staffId)
    {
        if (string.IsNullOrWhiteSpace(reason))
            throw new ArgumentException("Mở lại kết luận phải ghi lý do.");

        var report = await _contracts.ProgressReports
            .Include(r => r.Contract).ThenInclude(c => c.Project)
            .FirstOrDefaultAsync(r => r.Id == reportId)
            ?? throw new KeyNotFoundException("Không tìm thấy báo cáo tiến độ.");

        if (string.IsNullOrWhiteSpace(report.EvaluationResult))
            throw new InvalidOperationException("Kỳ báo cáo này chưa có kết luận nên không có gì để mở lại.");

        // Điều 16.1.b: đợt giải ngân (kỳ + 1) mở nhờ kỳ này Đạt.
        var nextTranche = report.ReportRound + 1;
        var paid = await _contracts.Disbursements.AnyAsync(d =>
            d.ContractId == report.ContractId && d.RoundNumber == nextTranche && d.Status == DisbursementStatus.Disbursed);
        if (paid && report.EvaluationResult == "PASS")
            throw new InvalidOperationException(
                $"Kết luận Đạt của kỳ này đã dùng để giải ngân đợt {nextTranche} — không mở lại được. " +
                "Cần điều chỉnh thì lập đơn điều chỉnh / quyết định riêng.");

        var oldResult = ProgressResultVi(report.EvaluationResult);
        report.Status = ProgressReportStatus.Submitted;
        report.EvaluationResult = null;
        report.EvaluationComments = null;
        report.EvaluatedAt = null;
        report.EvaluatedBy = null;
        report.UpdatedAt = _clock.UtcNow;

        var ky = string.IsNullOrWhiteSpace(report.RoundName) ? $"kỳ {report.ReportRound}" : report.RoundName;
        _decisions.Log(
            report.Contract.ProjectId, DecisionTypes.ResultReopened,
            $"Mở lại kết luận đánh giá tiến độ {ky} (trước đó: {oldResult})",
            "ProgressReport", report.Id.ToString(),
            result: "REOPENED", reason: reason.Trim(), decidedBy: staffId, decidedByRole: "Phòng QLKH");
        await _contracts.SaveChangesAsync();

        await _notifier.NotifyAsync(
            report.Contract.Project.PiUserId,
            "PROGRESS_REPORT_REOPENED",
            "Kết luận tiến độ được mở lại",
            $"Phòng QLKH mở lại kết luận báo cáo tiến độ {ky} của đề tài \"{report.Contract.Project.TitleVi}\". " +
            $"Lý do: {reason.Trim()}",
            actionUrl: "/progress-reports",
            entityType: "ProgressReport",
            entityId: report.Id.ToString(),
            priority: "HIGH");

        return await GetByIdAsync(reportId);
    }

    public async Task<ProgressReportDto> EvaluateAsync(Guid reportId, EvaluateProgressReportRequest request, Guid staffId)
    {
        // Chỉ Đạt / Không đạt (03/10). QĐ543 không có mức "Đạt có điều kiện": Điều 16.1.b chỉ hỏi giai
        // đoạn có đánh giá "Đạt" hay không để giải ngân đợt sau — mức thứ ba không mở được gì mà cũng
        // không chặn được gì, chỉ làm Staff phân vân. "Không đạt" KHÔNG chấm dứt hợp đồng: nó giữ đợt
        // giải ngân kế tiếp lại, còn đình chỉ hay gia hạn là Hiệu trưởng quyết theo Điều 10.3.
        var validResults = new[] { "PASS", "FAIL" };
        if (!validResults.Contains(request.EvaluationResult))
            throw new ArgumentException("Kết luận của hội đồng tiến độ chỉ nhận Đạt (PASS) hoặc Không đạt (FAIL).");

        var report = await _contracts.ProgressReports
            .FirstOrDefaultAsync(r => r.Id == reportId)
            ?? throw new KeyNotFoundException("Không tìm thấy báo cáo tiến độ.");

        if (report.Status != ProgressReportStatus.Submitted)
            throw new InvalidOperationException($"Báo cáo đang ở trạng thái {StatusText.Vi(report.Status)} — chỉ đánh giá được báo cáo đã nộp.");

        // Thầy 29/07: "Staff phải xem được bản báo cáo mới đánh giá". Trước đây luật này CHỈ khoá ở
        // giao diện (nút mờ đi) — gọi thẳng API là đánh giá được báo cáo trắng trơn, không có gì để
        // đọc. Chấp nhận một trong hai: file đính kèm HOẶC link PI dán (file lớn thì dán link).
        var reportDocs = await _documents.Query()
            .Where(d => d.EntityType == "ProgressReport" && d.EntityId == reportId.ToString() && !d.IsDeleted)
            .Select(d => d.DocumentCategory)
            .ToListAsync();
        // Biên bản họp KHÔNG được tính là "bản báo cáo của PI".
        var hasAttachment = reportDocs.Any(c => c != "PROGRESS_MINUTES");
        if (!hasAttachment && string.IsNullOrWhiteSpace(report.ReportFileUrl))
            throw new InvalidOperationException(
                "Báo cáo chưa có file đính kèm lẫn link — chưa có gì để đọc thì chưa đánh giá được. " +
                "Đề nghị chủ nhiệm bổ sung bản báo cáo trước.");

        // QĐ543 Điều 10.1: chủ nhiệm báo cáo tiến độ "trong buổi họp với Hội đồng đánh giá tiến độ do
        // Trường tổ chức". Trước 01/10 Phòng QLKH tự chọn Đạt/Không đạt — thầy góp ý: cán bộ phòng không
        // có chuyên môn để đánh giá. Nay Phòng QLKH chỉ GHI NHẬN kết luận của hội đồng, và phải có biên
        // bản họp làm bằng chứng. Hệ thống KHÔNG quản lý thành viên/phiếu của hội đồng tiến độ (khác hội
        // đồng xét duyệt, nghiệm thu) — đó là giới hạn đã nêu trong tài liệu.
        if (!reportDocs.Contains("PROGRESS_MINUTES"))
            throw new InvalidOperationException(
                "Chưa có biên bản họp Hội đồng đánh giá tiến độ (QĐ543 Điều 10.1) — tải biên bản lên trước khi " +
                "ghi nhận kết luận của hội đồng.");

        report.Status = ProgressReportStatus.Evaluated;
        report.EvaluatedBy = staffId;
        report.EvaluatedAt = DateTime.UtcNow;
        report.EvaluationResult = request.EvaluationResult;
        report.EvaluationComments = request.EvaluationComments;
        report.UpdatedAt = DateTime.UtcNow;

        var projectId = await _contracts.Query()
            .Where(c => c.Id == report.ContractId)
            .Select(c => c.ProjectId)
            .FirstOrDefaultAsync();
        if (projectId != Guid.Empty)
        {
            var ky = string.IsNullOrWhiteSpace(report.RoundName)
                ? $"kỳ {report.ReportRound}" : report.RoundName;
            _decisions.Log(
                projectId, DecisionTypes.ProgressEvaluated,
                $"Hội đồng đánh giá tiến độ {ky}: {ProgressResultVi(request.EvaluationResult)}",
                "ProgressReport", report.Id.ToString(),
                result: request.EvaluationResult, reason: request.EvaluationComments,
                documentNo: "BM06", decidedBy: staffId, decidedByRole: "Hội đồng đánh giá tiến độ (Phòng QLKH ghi nhận)");
        }

        await _contracts.SaveChangesAsync();

        // Báo chủ nhiệm KẾT QUẢ đánh giá. Trước đây hệ thống im lặng — PI nộp xong không biết
        // đã được duyệt chưa, mà kết quả "Không đạt" thì họ phải biết NGAY
        // để còn kịp khắc phục; kỳ sau chỉ mở khi kỳ trước đã có kết quả.
        var info = await _contracts.ProgressReports
            .Where(r => r.Id == reportId)
            .Select(r => new { r.Contract!.Project!.PiUserId, Title = r.Contract!.Project!.TitleVi, r.ReportRound })
            .FirstOrDefaultAsync();
        if (info != null)
        {
            var ketQua = ProgressResultVi(request.EvaluationResult);
            var nhanXet = string.IsNullOrWhiteSpace(report.EvaluationComments)
                ? ""
                : $" Nhận xét: {report.EvaluationComments}";
            await _notifier.NotifyAsync(
                info.PiUserId,
                "PROGRESS_REPORT_EVALUATED",
                $"Kết quả đánh giá báo cáo tiến độ kỳ {info.ReportRound}: {ketQua}",
                $"Báo cáo tiến độ kỳ {info.ReportRound} của đề tài \"{info.Title}\" đã được đánh giá: " +
                $"{ketQua}.{nhanXet}",
                actionUrl: "/progress-reports",
                entityType: "ProgressReport",
                entityId: reportId.ToString(),
                priority: request.EvaluationResult == "PASS" ? "NORMAL" : "HIGH");
        }

        return await GetByIdAsync(reportId);
    }

    public async Task<ProgressReportDto> ScheduleAsync(Guid reportId, ScheduleProgressReportRequest request)
    {
        var report = await _contracts.ProgressReports
            .Include(r => r.Contract)
            .FirstOrDefaultAsync(r => r.Id == reportId)
            ?? throw new KeyNotFoundException("Không tìm thấy báo cáo tiến độ.");

        var now = _clock.UtcNow;
        var today = DateOnly.FromDateTime(now.AddHours(7));
        var effectiveDueDate = report.DueDate;

        if (!string.IsNullOrWhiteSpace(request.DueDate))
        {
            if (!DateOnly.TryParse(request.DueDate, out var due))
                throw new ArgumentException("DueDate phải là ngày hợp lệ (yyyy-MM-dd).");
            if (due < today)
                throw new ArgumentException($"Hạn nộp {due:dd/MM/yyyy} đã ở trong quá khứ — hãy chọn từ ngày {today:dd/MM/yyyy} trở đi.");
            if (due < report.Contract.StartDate || due > report.Contract.EndDate)
                throw new ArgumentException(
                    $"Hạn nộp phải nằm trong thời gian hợp đồng từ {report.Contract.StartDate:dd/MM/yyyy} đến {report.Contract.EndDate:dd/MM/yyyy}.");
            if (due < report.ReportingPeriodStart)
                throw new ArgumentException(
                    $"Hạn nộp không thể trước ngày bắt đầu kỳ báo cáo {report.ReportingPeriodStart:dd/MM/yyyy}.");
            report.DueDate = due;
            effectiveDueDate = due;
        }
        if (!string.IsNullOrWhiteSpace(request.ScheduledMeetingAt))
        {
            if (!DateTimeOffset.TryParse(request.ScheduledMeetingAt, out var parsedAt))
                throw new ArgumentException("ScheduledMeetingAt phải là thời điểm hợp lệ.");
            var at = parsedAt.UtcDateTime;
            var meetingDate = DateOnly.FromDateTime(at.AddHours(7));
            if (at <= now)
                throw new ArgumentException("Thời gian họp đã ở trong quá khứ — hãy chọn một thời điểm trong tương lai.");
            if (meetingDate < report.Contract.StartDate || meetingDate > report.Contract.EndDate)
                throw new ArgumentException(
                    $"Buổi họp phải nằm trong thời gian hợp đồng từ {report.Contract.StartDate:dd/MM/yyyy} đến {report.Contract.EndDate:dd/MM/yyyy}.");
            if (meetingDate < report.ReportingPeriodStart)
                throw new ArgumentException(
                    $"Buổi họp không thể trước ngày bắt đầu kỳ báo cáo {report.ReportingPeriodStart:dd/MM/yyyy}.");
            if (effectiveDueDate.HasValue && meetingDate < effectiveDueDate.Value)
                throw new ArgumentException(
                    $"Buổi họp không thể diễn ra trước hạn nộp {effectiveDueDate.Value:dd/MM/yyyy}.");
            report.ScheduledMeetingAt = at;
        }

        if (effectiveDueDate.HasValue && report.ScheduledMeetingAt.HasValue
            && DateOnly.FromDateTime(report.ScheduledMeetingAt.Value.AddHours(7)) < effectiveDueDate.Value)
            throw new ArgumentException(
                $"Buổi họp hiện tại không thể diễn ra trước hạn nộp mới {effectiveDueDate.Value:dd/MM/yyyy} — hãy đổi cả thời gian họp.");
        report.MeetingLink = request.MeetingLink;
        if (request.RoundName != null)
            report.RoundName = string.IsNullOrWhiteSpace(request.RoundName) ? null : request.RoundName.Trim();
        report.UpdatedAt = DateTime.UtcNow;
        await _contracts.SaveChangesAsync();

        return await GetByIdAsync(reportId);
    }

    private ProgressReportSummaryDto MapSummary(ProgressReport r) => new()
    {
        Id = r.Id,
        ContractId = r.ContractId,
        ReportRound = r.ReportRound,
        RoundName = r.RoundName,
        ReportFileUrl = r.ReportFileUrl,
        ReportingPeriodStart = r.ReportingPeriodStart.ToString("yyyy-MM-dd"),
        ReportingPeriodEnd = r.ReportingPeriodEnd.ToString("yyyy-MM-dd"),
        OverallCompletionPct = r.OverallCompletionPct,
        ExpenditureToDate = r.ExpenditureToDate,
        Status = r.Status,
        SubmittedAt = r.SubmittedAt,
        CreatedAt = r.CreatedAt,
        DueDate = r.DueDate?.ToString("yyyy-MM-dd"),
        DaysLeft = DeadlineMath.DaysLeft(r.DueDate, _clock),
        ScheduledMeetingAt = r.ScheduledMeetingAt,
        MeetingLink = r.MeetingLink,
        EvaluationResult = r.EvaluationResult,
        EvaluationComments = r.EvaluationComments,
        EvaluatedAt = r.EvaluatedAt
    };

    private ProgressReportDto MapDetail(ProgressReport r) => new()
    {
        Id = r.Id,
        ContractId = r.ContractId,
        ReportRound = r.ReportRound,
        RoundName = r.RoundName,
        // Phải map ở CẢ HAI mapper. Thiếu ở đây thì danh sách trả link mà chi tiết trả null —
        // dialog đánh giá của Staff đọc chi tiết nên báo "PI chưa nộp file" và khoá nút lưu,
        // dù PI đã nộp link hẳn hoi.
        ReportFileUrl = r.ReportFileUrl,
        ReportingPeriodStart = r.ReportingPeriodStart.ToString("yyyy-MM-dd"),
        ReportingPeriodEnd = r.ReportingPeriodEnd.ToString("yyyy-MM-dd"),
        CompletedContent = r.CompletedContent,
        PendingContent = r.PendingContent,
        OverallCompletionPct = r.OverallCompletionPct,
        ExpenditureToDate = r.ExpenditureToDate,
        NextPeriodPlan = r.NextPeriodPlan,
        PiRecommendations = r.PiRecommendations,
        Status = r.Status,
        SubmittedAt = r.SubmittedAt,
        EvaluationResult = r.EvaluationResult,
        EvaluationComments = r.EvaluationComments,
        EvaluatedAt = r.EvaluatedAt,
        CreatedAt = r.CreatedAt,
        DueDate = r.DueDate?.ToString("yyyy-MM-dd"),
        DaysLeft = DeadlineMath.DaysLeft(r.DueDate, _clock),
        ScheduledMeetingAt = r.ScheduledMeetingAt,
        MeetingLink = r.MeetingLink,
        Items = r.Items.Select(i => new ProgressReportItemDto
        {
            Id = i.Id,
            ActivityId = i.ActivityId,
            ActivityName = i.Activity?.ActivityName ?? "—",
            CompletionRate = i.CompletionRate,
            CompletionStatus = i.CompletionStatus,
            EvidenceDescription = i.EvidenceDescription,
            Notes = i.Notes
        }).ToList()
    };
}
