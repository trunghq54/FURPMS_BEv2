using FURPMS.Application.Constants;
using FURPMS.Application.DTOs.Contract;
using FURPMS.Application.Interfaces;
using FURPMS.Application.Interfaces.Repositories;
using FURPMS.Application.Interfaces.Services;
using FURPMS.Domain.Entities.Contracts;
using FURPMS.Domain.Entities.Financial;
using Microsoft.EntityFrameworkCore;

namespace FURPMS.Infrastructure.Services;

public class DisbursementService : IDisbursementService
{
    private readonly IContractRepository _contracts;
    private readonly IMasterDataRepository _masterData;
    private readonly IClock _clock;
    private readonly ISystemSettingService _settings;
    private readonly INotifier _notifier;
    private readonly IDocumentRepository _documents;
    private readonly IDecisionLogger _decisions;

    public DisbursementService(IContractRepository contracts, IMasterDataRepository masterData,
        IClock clock,
        ISystemSettingService settings,
        INotifier notifier,
        IDocumentRepository documents,
        IDecisionLogger decisions)
    {
        _decisions = decisions;
        _notifier = notifier;
        _contracts = contracts;
        _masterData = masterData;
        _clock = clock;
        _settings = settings;
        _documents = documents;
    }

    public async Task<IEnumerable<DisbursementResponse>> GetByContractAsync(Guid contractId)
    {
        _ = await _contracts.Query().FirstOrDefaultAsync(c => c.Id == contractId)
            ?? throw new KeyNotFoundException("Không tìm thấy hợp đồng.");

        var items = await _contracts.Disbursements
            .Include(d => d.Deliverable)
            .Where(d => d.ContractId == contractId)
            .OrderBy(d => d.RoundNumber)
            .ToListAsync();

        var itemIds = items.Select(x => x.Id.ToString()).ToList();
        var evidenceIdList = await _documents.Query()
            .Where(x => x.EntityType == "Disbursement" && itemIds.Contains(x.EntityId) && !x.IsDeleted)
            .Select(x => x.EntityId)
            .Distinct()
            .ToListAsync();
        var evidenceIds = evidenceIdList.ToHashSet();

        var ctx = await LoadLockContextAsync(contractId);
        return items.Select(x =>
        {
            var dto = Map(x, evidenceIds.Contains(x.Id.ToString()));
            if (x.Status != DisbursementStatus.Disbursed && ctx != null) dto.LockReason = LockReason(x, ctx);
            return dto;
        });
    }

    /// <summary>
    /// Sinh lại lịch giải ngân (03/10). Lịch chỉ sinh MỘT lần: hợp đồng sinh lịch lúc loại đề tài còn
    /// cấu hình sai (chưa đánh dấu Ứng dụng), hay trước bản sửa 01/10, là kẹt mãi ở 1 đợt 100% "sau
    /// nghiệm thu" — đúng lỗi bạn nhóm gặp trên deploy: đề tài ứng dụng mà "Đợt 1 là đợt CUỐI".
    /// Chỉ cho khi CHƯA đợt nào đã chi; minh chứng đã tải của các đợt cũ bị gỡ theo. Ghi sổ quyết định.
    /// </summary>
    public async Task<IEnumerable<DisbursementResponse>> RegenerateAsync(Guid contractId, string reason, Guid staffId)
    {
        if (string.IsNullOrWhiteSpace(reason))
            throw new ArgumentException("Sinh lại lịch giải ngân phải ghi lý do.");

        var contract = await _contracts.Query()
            .Include(c => c.Project)
            .Include(c => c.Disbursements)
            .FirstOrDefaultAsync(c => c.Id == contractId)
            ?? throw new KeyNotFoundException("Không tìm thấy hợp đồng.");

        if (contract.Disbursements.Any(d => d.Status == DisbursementStatus.Disbursed))
            throw new InvalidOperationException(
                "Đã có đợt được đánh dấu đã chi — không sinh lại cả lịch được. Huỷ xác nhận đợt đó trước nếu bấm nhầm.");

        var oldIds = contract.Disbursements.Select(d => d.Id.ToString()).ToList();
        var oldCount = contract.Disbursements.Count;
        var evidence = await _documents.Query()
            .Where(x => x.EntityType == "Disbursement" && oldIds.Contains(x.EntityId) && !x.IsDeleted)
            .ToListAsync();
        foreach (var doc in evidence) { doc.IsDeleted = true; _documents.Update(doc); }
        _contracts.RemoveDisbursementsRange(contract.Disbursements.ToList());
        await _documents.SaveChangesAsync();
        await _contracts.SaveChangesAsync();

        var tranches = await GenerateFromTemplateAsync(contract);
        _contracts.AddDisbursementsRange(tranches);

        _decisions.Log(
            contract.ProjectId, DecisionTypes.ResultReopened,
            $"Sinh lại lịch giải ngân hợp đồng {contract.ContractNumber}: {oldCount} đợt → {tranches.Count} đợt",
            "Contract", contract.Id.ToString(),
            result: "REGENERATED", reason: reason.Trim(), decidedBy: staffId, decidedByRole: "Phòng QLKH");
        await _contracts.SaveChangesAsync();

        return await GetByContractAsync(contractId);
    }

