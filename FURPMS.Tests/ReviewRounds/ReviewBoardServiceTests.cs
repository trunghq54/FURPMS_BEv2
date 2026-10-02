using FURPMS.Application.Constants;
using FURPMS.Application.DTOs.ReviewRounds;
using FURPMS.Domain.Entities.Cycles;
using FURPMS.Domain.Entities.MasterData;
using FURPMS.Domain.Entities.Proposals;
using FURPMS.Domain.Entities.Progress;
using FURPMS.Domain.Entities.Review;
using FURPMS.Domain.Entities.Users;
using FURPMS.Infrastructure.Repositories;
using FURPMS.Infrastructure.Services;
using FURPMS.Tests.Helpers;
using FURPMS.Tests.Reminders; // FakeClock
using Microsoft.EntityFrameworkCore;

namespace FURPMS.Tests.ReviewRounds;

public class ReviewBoardServiceTests
{
    private static User MakeUser(Guid? id = null) => new()
    {
        Id = id ?? Guid.NewGuid(),
        Email = $"user-{Guid.NewGuid()}@test.com",
        FullName = "Test User",
        Status = "ACTIVE"
    };

    private static (FURPMS.Domain.Entities.Projects.Project project, Proposal proposal) MakeProjectWithProposal(
        Guid piUserId, string status = "SUBMITTED", int cycleTrackId = 1)
    {
        var project = new FURPMS.Domain.Entities.Projects.Project
        {
            Id = Guid.NewGuid(),
            CycleTrackId = cycleTrackId,
            OrderId = 1,
            PiUserId = piUserId,
            HostingUnitId = 1,
            ResearchTypeId = 1,
            TitleVi = "Test Project",
            Status = "UNDER_REVIEW",
            PlannedStartDate = DateOnly.FromDateTime(DateTime.UtcNow),
            PlannedEndDate = DateOnly.FromDateTime(DateTime.UtcNow.AddMonths(12))
        };
        var proposal = new Proposal
        {
            Id = Guid.NewGuid(),
            ProjectId = project.Id,
            VersionNo = 1,
            IsCurrent = true,
            TitleVi = "Test Proposal",
            AbstractVi = "Abstract",
            ResearchObjectives = "Objectives",
            DurationMonths = 12,
            PlannedStartDate = DateOnly.FromDateTime(DateTime.UtcNow),
            PlannedEndDate = DateOnly.FromDateTime(DateTime.UtcNow.AddMonths(12)),
            Status = status
        };
        return (project, proposal);
    }

    private static ReviewRound MakeRound(int cycleTrackId = 1, string dimension = "SCIENCE") => new()
    {
        Id = Guid.NewGuid(),
        CycleTrackId = cycleTrackId,
        RoundNumber = 1,
        Sequence = 1,
        Dimension = dimension,
        RoundType = "REVIEW",
        Status = "PENDING"
    };

    private static ReviewBoardService MakeService(FURPMS.Infrastructure.Data.FURPMSDbContext db) =>
        TestServices.ReviewBoards(db);

    // ── Xóa vòng ─────────────────────────────────────────────────────────────

    [Fact]
    public async Task DeleteRound_Empty_Succeeds()
    {
        var db = TestDbContextFactory.Create($"test-{Guid.NewGuid()}");
        var round = MakeRound();
        db.ReviewRounds.Add(round);
        await db.SaveChangesAsync();

        var service = MakeService(db);
        await service.DeleteRoundAsync(round.Id);

        Assert.Null(await db.ReviewRounds.FindAsync(round.Id));
    }

    [Fact]
    public async Task DeleteRound_WithCouncil_Throws()
    {
        var db = TestDbContextFactory.Create($"test-{Guid.NewGuid()}");
        var round = MakeRound();
        db.ReviewRounds.Add(round);
        db.ReviewCouncils.Add(new ReviewCouncil
        {
            Id = Guid.NewGuid(),
            RoundId = round.Id,
            CouncilType = "REVIEW",
            CreatedBy = Guid.NewGuid()
        });
        await db.SaveChangesAsync();

        var service = MakeService(db);
        var ex = await Assert.ThrowsAsync<InvalidOperationException>(() => service.DeleteRoundAsync(round.Id));
        Assert.Contains("hội đồng", ex.Message);
    }

    [Fact]
    public async Task DeleteRound_WithFinalizedProjectRound_Throws()
    {
        var db = TestDbContextFactory.Create($"test-{Guid.NewGuid()}");
        var round = MakeRound();
        db.ReviewRounds.Add(round);
        db.ProjectRounds.Add(new ProjectRound { ProjectId = Guid.NewGuid(), RoundId = round.Id, Status = "PASSED", FinalizedAt = DateTime.UtcNow });
        await db.SaveChangesAsync();

        var service = MakeService(db);
        await Assert.ThrowsAsync<InvalidOperationException>(() => service.DeleteRoundAsync(round.Id));
    }

    // ── Tạo hội đồng trọn gói ────────────────────────────────────────────────

