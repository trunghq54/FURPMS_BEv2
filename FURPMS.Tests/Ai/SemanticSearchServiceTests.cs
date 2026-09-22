using FURPMS.Application.Common;
using FURPMS.Application.Constants;
using FURPMS.Application.DTOs.Ai;
using FURPMS.Domain.Entities.AI;
using FURPMS.Domain.Entities.Cycles;
using FURPMS.Domain.Entities.MasterData;
using FURPMS.Domain.Entities.Projects;
using FURPMS.Domain.Entities.Proposals;
using FURPMS.Domain.Entities.Users;
using FURPMS.Infrastructure.Data;
using FURPMS.Tests.Helpers;

namespace FURPMS.Tests.Ai;

/// <summary>
/// Hành vi của <see cref="FURPMS.Infrastructure.Services.SemanticSearchService"/> — FE-14.
///
/// <para>Chạy hoàn toàn offline: vector của kho nằm sẵn trong DB in-memory, vector của câu hỏi do
/// test cấp. Không gọi mạng nên luôn xanh trong CI và cho cùng thứ hạng giữa hai lần chạy — đó
/// cũng chính là điều cần nói khi bị hỏi *"xếp hạng này có tất định không"*.</para>
/// </summary>
public class SemanticSearchServiceTests
{
    private const string VectorEntityType =
        FURPMS.Infrastructure.Services.DuplicateCheckService.VectorEntityType;

    private static User NewUser(FURPMSDbContext db, string name)
    {
        var u = new User
        {
            Id = Guid.NewGuid(),
            Email = Guid.NewGuid().ToString("N")[..12] + "@t.com",
            FullName = name,
            Status = UserStatus.Active
        };
        db.Users.Add(u);
        return u;
    }

    /// <summary>
    /// Ba đề cương đã vector hoá trong mặt phẳng 2 chiều, đặt ở ba hướng tách bạch để thứ hạng
    /// không phụ thuộc số lẻ: A ở trục x, B lệch ~30°, C vuông góc với A.
    /// </summary>
    private static async Task<(FURPMSDbContext db, Guid userId, Proposal a, Proposal b, Proposal c)>
        SeedAsync()
    {
        var db = TestDbContextFactory.Create("search-" + Guid.NewGuid());
        var staff = NewUser(db, "Chuyên viên");
        var pi = NewUser(db, "Chủ nhiệm");

        db.ResearchTypes.Add(new ResearchType { Id = 1, Code = "APPLIED", Name = "Ứng dụng", IsActive = true });
        db.ResearchTracks.Add(new ResearchTrack { Id = 1, Code = "IT", Name = "CNTT", IsActive = true });
        await db.SaveChangesAsync();

        var cycle = new ResearchCycle
        {
            CycleYear = 2026,
            SemesterCode = "SU26",
            ResearchTypeId = 1,
            SubmissionOpenDate = DateOnly.FromDateTime(DateTime.UtcNow).AddDays(-30),
            SubmissionDeadline = DateOnly.FromDateTime(DateTime.UtcNow).AddDays(30),
            Status = CycleStatus.Open,
            CreatedBy = staff.Id
        };
        db.ResearchCycles.Add(cycle);
        await db.SaveChangesAsync();

        var track = new CycleTrack { CycleId = cycle.Id, TrackId = 1 };
        db.CycleTracks.Add(track);
        await db.SaveChangesAsync();

        async Task<Proposal> Seed(string code, string title, string snapshot)
        {
            var project = new Project
            {
                Id = Guid.NewGuid(), CycleTrackId = track.Id, OrderId = 1, PiUserId = pi.Id,
                HostingUnitId = 1, ResearchTypeId = 1, ProjectCode = code, TitleVi = title,
                Status = ProjectStatus.UnderReview,
                PlannedStartDate = DateOnly.FromDateTime(DateTime.UtcNow),
                PlannedEndDate = DateOnly.FromDateTime(DateTime.UtcNow).AddMonths(12)
            };
            db.Projects.Add(project);

            var proposal = new Proposal
            {
                Id = Guid.NewGuid(), ProjectId = project.Id, VersionNo = 1, IsCurrent = true,
                TitleVi = title, AbstractVi = "A", ResearchObjectives = "O", DurationMonths = 12,
                PlannedStartDate = DateOnly.FromDateTime(DateTime.UtcNow),
                PlannedEndDate = DateOnly.FromDateTime(DateTime.UtcNow).AddMonths(12),
                Status = ProposalStatus.Submitted, SubmittedAt = DateTime.UtcNow
            };
            db.Proposals.Add(proposal);
            await db.SaveChangesAsync();

            db.SemanticSearchVectors.Add(new SemanticSearchVector
            {
                EntityType = VectorEntityType,
                EntityId = proposal.Id.ToString(),
                ContentHash = "h-" + proposal.Id,
                ContentSnapshot = snapshot,
                ModelUsed = "test-embedding-model"
            });
            return proposal;
        }

        var a = await Seed("DT-A", "Cảnh báo sớm nguy cơ thôi học", new string('x', 400));
        var b = await Seed("DT-B", "Phân tích kết quả học tập", "Ngắn gọn.");
        var c = await Seed("DT-C", "Hệ thống tưới tiêu thông minh", "Nông nghiệp.");
        await db.SaveChangesAsync();

        void SetVector(Guid proposalId, float[] vec)
        {
            var row = db.SemanticSearchVectors.Single(v => v.EntityId == proposalId.ToString());
            row.Embedding = VectorMath.Serialize(vec);
            row.Dimensions = vec.Length;
        }

        SetVector(a.Id, new float[] { 1f, 0f });
        SetVector(b.Id, new float[] { 0.87f, 0.5f });   // ~30° so với A
        SetVector(c.Id, new float[] { 0f, 1f });        // vuông góc với A
        await db.SaveChangesAsync();

        return (db, pi.Id, a, b, c);
    }