    /// <summary>
    /// Huỷ "đã giải ngân" bấm nhầm (03/10) — đợt về Chờ, giữ minh chứng đã tải. Chặn khi đã lập hồ sơ
    /// thanh lý: số "đã chi" đã chép sang biên bản, phải xoá/huỷ ký thanh lý trước.
    /// </summary>
    public async Task<DisbursementResponse> UndoConfirmAsync(int disbursementId, string reason, Guid staffId)
    {
        if (string.IsNullOrWhiteSpace(reason))
            throw new ArgumentException("Huỷ xác nhận giải ngân phải ghi lý do.");

        var d = await _contracts.Disbursements
            .Include(x => x.Deliverable)
            .Include(x => x.Contract)
            .FirstOrDefaultAsync(x => x.Id == disbursementId)
            ?? throw new KeyNotFoundException("Không tìm thấy đợt giải ngân.");

        if (d.Status != DisbursementStatus.Disbursed)
            throw new InvalidOperationException("Đợt này chưa được đánh dấu đã chi nên không có gì để huỷ.");
        if (await _contracts.Settlements.AnyAsync(s => s.ContractId == d.ContractId))
            throw new InvalidOperationException(
                "Hợp đồng đã lập hồ sơ quyết toán — xoá hồ sơ quyết toán trước rồi mới huỷ xác nhận giải ngân.");

        d.Status = DisbursementStatus.Pending;
        d.DisbursedAt = null;
        d.ProcessedBy = null;
        _decisions.Log(
            d.Contract.ProjectId, DecisionTypes.ResultReopened,
            $"Huỷ xác nhận đã giải ngân đợt {d.RoundNumber}",
            "ContractDisbursement", d.Id.ToString(),
            result: "REOPENED", reason: reason.Trim(), decidedBy: staffId, decidedByRole: "Phòng QLKH");
        await _contracts.SaveChangesAsync();

        var hasEvidence = await _documents.Query().AnyAsync(x =>
            x.EntityType == "Disbursement" && x.EntityId == d.Id.ToString() && !x.IsDeleted);
        var dto = Map(d, hasEvidence);
        var ctx = await LoadLockContextAsync(d.ContractId);
        if (ctx != null) dto.LockReason = LockReason(d, ctx);
        return dto;
    }

    public async Task<DisbursementResponse> LinkDeliverableAsync(int disbursementId, LinkDeliverableRequest request)
    {
        var d = await _contracts.Disbursements
            .Include(x => x.Deliverable)
            .FirstOrDefaultAsync(x => x.Id == disbursementId)
            ?? throw new KeyNotFoundException("Không tìm thấy đợt giải ngân.");

        if (d.Status == DisbursementStatus.Disbursed)
            throw new InvalidOperationException(
                "Đợt này đã đánh dấu giải ngân — không đổi được sản phẩm minh chứng nữa.");

        if (request.DeliverableId is int deliverableId)
        {
            // Chỉ cho gắn sản phẩm CÙNG hợp đồng, tránh lấy minh chứng của đề tài khác.
            var deliverable = await _contracts.Deliverables
                .FirstOrDefaultAsync(x => x.Id == deliverableId && x.ContractId == d.ContractId)
                ?? throw new ArgumentException(
                    $"Sản phẩm {deliverableId} không thuộc hợp đồng của đợt giải ngân này.");

            d.DeliverableId = deliverable.Id;
            d.Deliverable = deliverable;

            // Sản phẩm đã nghiệm thu từ trước ⇒ điều kiện coi như đạt ngay khi gắn.
            d.ConditionMetAt = deliverable.AcceptanceStatus == AcceptanceStatus.Passed
                ? d.ConditionMetAt ?? _clock.UtcNow
                : null;
        }
        else
        {
            d.DeliverableId = null;
            d.Deliverable = null;
            d.ConditionMetAt = null;
        }

        await _contracts.SaveChangesAsync();
        return Map(d);
    }