    [Fact]
    public async Task CreateCouncilPackage_Valid_CreatesCouncilWithAssignmentsAndMembers()
    {
        var db = TestDbContextFactory.Create($"test-{Guid.NewGuid()}");
        var pi = MakeUser();
        var (project, proposal) = MakeProjectWithProposal(pi.Id);
        var chair = MakeUser();
        var secretary = MakeUser();
        db.Users.AddRange(pi, chair, secretary);
        db.Projects.Add(project);
        db.Proposals.Add(proposal);

        var round = MakeRound();
        db.ReviewRounds.Add(round);
        db.ProjectRounds.Add(new ProjectRound { ProjectId = project.Id, RoundId = round.Id, Status = "PENDING" });
        await db.SaveChangesAsync();

        var service = MakeService(db);
        var result = await service.CreateCouncilPackageAsync(round.Id, new CreateCouncilPackageRequest
        {
            ProjectIds = new List<Guid> { project.Id },
            Members = new List<CouncilPackageMemberRequest>
            {
                new() { UserId = chair.Id, MemberRole = "Chair" },
                new() { UserId = secretary.Id, MemberRole = "Secretary" }
            }
        }, Guid.NewGuid());

        Assert.Equal(2, result.Members.Count);
        Assert.Equal(project.Id, Assert.Single(result.ProjectIds));
        Assert.Equal(1, await db.CouncilProjectAssignments.CountAsync(a => a.CouncilId == result.Id));
    }

    [Fact]
    public async Task CreateCouncilPackage_MissingSecretary_Throws()
    {
        var db = TestDbContextFactory.Create($"test-{Guid.NewGuid()}");
        var pi = MakeUser();
        var (project, proposal) = MakeProjectWithProposal(pi.Id);
        var chair = MakeUser();
        db.Users.AddRange(pi, chair);
        db.Projects.Add(project);
        db.Proposals.Add(proposal);

        var round = MakeRound();
        db.ReviewRounds.Add(round);
        db.ProjectRounds.Add(new ProjectRound { ProjectId = project.Id, RoundId = round.Id, Status = "PENDING" });
        await db.SaveChangesAsync();

        var service = MakeService(db);
        var ex = await Assert.ThrowsAsync<ArgumentException>(() => service.CreateCouncilPackageAsync(round.Id, new CreateCouncilPackageRequest
        {
            ProjectIds = new List<Guid> { project.Id },
            Members = new List<CouncilPackageMemberRequest> { new() { UserId = chair.Id, MemberRole = "Chair" } }
        }, Guid.NewGuid()));
        Assert.Contains("Thư ký", ex.Message);
        Assert.Equal(0, await db.ReviewCouncils.CountAsync());
    }

    [Fact]
    public async Task CreateCouncilPackage_SamePersonMultipleRoles_Throws()
    {
        var db = TestDbContextFactory.Create($"test-{Guid.NewGuid()}");
        var pi = MakeUser();
        var (project, proposal) = MakeProjectWithProposal(pi.Id);
        var one = MakeUser();
        db.Users.AddRange(pi, one);
        db.Projects.Add(project);
        db.Proposals.Add(proposal);

        var round = MakeRound();
        db.ReviewRounds.Add(round);
        db.ProjectRounds.Add(new ProjectRound { ProjectId = project.Id, RoundId = round.Id, Status = "PENDING" });
        await db.SaveChangesAsync();

        var service = MakeService(db);
        // 1 người vừa Chủ tịch vừa Thư ký → phải bị chặn (mỗi người chỉ 1 vị trí/hội đồng)
        var ex = await Assert.ThrowsAsync<ArgumentException>(() => service.CreateCouncilPackageAsync(round.Id, new CreateCouncilPackageRequest
        {
            ProjectIds = new List<Guid> { project.Id },
            Members = new List<CouncilPackageMemberRequest>
            {
                new() { UserId = one.Id, MemberRole = "Chair" },
                new() { UserId = one.Id, MemberRole = "Secretary" }
            }
        }, Guid.NewGuid()));
        Assert.Contains("1 vị trí", ex.Message);
        Assert.Equal(0, await db.ReviewCouncils.CountAsync());
    }

    [Fact]
    public async Task CreateCouncilPackage_PiConflictOfInterest_Throws_And_CreatesNothing()
    {
        var db = TestDbContextFactory.Create($"test-{Guid.NewGuid()}");
        var pi = MakeUser();
        var (project, proposal) = MakeProjectWithProposal(pi.Id);
        var chair = MakeUser();
        var secretary = MakeUser();
        db.Users.AddRange(pi, chair, secretary);
        db.Projects.Add(project);
        db.Proposals.Add(proposal);

        var round = MakeRound();
        db.ReviewRounds.Add(round);
        db.ProjectRounds.Add(new ProjectRound { ProjectId = project.Id, RoundId = round.Id, Status = "PENDING" });
        await db.SaveChangesAsync();

        // Hội đồng đủ Chair+Secretary nhưng thêm PI làm ủy viên → COI phải chặn, KHÔNG tạo nửa vời.
        var service = MakeService(db);
        await Assert.ThrowsAsync<ArgumentException>(() => service.CreateCouncilPackageAsync(round.Id, new CreateCouncilPackageRequest
        {
            ProjectIds = new List<Guid> { project.Id },
            Members = new List<CouncilPackageMemberRequest>
            {
                new() { UserId = chair.Id, MemberRole = "Chair" },
                new() { UserId = secretary.Id, MemberRole = "Secretary" },
                new() { UserId = pi.Id, MemberRole = "Member" }
            }
        }, Guid.NewGuid()));

        Assert.Equal(0, await db.ReviewCouncils.CountAsync());
    }