    private static SemanticSearchRequest Q(string text, int? topK = null) =>
        new() { Query = text, TopK = topK };

    // ── Xếp hạng ──────────────────────────────────────────────────────────

    [Fact]
    public async Task Search_XepTheoDoGanNghia_GanNhatDungDau()
    {
        var (db, userId, a, b, c) = await SeedAsync();
        // Câu hỏi trùng hướng với A.
        var svc = TestServices.SemanticSearch(db, _ => new float[] { 1f, 0f });

        var res = await svc.SearchAsync(Q("nguy cơ bỏ học"), userId, new[] { "Faculty" });

        Assert.Equal(3, res.IndexedCount);
        Assert.Equal(a.Id.ToString(), res.Hits[0].Id);
        Assert.Equal(b.Id.ToString(), res.Hits[1].Id);

        // C vuông góc → cosine 0 → bị loại hẳn, không phải xuống cuối với điểm 0.
        // Hiện một kết quả "0% liên quan" chỉ làm người dùng tưởng hệ thống tìm bừa.
        Assert.DoesNotContain(res.Hits, h => h.Id == c.Id.ToString());

        // Thang 0–100 để giao diện tô màu, giảm dần theo thứ hạng.
        Assert.True(res.Hits[0].Relevance > res.Hits[1].Relevance);
        Assert.InRange(res.Hits[0].Relevance, 99, 100);
    }

    [Fact]
    public async Task Search_GhepDuocTenDeTai_ChuNhiem_Nam()
    {
        var (db, userId, a, _, _) = await SeedAsync();
        var svc = TestServices.SemanticSearch(db, _ => new float[] { 1f, 0f });

        var res = await svc.SearchAsync(Q("thôi học"), userId, new[] { "Faculty" });
        var top = res.Hits[0];

        Assert.Equal("Cảnh báo sớm nguy cơ thôi học", top.Title);
        Assert.Equal("DT-A", top.ProjectCode);
        Assert.Equal("Chủ nhiệm", top.PiName);
        Assert.Equal(2026, top.CycleYear);
        Assert.Equal("proposal", top.Type);
        Assert.Equal(a.Id.ToString(), top.Id);
    }

    [Fact]
    public async Task Search_DoanTrich_CatNgan_KhongDutGiuaChu()
    {
        var (db, userId, _, _, _) = await SeedAsync();
        var svc = TestServices.SemanticSearch(db, _ => new float[] { 1f, 0f });

        var res = await svc.SearchAsync(Q("thôi học"), userId, new[] { "Faculty" });

        // Snapshot của A dài 400 ký tự → phải bị cắt và có dấu … ở cuối.
        Assert.True(res.Hits[0].Snippet.Length <= SemanticSearchDefaults.SnippetLength + 1);
        Assert.EndsWith("…", res.Hits[0].Snippet);
    }

    [Fact]
    public async Task Search_DoanTrich_BoPhanDauTrungTieuDe()
    {
        var (db, userId, a, _, _) = await SeedAsync();

        // ContentSnapshot thật do EmbeddingWorker ghép là "tiêu đề + tóm tắt + mục tiêu", nên
        // đoạn trích luôn mở đầu bằng đúng tiêu đề. Để nguyên thì trên màn hình tiêu đề hiện hai
        // lần liền nhau — đã thấy tận mắt khi chạy thử trên trình duyệt 22/09.
        var row = db.SemanticSearchVectors.Single(v => v.EntityId == a.Id.ToString());
        row.ContentSnapshot = "Cảnh báo sớm nguy cơ thôi học Đề tài xây dựng mô hình dự báo sớm "
                            + "dựa trên dữ liệu học tập và tương tác của sinh viên qua các học kỳ.";
        await db.SaveChangesAsync();

        var svc = TestServices.SemanticSearch(db, _ => new float[] { 1f, 0f });
        var res = await svc.SearchAsync(Q("thôi học"), userId, new[] { "Faculty" });

        Assert.StartsWith("Đề tài xây dựng", res.Hits[0].Snippet);
        Assert.DoesNotContain("Cảnh báo sớm nguy cơ thôi học", res.Hits[0].Snippet);
    }