    public async Task<IEnumerable<DisbursementResponse>> GenerateAsync(Guid contractId)
    {
        var contract = await _contracts.Query()
            .Include(c => c.Project).ThenInclude(p => p.Proposals.Where(x => x.IsCurrent))
            .Include(c => c.Disbursements)
            .FirstOrDefaultAsync(c => c.Id == contractId)
            ?? throw new KeyNotFoundException("Không tìm thấy hợp đồng.");

        if (contract.Disbursements.Any())
            throw new InvalidOperationException("Hợp đồng này đã sinh các đợt giải ngân rồi.");

        // QĐ543 **Điều 16**: lịch giải ngân do **LOẠI ĐỀ TÀI** quyết định, KHÔNG phải lựa chọn của
        // chủ nhiệm. Ứng dụng 4 đợt 30–30–30–10; Cơ bản 1 lần sau nghiệm thu "Đạt".
        //
        // Trước đây code chia theo `Proposal.FundingMethod` (WHOLE/PARTIAL) — khái niệm "phương
        // thức khoán chi" lấy từ mẫu thuyết minh cấp Bộ, **không có trong QĐ543** (rà toàn văn:
        // chữ "khoán" chỉ xuất hiện ở "thuê khoán chuyên môn" và "giao khoán", không có mục nào
        // cho chủ nhiệm chọn kiểu giải ngân). Chia sai gốc thì số đợt và điều kiện mở đều lệch.
        var tranches = await GenerateFromTemplateAsync(contract);

        _contracts.AddDisbursementsRange(tranches);
        await _contracts.SaveChangesAsync();

        return tranches.Select(x => Map(x));
    }