    [Fact]
    public async Task CreateCouncilPackage_ProjectNotJoinedRound_Throws()
    {
        var db = TestDbContextFactory.Create($"test-{Guid.NewGuid()}");
        var pi = MakeUser();
        var (project, proposal) = MakeProjectWithProposal(pi.Id);
        var chair = MakeUser();
        var secretary = MakeUser();
        db.Users.AddRange(pi, chair, secretary);
        db.Projects.Add(project);
        db.Proposals.Add(proposal);

        var round = MakeRound();
        db.ReviewRounds.Add(round); // KHÔNG add ProjectRound cho project này
        await db.SaveChangesAsync();

        var service = MakeService(db);
        await Assert.ThrowsAsync<ArgumentException>(() => service.CreateCouncilPackageAsync(round.Id, new CreateCouncilPackageRequest
        {
            ProjectIds = new List<Guid> { project.Id },
            Members = new List<CouncilPackageMemberRequest>
            {
                new() { UserId = chair.Id, MemberRole = "Chair" },
                new() { UserId = secretary.Id, MemberRole = "Secretary" }
            }
        }, Guid.NewGuid()));
    }

    // ── Chuyên môn (QĐ543 Điều 8.2) — đường "tạo cả gói cùng lúc" ────────────
    //
    // Trước 26/08, đường add-từng-người (CouncilService.AddMemberAsync, xem CouncilExpertiseTests)
    // đã kiểm chuyên môn; đường "tạo cả gói cùng lúc" — đúng nút Staff thực sự bấm khi bắt đầu từ
    // màn "Hội đồng & Chấm" — hoàn toàn bỏ qua, phát hiện khi bấm thử trực tiếp trên trình duyệt.

    private const int TrackAi = 21;
    private const int TrackIt = 22;

    private static async Task<(FURPMS.Infrastructure.Data.FURPMSDbContext db, FURPMS.Domain.Entities.Projects.Project project, User pi)>
        SeedTrackedProjectAsync(FURPMS.Infrastructure.Data.FURPMSDbContext db)
    {
        db.ResearchTracks.AddRange(
            new ResearchTrack { Id = TrackAi, Code = "AI", Name = "Trí tuệ nhân tạo", IsActive = true },
            new ResearchTrack { Id = TrackIt, Code = "IT", Name = "Công nghệ thông tin", IsActive = true });
        var cycleTrack = new CycleTrack { Id = 900, CycleId = 1, TrackId = TrackAi };
        db.CycleTracks.Add(cycleTrack);
        var pi = MakeUser();
        db.Users.Add(pi);
        var (project, proposal) = MakeProjectWithProposal(pi.Id, cycleTrackId: cycleTrack.Id);
        db.Projects.Add(project);
        db.Proposals.Add(proposal);
        await db.SaveChangesAsync();
        return (db, project, pi);
    }

    private static User MakeExpert(FURPMS.Infrastructure.Data.FURPMSDbContext db, string name, params int[] trackIds)
    {
        var u = MakeUser();
        u.FullName = name;
        db.Users.Add(u);
        db.SaveChanges();
        foreach (var t in trackIds)
            db.UserResearchTracks.Add(new UserResearchTrack { UserId = u.Id, TrackId = t });
        db.SaveChanges();
        return u;
    }

    [Fact]
    public async Task CreateCouncilPackage_MemberNgoaiLinhVuc_KhongLyDo_Throws_KhongTaoNuaVoi()
    {
        var db = TestDbContextFactory.Create($"test-{Guid.NewGuid()}");
        var (_, project, _) = await SeedTrackedProjectAsync(db);
        var chair = MakeExpert(db, "Chủ tịch đúng ngành", TrackAi);
        var secretary = MakeExpert(db, "Thư ký đúng ngành", TrackAi);
        var outsider = MakeExpert(db, "Ủy viên khác ngành", TrackIt);

        var round = MakeRound(cycleTrackId: project.CycleTrackId);
        db.ReviewRounds.Add(round);
        db.ProjectRounds.Add(new ProjectRound { ProjectId = project.Id, RoundId = round.Id, Status = "PENDING" });
        await db.SaveChangesAsync();

        var service = MakeService(db);
        var ex = await Assert.ThrowsAsync<ArgumentException>(() => service.CreateCouncilPackageAsync(round.Id, new CreateCouncilPackageRequest
        {
            ProjectIds = new List<Guid> { project.Id },
            Members = new List<CouncilPackageMemberRequest>
            {
                new() { UserId = chair.Id, MemberRole = "Chair" },
                new() { UserId = secretary.Id, MemberRole = "Secretary" },
                new() { UserId = outsider.Id, MemberRole = "Member" }
            }
        }, Guid.NewGuid()));

        Assert.Contains("Trí tuệ nhân tạo", ex.Message);
        Assert.Equal(0, await db.ReviewCouncils.CountAsync());
    }

