using DocumentFormat.OpenXml.Packaging;
using FURPMS.Domain.Entities.Users;
using FURPMS.Infrastructure.Repositories;
using FURPMS.Infrastructure.Services;
using FURPMS.Tests.Helpers;

namespace FURPMS.Tests.Users;

/// <summary>Xuất lý lịch khoa học theo QĐ543 Biểu mẫu 02 (03/10).</summary>
public class AcademicCvExportTests
{
    [Fact]
    public async Task ExportBm02_FillsProfileAndWorks_NoNationalId()
    {
        var db = TestDbContextFactory.Create($"test-{Guid.NewGuid()}");
        var user = new User { Id = Guid.NewGuid(), Email = "cv@test.com", FullName = "Nguyễn Văn Mẫu", Status = "ACTIVE", Phone = "0900000000" };
        db.Users.Add(user);
        db.AcademicProfiles.Add(new AcademicProfile
        {
            UserId = user.Id, AcademicTitle = "PGS. TS.", Gender = "Male", DateOfBirth = new DateOnly(1984, 5, 12),
            Institution = "Trường Đại học FPT", GsPgsYear = 2021, NationalId = "001084000000"
        });
        db.AcademicWorks.AddRange(
            new AcademicWork { UserId = user.Id, WorkType = WorkTypes.Publication, Category = "ISI_SCOPUS", Title = "Bài ISI thử", Venue = "IEEE Access", Year = 2023 },
            new AcademicWork { UserId = user.Id, WorkType = WorkTypes.Project, Category = "PROJECT_LEAD", Title = "Đề tài cấp Bộ thử", Status = "ACCEPTED" });
        await db.SaveChangesAsync();

        var svc = new DocumentExportService(new ProposalRepository(db), new MasterDataRepository(db), new ContractRepository(db), new ReviewRepository(db));
        var (content, fileName) = await svc.ExportAcademicCvDocAsync(user.Id);

        using var doc = WordprocessingDocument.Open(new MemoryStream(content), false);
        var text = doc.MainDocumentPart!.Document.Body!.InnerText;
        Assert.StartsWith("BM02_", fileName);
        Assert.Contains("LÝ LỊCH KHOA HỌC", text);
        Assert.Contains("Nguyễn Văn Mẫu", text);
        Assert.Contains("1984", text);
        Assert.Contains("ISI/SCOPUS: 1", text);
        Assert.Contains("Bài ISI thử", text);
        Assert.Contains("Đề tài cấp Bộ thử", text);
        Assert.Contains("Đã nghiệm thu", text);
        // BM02 không có mục căn cước — không được lộ ra file.
        Assert.DoesNotContain("001084000000", text);
    }
}