    public async Task<DisbursementResponse> ConfirmAsync(int disbursementId, ConfirmDisbursementRequest request, Guid processedBy)
    {
        var d = await _contracts.Disbursements
            .Include(x => x.Deliverable)
            .FirstOrDefaultAsync(x => x.Id == disbursementId)
            ?? throw new KeyNotFoundException("Không tìm thấy đợt giải ngân.");

        if (d.Status == DisbursementStatus.Disbursed)
            throw new InvalidOperationException("Đợt giải ngân này đã được đánh dấu đã chi.");

        var hasEvidence = await _documents.Query().AnyAsync(x =>
            x.EntityType == "Disbursement" && x.EntityId == d.Id.ToString() && !x.IsDeleted);
        if (!hasEvidence)
            throw new InvalidOperationException(
                "Phải tải lên ít nhất một file minh chứng (hợp đồng/chứng từ) trước khi đánh dấu đã giải ngân.");

        // Đợt nào có gắn sản phẩm minh chứng thì sản phẩm phải nghiệm thu ĐẠT rồi mới
        // được đánh dấu đã giải ngân (QĐ543 Điều 16 — giải ngân theo tiến độ thực hiện).
        // Đợt KHÔNG gắn sản phẩm (vd tạm ứng khởi động hợp đồng) vẫn cho qua bình thường.
        if (d.Deliverable is not null && d.Deliverable.AcceptanceStatus != AcceptanceStatus.Passed)
            throw new InvalidOperationException(
                $"Sản phẩm minh chứng \"{d.Deliverable.ProductName}\" chưa nghiệm thu Đạt " +
                "— chưa thể đánh dấu đã giải ngân đợt này.");

        await AssertMilestoneUnlockedAsync(d);

        if (request.ActualAmount.HasValue) d.ActualAmount = request.ActualAmount;   // optional, không bắt buộc
        if (!string.IsNullOrWhiteSpace(request.BankReference)) d.BankReference = request.BankReference;
        d.Notes = request.Notes;
        d.Status = DisbursementStatus.Disbursed;
        d.DisbursedAt = _clock.UtcNow;
        d.ProcessedBy = processedBy;

        var disbProjectId = await _contracts.Query()
            .Where(c => c.Id == d.ContractId)
            .Select(c => c.ProjectId)
            .FirstOrDefaultAsync();
        if (disbProjectId != Guid.Empty)
        {
            // Số tiền: ưu tiên số thực chi Phòng Tài chính báo; chưa có thì nói rõ là TẠM TÍNH
            // theo kế hoạch — hệ thống không được để người đọc tưởng đó là số quyết toán (rule #15).
            var soTien = d.ActualAmount ?? d.PlannedAmount;
            var tamTinh = d.ActualAmount is null ? " (tạm tính theo kế hoạch)" : "";
            _decisions.Log(
                disbProjectId, DecisionTypes.DisbursementConfirmed,
                $"Xác nhận giải ngân đợt {d.RoundNumber} — {d.Percentage:0.##}%, {soTien:N0} đ{tamTinh}",
                "ContractDisbursement", d.Id.ToString(),
                result: "DISBURSED", reason: d.BankReference,
                decidedBy: processedBy, decidedByRole: "Phòng QLKH");
        }

        await _contracts.SaveChangesAsync();

        // Báo chủ nhiệm mốc giải ngân đã được xác nhận. Trước đây hệ thống im lặng: PI chỉ biết
        // khi tự vào xem hợp đồng, mà đây là mốc họ chờ nhất — tiền về mới triển khai tiếp được.
        // Hệ thống KHÔNG quản tiền (rule #15) nên thông báo nói về MỐC và MINH CHỨNG, không nói số.
        var piInfo = await _contracts.Query()
            .Where(x => x.Id == d.ContractId)
            .Select(x => new { x.Project!.PiUserId, x.ContractNumber, Title = x.Project!.TitleVi })
            .FirstOrDefaultAsync();
        if (piInfo != null)
        {
            await _notifier.NotifyAsync(
                piInfo.PiUserId,
                "DISBURSEMENT_CONFIRMED",
                $"Đã xác nhận giải ngân đợt {d.RoundNumber}",
                $"Đợt giải ngân số {d.RoundNumber} của hợp đồng {piInfo.ContractNumber} " +
                $"(đề tài \"{piInfo.Title}\") đã được đánh dấu đã chi. " +
                "Xem chứng từ ở mục Tiến trình đề tài → phần Giải ngân của hợp đồng.",
                actionUrl: "/my-timeline",
                entityType: "Contract",
                entityId: d.ContractId.ToString());
        }

        return Map(d, hasEvidence);
    }

    /// <summary>
    /// Khóa mốc giải ngân theo QĐ543 Điều 16: hợp đồng phải đã ký; đề tài ứng dụng cần tiến độ giai đoạn
    /// tương ứng đạt cho đợt 2/3; đợt cuối (kể cả đề tài cơ bản chỉ có một đợt) cần nghiệm thu Đạt.
    /// <para>
    /// Mốc "công nhận Đạt" đọc qua <c>Project.Status == COMPLETED</c> — trạng thái này CHỈ được đặt
    /// ở một đường duy nhất: Chủ tịch chốt biên bản vòng NGHIỆM THU với kết quả Đạt
    /// (<c>ReviewScoringService.ApproveMinutesAsync</c>).
    /// </para>
    /// </summary>
    private async Task AssertMilestoneUnlockedAsync(ContractDisbursement d)
    {
        var ctx = await LoadLockContextAsync(d.ContractId);
        if (ctx == null) return;
        // Điều kiện sản phẩm đã chặn riêng ở ConfirmAsync (thông báo cụ thể hơn) — ở đây chỉ mốc QĐ543.
        var reason = LockReason(d, ctx, includeDeliverable: false);
        if (reason != null)
            throw new InvalidOperationException($"Chưa thể xác nhận giải ngân đợt {d.RoundNumber}: {reason}");
    }