    [Fact]
    public async Task CreateCouncilPackage_MemberNgoaiLinhVuc_CoLyDo_TaoDuoc_VaGhiVaoSoQuyetDinh()
    {
        var db = TestDbContextFactory.Create($"test-{Guid.NewGuid()}");
        var (_, project, _) = await SeedTrackedProjectAsync(db);
        var chair = MakeExpert(db, "Chủ tịch đúng ngành", TrackAi);
        var secretary = MakeExpert(db, "Thư ký đúng ngành", TrackAi);
        var outsider = MakeExpert(db, "Ủy viên khác ngành", TrackIt);

        var round = MakeRound(cycleTrackId: project.CycleTrackId);
        db.ReviewRounds.Add(round);
        db.ProjectRounds.Add(new ProjectRound { ProjectId = project.Id, RoundId = round.Id, Status = "PENDING" });
        await db.SaveChangesAsync();

        const string lyDo = "Đề tài có cấu phần hệ thống lớn, cần chuyên gia kiến trúc phần mềm.";
        var service = MakeService(db);
        var result = await service.CreateCouncilPackageAsync(round.Id, new CreateCouncilPackageRequest
        {
            ProjectIds = new List<Guid> { project.Id },
            Members = new List<CouncilPackageMemberRequest>
            {
                new() { UserId = chair.Id, MemberRole = "Chair" },
                new() { UserId = secretary.Id, MemberRole = "Secretary" },
                new()
                {
                    UserId = outsider.Id, MemberRole = "Member",
                    AcceptWithoutExpertise = true, ExpertiseNote = lyDo
                }
            }
        }, Guid.NewGuid());

        Assert.Equal(3, result.Members.Count);

        var row = await db.ProjectDecisions.FirstOrDefaultAsync(
            d => d.ProjectId == project.Id && d.DecisionType == DecisionTypes.ExpertiseOverride);
        Assert.NotNull(row);
        Assert.Equal(lyDo, row!.Reason);
    }

    // ── Thêm / bỏ đề tài khỏi vòng ───────────────────────────────────────────

    [Fact]
    public async Task AddProjectToRound_DifferentTrack_Throws()
    {
        var db = TestDbContextFactory.Create($"test-{Guid.NewGuid()}");
        var pi = MakeUser();
        var (project, proposal) = MakeProjectWithProposal(pi.Id, cycleTrackId: 2);
        db.Users.Add(pi);
        db.Projects.Add(project);
        db.Proposals.Add(proposal);

        var round = MakeRound(cycleTrackId: 1);
        db.ReviewRounds.Add(round);
        await db.SaveChangesAsync();

        var service = MakeService(db);
        await Assert.ThrowsAsync<ArgumentException>(() => service.AddProjectToRoundAsync(round.Id, project.Id));
    }

    [Fact]
    public async Task RemoveProjectFromRound_Pending_Succeeds()
    {
        var db = TestDbContextFactory.Create($"test-{Guid.NewGuid()}");
        var round = MakeRound();
        var projectId = Guid.NewGuid();
        db.ReviewRounds.Add(round);
        db.ProjectRounds.Add(new ProjectRound { ProjectId = projectId, RoundId = round.Id, Status = "PENDING" });
        await db.SaveChangesAsync();

        var service = MakeService(db);
        await service.RemoveProjectFromRoundAsync(round.Id, projectId);

        Assert.False(await db.ProjectRounds.AnyAsync(pr => pr.RoundId == round.Id && pr.ProjectId == projectId));
    }

    [Fact]
    public async Task RemoveProjectFromRound_AlreadyFinalized_Throws()
    {
        var db = TestDbContextFactory.Create($"test-{Guid.NewGuid()}");
        var round = MakeRound();
        var projectId = Guid.NewGuid();
        db.ReviewRounds.Add(round);
        db.ProjectRounds.Add(new ProjectRound { ProjectId = projectId, RoundId = round.Id, Status = "PASSED", FinalizedAt = DateTime.UtcNow });
        await db.SaveChangesAsync();

        var service = MakeService(db);
        await Assert.ThrowsAsync<InvalidOperationException>(() => service.RemoveProjectFromRoundAsync(round.Id, projectId));
    }

    [Fact]
    public async Task AddProjectToAcceptance_UnfinishedReviewRound_Blocked()
    {
        var db = TestDbContextFactory.Create($"test-{Guid.NewGuid()}");
        var pi = MakeUser();
        var (project, proposal) = MakeProjectWithProposal(pi.Id);
        var reviewRound = MakeRound();
        reviewRound.Status = "OPEN";
        var acceptanceRound = new ReviewRound
        {
            Id = Guid.NewGuid(), CycleTrackId = 1, RoundNumber = 2, Sequence = 2,
            Dimension = "SCIENCE", RoundType = "ACCEPTANCE", Status = "PENDING"
        };
        db.Users.Add(pi);
        db.Projects.Add(project);
        db.Proposals.Add(proposal);
        db.ReviewRounds.AddRange(reviewRound, acceptanceRound);
        db.ProjectRounds.Add(new ProjectRound
        {
            ProjectId = project.Id, RoundId = reviewRound.Id, Status = "OPEN"
        });
        await db.SaveChangesAsync();

        var service = MakeService(db);
        var error = await Assert.ThrowsAsync<InvalidOperationException>(
            () => service.AddProjectToRoundAsync(acceptanceRound.Id, project.Id));

        Assert.Contains("chưa hoàn tất", error.Message);
        Assert.False(await db.ProjectRounds.AnyAsync(x => x.RoundId == acceptanceRound.Id));
    }

