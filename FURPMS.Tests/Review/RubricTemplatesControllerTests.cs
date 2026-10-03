using FURPMS.API.Controllers;
using FURPMS.Domain.Entities.Financial;
using FURPMS.Domain.Entities.Review;
using FURPMS.Infrastructure.Data;
using FURPMS.Infrastructure.Repositories;
using FURPMS.Infrastructure.Services;
using FURPMS.Tests.Helpers;
using Microsoft.EntityFrameworkCore;

namespace FURPMS.Tests.Review;

/// <summary>
/// Bộ tiêu chí (03/10): bộ NGHIỆM THU chấm theo mức 1–5 (QĐ543 Biểu mẫu 10) nên không có "Cộng 100"
/// như BM03 — trần mỗi nội dung = 5, tổng tự cộng. Đổi tên bộ được cả khi bộ đã có phiếu chấm.
/// </summary>
public class RubricTemplatesControllerTests
{
    private static RubricTemplatesController MakeController(FURPMSDbContext db) =>
        new(new MasterDataRepository(db), new ReviewRepository(db), new CycleRepository(db), new RubricResolver(db));

    private static async Task<RubricTemplate> CreateAsync(FURPMSDbContext db, string type)
    {
        await MakeController(db).Create(new CreateTemplateRequest { Name = "Bộ thử", TemplateType = type });
        return await db.RubricTemplates.Include(t => t.Criteria).SingleAsync();
    }

    [Fact]
    public async Task Acceptance_set_criteria_are_levels_1_to_5_and_total_follows()
    {
        using var db = TestDbContextFactory.Create(nameof(Acceptance_set_criteria_are_levels_1_to_5_and_total_follows));
        var tpl = await CreateAsync(db, "ACCEPTANCE");
        var controller = MakeController(db);

        // Admin không nhập điểm (MaxScore = 0) — vẫn thêm được, trần = 5.
        await controller.AddCriterion(tpl.Id, new SaveTemplateCriterionRequest { CriterionName = "Tính cấp thiết" });
        await controller.AddCriterion(tpl.Id, new SaveTemplateCriterionRequest { CriterionName = "Đóng góp khoa học", MaxScore = 40 });
        await controller.AddCriterion(tpl.Id, new SaveTemplateCriterionRequest { CriterionName = "Ý nghĩa thực tiễn" });

        var saved = await db.RubricTemplates.Include(t => t.Criteria).AsNoTracking().SingleAsync();
        Assert.All(saved.Criteria, c => Assert.Equal(5m, c.MaxScore));
        Assert.Equal(15m, saved.MaxTotalScore);

        var removed = saved.Criteria.First();
        await controller.DeleteCriterion(tpl.Id, removed.Id);
        saved = await db.RubricTemplates.Include(t => t.Criteria).AsNoTracking().SingleAsync();
        Assert.Equal(10m, saved.MaxTotalScore);
    }

    [Fact]
    public async Task Review_set_still_capped_at_100()
    {
        using var db = TestDbContextFactory.Create(nameof(Review_set_still_capped_at_100));
        var tpl = await CreateAsync(db, "REVIEW");
        var controller = MakeController(db);

        await controller.AddCriterion(tpl.Id, new SaveTemplateCriterionRequest { CriterionName = "Nội dung", MaxScore = 90 });
        await Assert.ThrowsAsync<ArgumentException>(() =>
            controller.AddCriterion(tpl.Id, new SaveTemplateCriterionRequest { CriterionName = "Kinh phí", MaxScore = 20 }));
        Assert.Equal(100m, (await db.RubricTemplates.AsNoTracking().SingleAsync()).MaxTotalScore);
    }

    [Fact]
    public async Task Rename_allowed_after_ballots_but_criteria_stay_locked()
    {
        using var db = TestDbContextFactory.Create(nameof(Rename_allowed_after_ballots_but_criteria_stay_locked));
        var tpl = await CreateAsync(db, "REVIEW");
        db.ProposalReviewScores.Add(new ProposalReviewScore
        {
            CouncilId = Guid.NewGuid(), ProjectId = Guid.NewGuid(), EvaluatorMemberId = Guid.NewGuid(), TemplateId = tpl.Id
        });
        await db.SaveChangesAsync();
        var controller = MakeController(db);

        await controller.Update(tpl.Id, new UpdateTemplateRequest { Name = "Phiếu chấm xét duyệt 2026" });
        Assert.Equal("Phiếu chấm xét duyệt 2026", (await db.RubricTemplates.AsNoTracking().SingleAsync()).Name);

        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            controller.AddCriterion(tpl.Id, new SaveTemplateCriterionRequest { CriterionName = "Mới", MaxScore = 10 }));
        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            controller.Update(tpl.Id, new UpdateTemplateRequest { AppliesBasic = false }));
    }
}