    private sealed record LockContext(
        DateTime? SignedAt, string ProjectStatus, bool IsApplied,
        List<int> Rounds, Dictionary<int, string?> ProgressResults);

    private async Task<LockContext?> LoadLockContextAsync(Guid contractId)
    {
        var contract = await _contracts.Query()
            .Where(c => c.Id == contractId)
            .Select(c => new
            {
                c.SignedAt,
                ProjectStatus = c.Project.Status,
                IsApplied = c.Project.ResearchType.RequireOrderingUnit
            })
            .FirstOrDefaultAsync();
        if (contract == null) return null;

        var rounds = await _contracts.Disbursements
            .Where(x => x.ContractId == contractId)
            .Select(x => x.RoundNumber)
            .ToListAsync();
        var progress = await _contracts.ProgressReports
            .Where(r => r.ContractId == contractId)
            .Select(r => new { r.ReportRound, r.EvaluationResult })
            .ToListAsync();
        return new LockContext(contract.SignedAt, contract.ProjectStatus, contract.IsApplied, rounds,
            progress.GroupBy(p => p.ReportRound).ToDictionary(g => g.Key, g => g.First().EvaluationResult));
    }

    /// <summary>
    /// Đợt này đang chờ điều kiện gì theo QĐ543 Điều 16 — <c>null</c> = đủ điều kiện giải ngân.
    /// Một luật duy nhất cho cả lúc HIỂN THỊ (vì sao chưa mở) và lúc XÁC NHẬN (chặn API).
    /// <list type="bullet">
    /// <item>Mọi đợt: hợp đồng phải đã ký (Điều 16.1.a).</item>
    /// <item>Đợt CUỐI (đợt 4 ứng dụng; đợt duy nhất của đề tài cơ bản): sau nghiệm thu Đạt (16.1.c, 16.2).</item>
    /// <item>Đợt giữa của đề tài ứng dụng: báo cáo tiến độ kỳ trước đó phải được kết luận Đạt (16.1.b).</item>
    /// </list>
    /// </summary>
    private static string? LockReason(ContractDisbursement d, LockContext ctx, bool includeDeliverable = true)
    {
        if (ctx.SignedAt == null)
            return "hợp đồng chưa được ghi nhận đã ký (QĐ543 Điều 16.1.a).";

        var isFinal = !ctx.Rounds.Any(n => n > d.RoundNumber);
        if (isFinal && ctx.ProjectStatus != ProjectStatus.Completed)
            return $"đây là đợt giải ngân CUỐI — theo QĐ543 chỉ giải ngân sau khi hội đồng nghiệm thu Đạt " +
                   $"(đề tài đang \"{StatusText.Vi(ctx.ProjectStatus)}\").";

        if (ctx.IsApplied && !isFinal && d.RoundNumber >= 2)
        {
            var need = d.RoundNumber - 1;
            ctx.ProgressResults.TryGetValue(need, out var result);
            if (!string.Equals(result, "PASS", StringComparison.OrdinalIgnoreCase))
                return $"chờ hội đồng tiến độ kết luận Đạt cho báo cáo tiến độ kỳ {need} " +
                       $"(QĐ543 Điều 16.1.b; hiện: {StatusText.Vi(result ?? "chưa có kết quả")}).";
        }

        if (includeDeliverable && d.Deliverable is not null && d.Deliverable.AcceptanceStatus != AcceptanceStatus.Passed)
            return $"sản phẩm minh chứng \"{d.Deliverable.ProductName}\" chưa nghiệm thu Đạt.";

        return null;
    }