    [Fact]
    public async Task AddProjectToAcceptance_FinalReportNotApproved_Blocked()
    {
        var db = TestDbContextFactory.Create($"test-{Guid.NewGuid()}");
        var pi = MakeUser();
        var (project, proposal) = MakeProjectWithProposal(pi.Id, status: "APPROVED");
        project.Status = "ACCEPTANCE";
        var reviewRound = MakeRound();
        reviewRound.Status = "PASSED";
        var acceptanceRound = new ReviewRound
        {
            Id = Guid.NewGuid(), CycleTrackId = 1, RoundNumber = 2, Sequence = 2,
            Dimension = "SCIENCE", RoundType = "ACCEPTANCE", Status = "PENDING"
        };
        db.Users.Add(pi);
        db.Projects.Add(project);
        db.Proposals.Add(proposal);
        db.ReviewRounds.AddRange(reviewRound, acceptanceRound);
        db.ProjectRounds.Add(new ProjectRound
        {
            ProjectId = project.Id, RoundId = reviewRound.Id, Status = "PASSED", FinalizedAt = DateTime.UtcNow
        });
        db.FinalReports.Add(new FinalReport { ProjectId = project.Id, Status = "SUBMITTED" });
        await db.SaveChangesAsync();

        var service = MakeService(db);
        var error = await Assert.ThrowsAsync<InvalidOperationException>(
            () => service.AddProjectToRoundAsync(acceptanceRound.Id, project.Id));

        Assert.Contains("chưa sẵn sàng", error.Message);
    }

    // ── Hạn chấm hiện ra ở màn "Hội đồng & Chấm" (27/08) ─────────────────
    //
    // Trước đó luật hạn chấm (nhóm 2, 25/08) chỉ hiện ở màn "Đề cương" của TỪNG đề tài — người
    // dùng phải mở đúng đề tài rồi đúng vòng mới đặt được hạn, dù đây mới là màn Staff thực sự
    // quản lý vòng chấm của cả một track.

    [Fact]
    public async Task GetBoard_VongDaDatHan_TraHanHieuLuc_VaKhongQuaHan()
    {
        var db = TestDbContextFactory.Create($"test-{Guid.NewGuid()}");
        var clock = new FakeClock { UtcNow = new DateTime(2026, 6, 18, 0, 0, 0, DateTimeKind.Utc) };

        db.ResearchTypes.Add(new ResearchType { Id = 1, Code = "APPLIED", Name = "Ứng dụng", IsActive = true });
        db.ResearchTracks.Add(new ResearchTrack { Id = 1, Code = "IT", Name = "CNTT", IsActive = true });
        var cycle = new ResearchCycle
        {
            CycleYear = 2026, SemesterCode = "SU26", ResearchTypeId = 1,
            SubmissionOpenDate = DateOnly.FromDateTime(clock.UtcNow).AddDays(-30),
            SubmissionDeadline = DateOnly.FromDateTime(clock.UtcNow).AddDays(30),
            Status = "OPEN", CreatedBy = Guid.NewGuid()
        };
        db.ResearchCycles.Add(cycle);
        await db.SaveChangesAsync();
        var track = new CycleTrack { CycleId = cycle.Id, TrackId = 1 };
        db.CycleTracks.Add(track);
        await db.SaveChangesAsync();

        var round = new ReviewRound
        {
            Id = Guid.NewGuid(), CycleTrackId = track.Id, RoundNumber = 1, Sequence = 1,
            Dimension = "SCIENCE", RoundType = "REVIEW", Status = "OPEN",
            ScoringDeadline = DateOnly.FromDateTime(clock.UtcNow).AddDays(10)
        };
        db.ReviewRounds.Add(round);
        await db.SaveChangesAsync();

        var service = TestServices.ReviewBoards(db, clock);
        var board = await service.GetReviewBoardAsync(cycle.CycleYear == 2026 ? cycle.Id : cycle.Id, track.TrackId);

        var boardRound = board.Rounds.Single(r => r.Id == round.Id);
        Assert.Equal("2026-06-28", boardRound.ScoringDeadline);
        Assert.False(boardRound.IsScoringOverdue);
        // Badge cần con số này để hiện "Còn N ngày" — chỉ có ngày trơ thì FE lại phải tự trừ,
        // đúng thứ rule A24 cấm (đã lộ ra khi bấm thử trên trình duyệt: badge chỉ in ngày).
        Assert.Equal(10, boardRound.ScoringDaysLeft);
    }

    [Fact]
    public async Task GetBoard_VongQuaHan_GanCo_ChuKhongTuDong()
    {
        var db = TestDbContextFactory.Create($"test-{Guid.NewGuid()}");
        var clock = new FakeClock { UtcNow = new DateTime(2026, 6, 18, 0, 0, 0, DateTimeKind.Utc) };

        db.ResearchTypes.Add(new ResearchType { Id = 1, Code = "APPLIED", Name = "Ứng dụng", IsActive = true });
        db.ResearchTracks.Add(new ResearchTrack { Id = 1, Code = "IT", Name = "CNTT", IsActive = true });
        var cycle = new ResearchCycle
        {
            CycleYear = 2026, SemesterCode = "SU26", ResearchTypeId = 1,
            SubmissionOpenDate = DateOnly.FromDateTime(clock.UtcNow).AddDays(-30),
            SubmissionDeadline = DateOnly.FromDateTime(clock.UtcNow).AddDays(30),
            Status = "OPEN", CreatedBy = Guid.NewGuid()
        };
        db.ResearchCycles.Add(cycle);
        await db.SaveChangesAsync();
        var track = new CycleTrack { CycleId = cycle.Id, TrackId = 1 };
        db.CycleTracks.Add(track);
        await db.SaveChangesAsync();

        // Vòng CÒN MỞ mà hạn đã qua 5 ngày trước "hôm nay" của đồng hồ giả.
        var round = new ReviewRound
        {
            Id = Guid.NewGuid(), CycleTrackId = track.Id, RoundNumber = 1, Sequence = 1,
            Dimension = "SCIENCE", RoundType = "REVIEW", Status = "OPEN",
            ScoringDeadline = DateOnly.FromDateTime(clock.UtcNow).AddDays(-5)
        };
        db.ReviewRounds.Add(round);
        await db.SaveChangesAsync();

        var service = TestServices.ReviewBoards(db, clock);
        var board = await service.GetReviewBoardAsync(cycle.Id, track.TrackId);

        var boardRound = board.Rounds.Single(r => r.Id == round.Id);
        Assert.True(boardRound.IsScoringOverdue);
        // Chỉ GẮN CỜ — vòng vẫn ở trạng thái OPEN, hệ thống không tự đóng (rule #12).
        Assert.Equal("OPEN", boardRound.Status);
    }

