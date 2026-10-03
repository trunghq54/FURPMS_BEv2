using FURPMS.Application.Constants;
using FURPMS.Application.DTOs.Cycles;
using FURPMS.Domain.Entities.Contracts;
using FURPMS.Domain.Entities.Cycles;
using FURPMS.Domain.Entities.MasterData;
using FURPMS.Infrastructure.Repositories;
using FURPMS.Infrastructure.Services;
using FURPMS.Tests.Helpers;
using Microsoft.EntityFrameworkCore;

namespace FURPMS.Tests.Cycles;

// 03/10: hệ thống chỉ có 2 loại đề tài cố định (BASIC / APPLIED); phân loại theo mã. Đổi loại của đợt thì đề
// tài đã nộp đổi theo.
public class ResearchKindTests
{
    private static CycleService MakeSvc(FURPMS.Infrastructure.Data.FURPMSDbContext db) =>
        new(new CycleRepository(db), new MasterDataRepository(db), new ProposalRepository(db), new ReviewRepository(db),
            TestNotifier.Create(db), new DeadlineResolver(new CycleRepository(db)));

    [Theory]
    [InlineData("NGHIÊN_CỨU_ỨNG_DỤNG", "Nghiên cứu ứng dụng", true)]
    [InlineData("UD", "Đề tài ứng dụng / triển khai", true)]
    [InlineData("APPLIED", "Applied research", true)]
    [InlineData("NGHIÊN_CỨU_CƠ_BẢN", "Nghiên cứu cơ bản", false)]
    [InlineData("BASIC", "Basic", false)]
    [InlineData("SV", "Đề tài sinh viên", null)]
    [InlineData("X", "Cơ bản định hướng ứng dụng", null)]   // nói cả hai ⇒ không đoán
    public void KindFromName_ReadsVietnameseAndEnglishNames(string code, string name, bool? expected) =>
        Assert.Equal(expected, ResearchKind.KindFromName(code, name));

