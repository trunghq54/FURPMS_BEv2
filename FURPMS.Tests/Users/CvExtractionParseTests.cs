using FURPMS.Infrastructure.Services;

namespace FURPMS.Tests.Users;

/// <summary>
/// AI đọc CV (03/10): AI có thể trả mã lạ / định dạng lệch — phần đọc kết quả phải lọc chặt để form
/// chỉ nhận giá trị hợp lệ (ô chọn học vị, giới tính) và chỉ đề xuất công trình đúng mục BM02.
/// </summary>
public class CvExtractionParseTests
{
    [Fact]
    public void Parse_KeepsValidFields_DropsInventedCodes()
    {
        var raw = """
        ```json
        {"academicTitle":"TS.","degreeLevel":"phd","gender":"female","dateOfBirth":"1985-03-07",
         "institution":"Trường Đại học FPT","gsPgsYear":"2020",
         "works":[
           {"workType":"PUBLICATION","category":"ISI_SCOPUS","title":"Deep learning for X","venue":"IEEE Access","year":2023},
           {"workType":"PUBLICATION","category":"BOOK_TEXTBOOK","title":"Sai cặp mục"},
           {"workType":"PODCAST","category":"X","title":"Mục bịa"},
           {"workType":"PROJECT","category":"PROJECT_LEAD","title":"Đề tài cấp Bộ","year":1700}
         ]}
        ```
        """;

        var dto = CvExtractionService.Parse(raw);

        Assert.Null(dto.Warning);
        Assert.Equal("TS.", dto.AcademicTitle);
        Assert.Equal("PhD", dto.DegreeLevel);
        Assert.Equal("Female", dto.Gender);
        Assert.Equal("1985-03-07", dto.DateOfBirth);
        Assert.Equal(2020, dto.GsPgsYear);
        Assert.Equal(2, dto.Works.Count);
        Assert.Equal("ISI_SCOPUS", dto.Works[0].Category);
        Assert.Null(dto.Works[1].Year);   // năm vô lý bị bỏ
    }

    [Fact]
    public void Parse_GarbageOrUnknownEnums_GivesWarningOrNulls()
    {
        Assert.NotNull(CvExtractionService.Parse("không phải json").Warning);
        var dto = CvExtractionService.Parse("{\"degreeLevel\":\"Kỹ sư\",\"gender\":\"Nam\",\"dateOfBirth\":\"không rõ\"}");
        Assert.Null(dto.DegreeLevel);
        Assert.Null(dto.Gender);
        Assert.Null(dto.DateOfBirth);
    }
}