    [Fact]
    public async Task GetBoard_VongChuaDatHan_TraNull_KhongBiaNgay()
    {
        var db = TestDbContextFactory.Create($"test-{Guid.NewGuid()}");

        db.ResearchTypes.Add(new ResearchType { Id = 1, Code = "APPLIED", Name = "Ứng dụng", IsActive = true });
        db.ResearchTracks.Add(new ResearchTrack { Id = 1, Code = "IT", Name = "CNTT", IsActive = true });
        var cycle = new ResearchCycle
        {
            CycleYear = 2026, SemesterCode = "SU26", ResearchTypeId = 1,
            SubmissionOpenDate = DateOnly.FromDateTime(DateTime.UtcNow).AddDays(-30),
            SubmissionDeadline = DateOnly.FromDateTime(DateTime.UtcNow).AddDays(30),
            Status = "OPEN", CreatedBy = Guid.NewGuid()
        };
        db.ResearchCycles.Add(cycle);
        await db.SaveChangesAsync();
        var track = new CycleTrack { CycleId = cycle.Id, TrackId = 1 };
        db.CycleTracks.Add(track);
        await db.SaveChangesAsync();

        var round = new ReviewRound
        {
            Id = Guid.NewGuid(), CycleTrackId = track.Id, RoundNumber = 1, Sequence = 1,
            Dimension = "SCIENCE", RoundType = "REVIEW", Status = "PENDING"
        };
        db.ReviewRounds.Add(round);
        await db.SaveChangesAsync();

        var service = TestServices.ReviewBoards(db);
        var board = await service.GetReviewBoardAsync(cycle.Id, track.TrackId);

        // Vòng tạo trước khi có luật hạn (hoặc chưa ai đặt) phải hiện null, KHÔNG bịa một ngày.
        Assert.Null(board.Rounds.Single(r => r.Id == round.Id).ScoringDeadline);
    }

    [Fact]
    public async Task AddProjectToAcceptance_PassedReviewAndApprovedDossier_Succeeds()
    {
        var db = TestDbContextFactory.Create($"test-{Guid.NewGuid()}");
        var pi = MakeUser();
        var (project, proposal) = MakeProjectWithProposal(pi.Id, status: "APPROVED");
        project.Status = "ACCEPTANCE";
        var reviewRound = MakeRound();
        reviewRound.Status = "PASSED";
        var acceptanceRound = new ReviewRound
        {
            Id = Guid.NewGuid(), CycleTrackId = 1, RoundNumber = 2, Sequence = 2,
            Dimension = "SCIENCE", RoundType = "ACCEPTANCE", Status = "PENDING"
        };
        db.Users.Add(pi);
        db.Projects.Add(project);
        db.Proposals.Add(proposal);
        db.ReviewRounds.AddRange(reviewRound, acceptanceRound);
        db.ProjectRounds.Add(new ProjectRound
        {
            ProjectId = project.Id, RoundId = reviewRound.Id, Status = "PASSED", FinalizedAt = DateTime.UtcNow
        });
        db.FinalReports.Add(new FinalReport { ProjectId = project.Id, Status = "ACCEPTED" });
        await db.SaveChangesAsync();

        var service = MakeService(db);
        await service.AddProjectToRoundAsync(acceptanceRound.Id, project.Id);

        Assert.True(await db.ProjectRounds.AnyAsync(x => x.RoundId == acceptanceRound.Id && x.ProjectId == project.Id));
    }