    [Fact]
    public async Task FixedTypes_KindComesFromCode_OnlyNameAndCapEditable()
    {
        var db = TestDbContextFactory.Create($"test-{Guid.NewGuid()}");
        // Cờ cũ cố tình để SAI — phân loại giờ chỉ theo mã.
        db.ResearchTypes.AddRange(
            new ResearchType { Code = "APPLIED", Name = "Ứng dụng", RequireOrderingUnit = false, MaxBudgetCap = 150, IsActive = true },
            new ResearchType { Code = "BASIC", Name = "Cơ bản", RequireOrderingUnit = true, MaxBudgetCap = 100, IsActive = true });
        await db.SaveChangesAsync();
        var svc = MakeSvc(db);

        var types = (await svc.GetResearchTypesAsync()).ToDictionary(t => t.Code);
        Assert.Equal(ResearchKind.Applied, types["APPLIED"].Kind);
        Assert.Equal(2, types["APPLIED"].ProgressRounds);
        Assert.Equal(4, types["APPLIED"].DisbursementRounds);
        Assert.Equal(ResearchKind.Basic, types["BASIC"].Kind);

        var updated = await svc.UpdateResearchTypeAsync(types["APPLIED"].Id,
            new UpdateResearchTypeRequest { Name = "Đề tài ứng dụng / triển khai", MaxBudgetCap = 200, RequireOrderingUnit = false });
        Assert.Equal("Đề tài ứng dụng / triển khai", updated.Name);
        Assert.Equal(200, updated.MaxBudgetCap);
        Assert.Equal(ResearchKind.Applied, updated.Kind);   // cờ gửi lên bị bỏ qua

        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            svc.CreateResearchTypeAsync(new CreateResearchTypeRequest { Code = "SV", Name = "Sinh viên" }));
        await Assert.ThrowsAsync<InvalidOperationException>(() => svc.DeleteResearchTypeAsync(types["BASIC"].Id));
        await Assert.ThrowsAsync<InvalidOperationException>(() => svc.DeactivateResearchTypeAsync(types["BASIC"].Id));
        await Assert.ThrowsAsync<ArgumentException>(() =>
            svc.UpdateResearchTypeAsync(types["BASIC"].Id, new UpdateResearchTypeRequest { MaxBudgetCap = 0 }));
    }

    private static async Task<(ResearchCycle cycle, FURPMS.Domain.Entities.Projects.Project project, ResearchType applied)>
        SeedCycleWithProjectAsync(FURPMS.Infrastructure.Data.FURPMSDbContext db)
    {
        var basic = new ResearchType { Code = "BASIC", Name = "Cơ bản", RequireOrderingUnit = false, IsActive = true };
        var applied = new ResearchType { Code = "APPLIED", Name = "Ứng dụng", RequireOrderingUnit = true, IsActive = true };
        db.ResearchTypes.AddRange(basic, applied);
        db.ResearchTracks.Add(new ResearchTrack { Id = 1, Code = "IT", Name = "CNTT", IsActive = true });
        await db.SaveChangesAsync();

        var cycle = new ResearchCycle
        {
            CycleYear = 2026, SemesterCode = "UD26", ResearchTypeId = basic.Id,
            SubmissionOpenDate = new DateOnly(2026, 1, 1), SubmissionDeadline = new DateOnly(2026, 3, 1),
            ReviewDeadline = new DateOnly(2026, 6, 1), Status = "OPEN", CreatedBy = Guid.NewGuid()
        };
        db.ResearchCycles.Add(cycle);
        await db.SaveChangesAsync();
        var link = new CycleTrack { CycleId = cycle.Id, TrackId = 1 };
        db.CycleTracks.Add(link);
        await db.SaveChangesAsync();

        var project = new FURPMS.Domain.Entities.Projects.Project
        {
            Id = Guid.NewGuid(), CycleTrackId = link.Id, OrderId = 1, PiUserId = Guid.NewGuid(), HostingUnitId = 1,
            ResearchTypeId = basic.Id, TitleVi = "Đề tài", Status = "PROPOSED",
            PlannedStartDate = new DateOnly(2026, 1, 1), PlannedEndDate = new DateOnly(2026, 12, 31)
        };
        db.Projects.Add(project);
        await db.SaveChangesAsync();
        return (cycle, project, applied);
    }

    private static CreateCycleRequest Change(ResearchCycle c, int typeId) => new()
    {
        Name = c.SemesterCode!, AcademicYear = "2026", ResearchTypeId = typeId,
        SubmissionStartDate = "2026-01-01", SubmissionDeadline = "2026-03-01"
    };

    [Fact]
    public async Task ChangingCycleType_AlsoChangesProjectsAlreadySubmitted()
    {
        var db = TestDbContextFactory.Create($"test-{Guid.NewGuid()}");
        var (cycle, project, applied) = await SeedCycleWithProjectAsync(db);

        var dto = await MakeSvc(db).UpdateCycleAsync(cycle.Id, Change(cycle, applied.Id));

        Assert.True(dto.IsApplied);
        Assert.Equal(applied.Id, (await db.Projects.AsNoTracking().SingleAsync(p => p.Id == project.Id)).ResearchTypeId);
    }

    [Fact]
    public async Task ChangingCycleType_BlockedOnceAProjectHasAContract()
    {
        var db = TestDbContextFactory.Create($"test-{Guid.NewGuid()}");
        var (cycle, project, applied) = await SeedCycleWithProjectAsync(db);
        db.Contracts.Add(new Contract
        {
            ProjectId = project.Id, ContractNumber = "HD-1", TotalAmount = 1m,
            StartDate = new DateOnly(2026, 4, 1), EndDate = new DateOnly(2026, 12, 31), OriginalEndDate = new DateOnly(2026, 12, 31),
            Status = "ACTIVE", CreatedBy = Guid.NewGuid()
        });
        await db.SaveChangesAsync();

        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            MakeSvc(db).UpdateCycleAsync(cycle.Id, Change(cycle, applied.Id)));
        Assert.NotEqual(applied.Id, (await db.Projects.AsNoTracking().SingleAsync(p => p.Id == project.Id)).ResearchTypeId);
    }
}