    /// <summary>
    /// Sinh đợt giải ngân từ bảng mốc chuẩn của **loại đề tài** (QĐ543 Điều 16).
    /// <para>
    /// Loại đề tài chưa có mốc nào thì sinh theo đúng Điều 16, dựa vào cờ <c>RequireOrderingUnit</c>
    /// (cờ phân biệt Ứng dụng/Cơ bản trong cả hệ thống): Ứng dụng 4 đợt 30–30–30–10, Cơ bản 1 đợt
    /// 100% sau nghiệm thu.
    /// </para>
    /// <para>
    /// Sửa 01/10: trước đó thiếu mốc thì MỌI loại đều ra 1 đợt 100%. Mốc chỉ được seed cho loại có mã
    /// đúng "APPLIED"/"BASIC" (mà hai loại này chỉ tự tạo ở môi trường dev), và không có màn nào cấu
    /// hình mốc — nên trên bản deploy, loại "Ứng dụng" do nhóm tự tạo cũng chỉ có 1 đợt giải ngân.
    /// </para>
    /// </summary>
    /// <summary>Mốc mặc định theo QĐ543 Điều 16 khi loại đề tài chưa cấu hình mốc riêng.</summary>
    private static List<DisbursementTemplate> DefaultTemplates(bool isApplied) => isApplied
        ? new List<DisbursementTemplate>
        {
            new() { RoundNumber = 1, Percentage = 30m, ConditionDescription = "Sau khi ký kết Hợp đồng NCKH với Chủ nhiệm đề tài" },
            new() { RoundNumber = 2, Percentage = 30m, ConditionDescription = "Sau khi đánh giá tiến độ giai đoạn 1 đạt yêu cầu" },
            new() { RoundNumber = 3, Percentage = 30m, ConditionDescription = "Sau khi đánh giá tiến độ giai đoạn 2 đạt yêu cầu" },
            new() { RoundNumber = 4, Percentage = 10m, ConditionDescription = "Sau khi Hội đồng nghiệm thu đánh giá \"Đạt\"" },
        }
        : new List<DisbursementTemplate>
        {
            new() { RoundNumber = 1, Percentage = 100m, ConditionDescription = "Sau khi Hội đồng nghiệm thu đánh giá \"Đạt\"" },
        };

    private async Task<List<ContractDisbursement>> GenerateFromTemplateAsync(
        Domain.Entities.Contracts.Contract contract)
    {
        var templates = await _masterData.DisbursementTemplates
            .Where(t => t.ResearchTypeId == contract.Project.ResearchTypeId && t.IsActive)
            .OrderBy(t => t.RoundNumber)
            .ToListAsync();

        if (templates.Count == 0)
        {
            var isApplied = await _masterData.ResearchTypes
                .Where(t => t.Id == contract.Project.ResearchTypeId)
                .Select(t => t.RequireOrderingUnit)
                .FirstOrDefaultAsync();
            templates = DefaultTemplates(isApplied);
        }

        var list = new List<ContractDisbursement>();
        decimal allocated = 0;
        for (int i = 0; i < templates.Count; i++)
        {
            var t = templates[i];
            // Đợt cuối lấy phần CÒN LẠI để tổng luôn khớp giá trị hợp đồng, tránh lệch vài đồng
            // do làm tròn từng đợt.
            var amount = i == templates.Count - 1
                ? contract.TotalAmount - allocated
                : Math.Round(contract.TotalAmount * t.Percentage / 100m, 0);
            allocated += amount;

            list.Add(new ContractDisbursement
            {
                ContractId = contract.Id,
                RoundNumber = t.RoundNumber,
                Percentage = t.Percentage,
                PlannedAmount = amount,
                ConditionDescription = t.ConditionDescription,
                Status = DisbursementStatus.Pending
            });
        }
        return list;
    }

    private static DisbursementResponse Map(ContractDisbursement d, bool hasEvidence = false) => new()
    {
        Id = d.Id,
        ContractId = d.ContractId,
        RoundNumber = d.RoundNumber,
        Percentage = d.Percentage,
        PlannedAmount = d.PlannedAmount,
        ActualAmount = d.ActualAmount,
        ConditionDescription = d.ConditionDescription,
        ConditionMetAt = d.ConditionMetAt,
        DisbursedAt = d.DisbursedAt,
        BankReference = d.BankReference,
        Status = d.Status,
        Notes = d.Notes,
        DeliverableId = d.DeliverableId,
        DeliverableName = d.Deliverable?.ProductName,
        DeliverableAcceptanceStatus = d.Deliverable?.AcceptanceStatus,
        DeliverableSubmittedAt = d.Deliverable?.SubmittedAt,
        IsBlockedByDeliverable =
            d.Deliverable is not null && d.Deliverable.AcceptanceStatus != AcceptanceStatus.Passed,
        HasEvidence = hasEvidence
    };
}