    // ── Dải mốc thời gian của vòng (01/10) ─────────────────────────────────────
    /// <summary>
    /// Vòng ĐÃ CHỐT vẫn phải trả đủ mốc (mở, hạn chấm, chốt) và hội đồng phải có hạn họp — trước
    /// 01/10 vòng chốt rồi là màn mất sạch mốc, nhìn tưởng không có deadline. Hạn họp chưa ghi thì
    /// suy ra 15 ngày LÀM VIỆC từ ngày lập (QĐ543 Điều 8.3.a), bỏ thứ Bảy/Chủ nhật.
    /// </summary>
    [Fact]
    public async Task GetBoard_VongDaChot_VanCoDuMoc_VaHanHopSuyRaTheoNgayLamViec()
    {
        var db = TestDbContextFactory.Create($"test-{Guid.NewGuid()}");
        var clock = new FakeClock { UtcNow = new DateTime(2026, 7, 20, 0, 0, 0, DateTimeKind.Utc) };

        db.ResearchTypes.Add(new ResearchType { Id = 1, Code = "APPLIED", Name = "Ứng dụng", IsActive = true });
        db.ResearchTracks.Add(new ResearchTrack { Id = 1, Code = "IT", Name = "CNTT", IsActive = true });
        var cycle = new ResearchCycle
        {
            CycleYear = 2026, SemesterCode = "SU26", ResearchTypeId = 1,
            SubmissionOpenDate = new DateOnly(2026, 5, 1), SubmissionDeadline = new DateOnly(2026, 6, 1),
            Status = "OPEN", CreatedBy = Guid.NewGuid()
        };
        db.ResearchCycles.Add(cycle);
        await db.SaveChangesAsync();
        var track = new CycleTrack { CycleId = cycle.Id, TrackId = 1 };
        db.CycleTracks.Add(track);
        await db.SaveChangesAsync();

        var pi = MakeUser();
        pi.FullName = "Nguyễn Văn Chủ Nhiệm";
        db.Users.Add(pi);
        var (project, proposal) = MakeProjectWithProposal(pi.Id, cycleTrackId: track.Id);
        db.Projects.Add(project);
        db.Proposals.Add(proposal);

        var round = new ReviewRound
        {
            Id = Guid.NewGuid(), CycleTrackId = track.Id, RoundNumber = 1, Sequence = 1,
            Dimension = "SCIENCE", RoundType = "REVIEW", Status = ReviewRoundStatus.Passed,
            OpenedAt = new DateTime(2026, 6, 15, 8, 0, 0, DateTimeKind.Utc),
            ClosedAt = new DateTime(2026, 7, 8, 8, 0, 0, DateTimeKind.Utc),
            ScoringDeadline = new DateOnly(2026, 6, 30)
        };
        db.ReviewRounds.Add(round);
        db.ProjectRounds.Add(new ProjectRound { ProjectId = project.Id, RoundId = round.Id, Status = ReviewRoundStatus.Passed });
        // Lập thứ Sáu 19/06/2026, chưa ghi hạn họp ⇒ 15 ngày làm việc = thứ Sáu 10/07/2026.
        db.ReviewCouncils.Add(new ReviewCouncil
        {
            Id = Guid.NewGuid(), RoundId = round.Id, CouncilType = "REVIEW", CreatedBy = Guid.NewGuid(),
            EstablishedAt = new DateOnly(2026, 6, 19)
        });
        await db.SaveChangesAsync();

        var board = await TestServices.ReviewBoards(db, clock).GetReviewBoardAsync(cycle.Id, track.TrackId);
        var dto = Assert.Single(board.Rounds);

        Assert.Equal("2026-06-15", dto.OpenedAt);
        Assert.Equal("2026-07-08", dto.ClosedAt);
        Assert.Equal("2026-06-30", dto.ScoringDeadline);
        Assert.False(dto.IsScoringExtended);

        var council = Assert.Single(dto.Councils);
        Assert.Equal("2026-06-19", council.EstablishedAt);
        Assert.Equal("2026-07-10", council.MeetingDeadline);
        Assert.True(council.IsMeetingDeadlineDerived);

        var row = Assert.Single(dto.Projects);
        Assert.Equal(proposal.Id, row.ProposalId);
        Assert.Equal("Nguyễn Văn Chủ Nhiệm", row.PiName);
    }

    // ── 03/10: Phản biện chỉ ở hội đồng nghiệm thu · lý do khác lĩnh vực được LƯU ──────────

    /// <summary>QĐ543 Điều 8.2: hội đồng xét duyệt không có vai Phản biện (chỉ nghiệm thu, Điều 12.3.b).</summary>
    [Fact]
    public async Task CreateCouncilPackage_XetDuyet_KhongNhanPhanBien()
    {
        var db = TestDbContextFactory.Create($"test-{Guid.NewGuid()}");
        var (_, project, _) = await SeedTrackedProjectAsync(db);
        var chair = MakeExpert(db, "Chủ tịch", TrackAi);
        var secretary = MakeExpert(db, "Thư ký", TrackAi);
        var opp = MakeExpert(db, "Phản biện", TrackAi);
        var round = MakeRound(cycleTrackId: project.CycleTrackId); // REVIEW
        db.ReviewRounds.Add(round);
        await db.SaveChangesAsync();

        var ex = await Assert.ThrowsAsync<ArgumentException>(() => MakeService(db).CreateCouncilPackageAsync(round.Id, new CreateCouncilPackageRequest
        {
            ProjectIds = new List<Guid>(),
            Members = new List<CouncilPackageMemberRequest>
            {
                new() { UserId = chair.Id, MemberRole = "Chair" },
                new() { UserId = secretary.Id, MemberRole = "Secretary" },
                new() { UserId = opp.Id, MemberRole = "Opponent" }
            }
        }, Guid.NewGuid()));
        Assert.Contains("Phản biện", ex.Message);
    }