    [Fact]
    public async Task Search_NoiDungChiCoTieuDe_VanHienTieuDe_KhongDeTrong()
    {
        var (db, userId, a, _, _) = await SeedAsync();

        // Phần còn lại sau khi bỏ tiêu đề quá ngắn → thà hiện lại tiêu đề còn hơn một dòng trống.
        var row = db.SemanticSearchVectors.Single(v => v.EntityId == a.Id.ToString());
        row.ContentSnapshot = "Cảnh báo sớm nguy cơ thôi học — ngắn.";
        await db.SaveChangesAsync();

        var svc = TestServices.SemanticSearch(db, _ => new float[] { 1f, 0f });
        var res = await svc.SearchAsync(Q("thôi học"), userId, new[] { "Faculty" });

        Assert.False(string.IsNullOrWhiteSpace(res.Hits[0].Snippet));
        Assert.Contains("Cảnh báo sớm", res.Hits[0].Snippet);
    }

    [Fact]
    public async Task Search_TopK_GioiHanSoKetQua()
    {
        var (db, userId, a, _, _) = await SeedAsync();
        var svc = TestServices.SemanticSearch(db, _ => new float[] { 1f, 0f });

        var res = await svc.SearchAsync(Q("thôi học", topK: 1), userId, new[] { "Faculty" });

        Assert.Single(res.Hits);
        Assert.Equal(a.Id.ToString(), res.Hits[0].Id);
    }

    // ── Đầu vào hỏng ──────────────────────────────────────────────────────

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public async Task Search_CauHoiRong_Nem400(string query)
    {
        var (db, userId, _, _, _) = await SeedAsync();
        var svc = TestServices.SemanticSearch(db, _ => new float[] { 1f, 0f });

        var ex = await Assert.ThrowsAsync<AppException>(
            () => svc.SearchAsync(Q(query), userId, new[] { "Faculty" }));

        Assert.Equal(400, ex.StatusCode);
        Assert.Equal("SEARCH_QUERY_EMPTY", ex.Code);
    }

    [Fact]
    public async Task Search_ModelNhungLoi_Nem503_KhongPhai500()
    {
        var (db, userId, _, _, _) = await SeedAsync();
        // Vector rỗng = mô hình trả về thứ không dùng được.
        var svc = TestServices.SemanticSearch(db, _ => Array.Empty<float>());

        var ex = await Assert.ThrowsAsync<AppException>(
            () => svc.SearchAsync(Q("gì đó"), userId, new[] { "Faculty" }));

        // 503 chứ không phải 500: đây là dịch vụ ngoài tạm hỏng, người dùng thử lại được.
        Assert.Equal(503, ex.StatusCode);
    }

    // ── Kho rỗng ──────────────────────────────────────────────────────────

    [Fact]
    public async Task Search_KhoVectorRong_TraIndexedCount0_VaKhongGoiModel()
    {
        var db = TestDbContextFactory.Create("search-" + Guid.NewGuid());
        // Không truyền hàm nhúng → TestGeminiService.EmbedAsync ném lỗi nếu bị gọi.
        // Test này xanh tức là nhánh "kho rỗng" thoát TRƯỚC khi gọi model — không tốn quota.
        var svc = TestServices.SemanticSearch(db);

        var res = await svc.SearchAsync(Q("bất kỳ"), Guid.NewGuid(), new[] { "Faculty" });

        Assert.Equal(0, res.IndexedCount);
        Assert.Empty(res.Hits);
    }

    [Fact]
    public async Task Search_VectorLechSoChieu_KhongNem_ChiBoQua()
    {
        var (db, userId, a, _, _) = await SeedAsync();

        // Câu hỏi 3 chiều trong khi kho là 2 chiều — đúng tình huống đổi model mà quên vector hoá
        // lại. Phải trả về rỗng một cách yên lặng, không được ném lỗi vào mặt người dùng.
        var svc = TestServices.SemanticSearch(db, _ => new float[] { 1f, 0f, 0f });

        var res = await svc.SearchAsync(Q("thôi học"), userId, new[] { "Faculty" });

        Assert.Equal(3, res.IndexedCount);
        Assert.Empty(res.Hits);
        Assert.NotNull(a);
    }
}