    /// <summary>
    /// Hội đồng tạo khi CHƯA có đề tài (đúng cách form "Tạo hội đồng" làm) vẫn phải so với lĩnh vực
    /// của PHIÊN, bắt ghi lý do và LƯU lý do. Trước 03/10 bỏ qua hẳn ⇒ lý do Staff gõ bị mất.
    /// </summary>
    [Fact]
    public async Task CreateCouncilPackage_ChuaCoDeTai_NgoaiLinhVuc_LuuLyDo()
    {
        var db = TestDbContextFactory.Create($"test-{Guid.NewGuid()}");
        var (_, project, _) = await SeedTrackedProjectAsync(db);
        var chair = MakeExpert(db, "Chủ tịch", TrackAi);
        var secretary = MakeExpert(db, "Thư ký", TrackAi);
        var outsider = MakeExpert(db, "Khác ngành", TrackIt);
        var round = MakeRound(cycleTrackId: project.CycleTrackId);
        db.ReviewRounds.Add(round);
        await db.SaveChangesAsync();

        CreateCouncilPackageRequest Req(string? note) => new()
        {
            ProjectIds = new List<Guid>(),
            Members = new List<CouncilPackageMemberRequest>
            {
                new() { UserId = chair.Id, MemberRole = "Chair" },
                new() { UserId = secretary.Id, MemberRole = "Secretary" },
                new() { UserId = outsider.Id, MemberRole = "Member", AcceptWithoutExpertise = note != null, ExpertiseNote = note }
            }
        };

        await Assert.ThrowsAsync<ArgumentException>(() => MakeService(db).CreateCouncilPackageAsync(round.Id, Req(null), Guid.NewGuid()));

        var council = await MakeService(db).CreateCouncilPackageAsync(round.Id, Req("Cần chuyên gia kiến trúc hệ thống."), Guid.NewGuid());
        var saved = await db.CouncilMembers.FirstAsync(m => m.CouncilId == council.Id && m.UserId == outsider.Id);
        Assert.Equal("Cần chuyên gia kiến trúc hệ thống.", saved.ExpertiseNote);
        Assert.Null((await db.CouncilMembers.FirstAsync(m => m.CouncilId == council.Id && m.UserId == chair.Id)).ExpertiseNote);
    }

    /// <summary>Gán đề tài sau đó: dùng lại lý do đã lưu, ghi vào sổ quyết định — không bắt gõ lần hai.</summary>
    [Fact]
    public async Task GanDeTai_DungLaiLyDoDaLuu_GhiSoQuyetDinh()
    {
        var db = TestDbContextFactory.Create($"test-{Guid.NewGuid()}");
        var (_, project, _) = await SeedTrackedProjectAsync(db);
        var chair = MakeExpert(db, "Chủ tịch", TrackAi);
        var secretary = MakeExpert(db, "Thư ký", TrackAi);
        var outsider = MakeExpert(db, "Khác ngành", TrackIt);
        var round = MakeRound(cycleTrackId: project.CycleTrackId);
        db.ReviewRounds.Add(round);
        db.ProjectRounds.Add(new ProjectRound { ProjectId = project.Id, RoundId = round.Id, Status = "PENDING" });
        await db.SaveChangesAsync();

        var council = await MakeService(db).CreateCouncilPackageAsync(round.Id, new CreateCouncilPackageRequest
        {
            ProjectIds = new List<Guid>(),
            Members = new List<CouncilPackageMemberRequest>
            {
                new() { UserId = chair.Id, MemberRole = "Chair" },
                new() { UserId = secretary.Id, MemberRole = "Secretary" },
                new() { UserId = outsider.Id, MemberRole = "Member", AcceptWithoutExpertise = true, ExpertiseNote = "Chuyên gia liên ngành." }
            }
        }, Guid.NewGuid());

        // Không truyền lý do lúc gán — phải tự lấy lý do đã lưu.
        await TestServices.Councils(db).AssignProjectToCouncilAsync(council.Id, project.Id);

        Assert.True(await db.ProjectDecisions.AnyAsync(d =>
            d.ProjectId == project.Id && d.DecisionType == DecisionTypes.ExpertiseOverride && d.Reason == "Chuyên gia liên ngành."));
    }

    /// <summary>QĐ543 Điều 12.3.b: hội đồng NGHIỆM THU phải có phản biện (03/10).</summary>
    [Fact]
    public async Task CreateCouncilPackage_NghiemThu_KhongCoPhanBien_Throws()
    {
        var db = TestDbContextFactory.Create($"test-{Guid.NewGuid()}");
        var (_, project, _) = await SeedTrackedProjectAsync(db);
        var chair = MakeExpert(db, "Chủ tịch", TrackAi);
        var secretary = MakeExpert(db, "Thư ký", TrackAi);
        var member = MakeExpert(db, "Ủy viên", TrackAi);
        var round = MakeRound(cycleTrackId: project.CycleTrackId);
        round.RoundType = "ACCEPTANCE";
        db.ReviewRounds.Add(round);
        await db.SaveChangesAsync();

        var ex = await Assert.ThrowsAsync<ArgumentException>(() => MakeService(db).CreateCouncilPackageAsync(round.Id, new CreateCouncilPackageRequest
        {
            Members = new List<CouncilPackageMemberRequest>
            {
                new() { UserId = chair.Id, MemberRole = "Chair" },
                new() { UserId = secretary.Id, MemberRole = "Secretary" },
                new() { UserId = member.Id, MemberRole = "Member" }
            }
        }, Guid.NewGuid()));
        Assert.Contains("phản biện", ex.Message);
    }
}
