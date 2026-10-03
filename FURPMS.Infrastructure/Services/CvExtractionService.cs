using System.Text.Json;
using FURPMS.Application.DTOs.Users;
using FURPMS.Application.Interfaces;
using FURPMS.Domain.Entities.Users;

namespace FURPMS.Infrastructure.Services;

/// <summary>
/// AI đọc CV / lý lịch khoa học (PDF, DOCX) để điền sẵn hồ sơ (03/10 — góp ý "Hồ sơ: thêm AI giúp
/// điền hộ khi upload CV"). Cùng khuôn với trích xuất đề cương: AI chỉ ĐIỀN SẴN, người dùng duyệt rồi
/// tự Lưu; AI lỗi thì trả cảnh báo, không chặn việc nhập tay.
/// </summary>
public class CvExtractionService
{
    private readonly IGeminiService _gemini;

    public CvExtractionService(IGeminiService gemini) => _gemini = gemini;

    private static readonly string Prompt =
        "Bạn là trợ lý đọc lý lịch khoa học / CV của giảng viên Việt Nam. Đọc tài liệu và trả về DUY NHẤT một JSON " +
        "(không giải thích, không markdown) với các khoá: " +
        "academicTitle (học hàm/học vị viết tắt như \"GS.\", \"PGS.\", \"TS.\", \"ThS.\"), " +
        "scientificRank (chức danh khoa học/nghề nghiệp, vd \"Giảng viên chính\"), " +
        "degreeLevel (CHỈ một trong: Bachelor, Master, PhD, Professor — học vị cao nhất), " +
        "specialization (chuyên ngành), specializationAreas (các hướng nghiên cứu, ngăn bằng dấu phẩy), " +
        "dateOfBirth (yyyy-MM-dd), gender (CHỈ một trong: Male, Female, Other), hometown (quê quán), " +
        "nationality, gsPgsYear (số nguyên, năm được công nhận GS/PGS), gsPgsInstitution, " +
        "institution (cơ quan công tác), institutionAddress, " +
        "works (mảng công trình; mỗi phần tử gồm workType, category, title, venue, authors, year). " +
        "workType và category CHỈ được lấy trong các cặp sau: " +
        "PUBLICATION với ISI_SCOPUS | JOURNAL_INTL | JOURNAL_DOMESTIC | CONFERENCE_INTL | CONFERENCE_DOMESTIC; " +
        "BOOK với BOOK_MONOGRAPH | BOOK_TEXTBOOK; PATENT với PATENT_GRANTED; " +
        "APPLICATION với APPLIED_ABROAD | APPLIED_DOMESTIC; PROJECT với PROJECT_LEAD | PROJECT_MEMBER; " +
        "AWARD với AWARD_GENERAL; SUPERVISION với PHD | MASTER. " +
        "venue là tên tạp chí/hội nghị/nhà xuất bản/cơ quan; year là số nguyên. Tối đa 40 công trình. " +
        "Không suy đoán nội dung không có trong tài liệu; khoá nào không tìm thấy thì để null, works không có thì [].";

    private static readonly HashSet<string> Degrees = new(StringComparer.OrdinalIgnoreCase) { "Bachelor", "Master", "PhD", "Professor" };
    private static readonly HashSet<string> Genders = new(StringComparer.OrdinalIgnoreCase) { "Male", "Female", "Other" };

    public async Task<ExtractedCvDto> ExtractAsync(Stream content, string fileName, string contentType, CancellationToken ct = default)
    {
        using var ms = new MemoryStream();
        await content.CopyToAsync(ms, ct);

        if (!_gemini.IsConfigured)
            return new ExtractedCvDto { Warning = "AI chưa được cấu hình — vui lòng nhập tay." };

        try
        {
            var raw = await GeminiFileInput.AskAboutFileAsync(_gemini, ms.ToArray(), fileName, contentType, Prompt, ct);
            return Parse(raw);
        }
        catch (ArgumentException)
        {
            throw;   // định dạng file không đọc được — báo thẳng cho người dùng
        }
        catch (Exception ex)
        {
            var lower = ex.Message.ToLowerInvariant();
            return new ExtractedCvDto
            {
                Warning = lower.Contains("429") || lower.Contains("quota") || lower.Contains("503")
                    ? "AI đang quá tải — chờ một lát rồi thử lại, hoặc nhập tay."
                    : "AI chưa đọc được CV này — thử lại hoặc nhập tay."
            };
        }
    }

    public static ExtractedCvDto Parse(string raw)
    {
        var json = raw.Trim();
        if (json.StartsWith("```"))
        {
            var nl = json.IndexOf('\n');
            if (nl >= 0) json = json[(nl + 1)..];
            if (json.EndsWith("```")) json = json[..^3];
        }

        try
        {
            using var doc = JsonDocument.Parse(json.Trim());
            var r = doc.RootElement;
            var degree = Str(r, "degreeLevel");
            var gender = Str(r, "gender");
            var dob = Str(r, "dateOfBirth");
            return new ExtractedCvDto
            {
                AcademicTitle = Str(r, "academicTitle"),
                ScientificRank = Str(r, "scientificRank"),
                DegreeLevel = degree != null && Degrees.Contains(degree) ? Degrees.First(d => d.Equals(degree, StringComparison.OrdinalIgnoreCase)) : null,
                Specialization = Str(r, "specialization"),
                SpecializationAreas = Str(r, "specializationAreas"),
                DateOfBirth = DateOnly.TryParse(dob, out var d) ? d.ToString("yyyy-MM-dd") : null,
                Gender = gender != null && Genders.Contains(gender) ? Genders.First(g => g.Equals(gender, StringComparison.OrdinalIgnoreCase)) : null,
                Hometown = Str(r, "hometown"),
                Nationality = Str(r, "nationality"),
                GsPgsYear = Int(r, "gsPgsYear"),
                GsPgsInstitution = Str(r, "gsPgsInstitution"),
                Institution = Str(r, "institution"),
                InstitutionAddress = Str(r, "institutionAddress"),
                Works = Works(r),
            };
        }
        catch (JsonException)
        {
            return new ExtractedCvDto { Warning = "AI trả về dữ liệu không đúng định dạng — thử lại hoặc nhập tay." };
        }
    }

    /// <summary>Chỉ giữ công trình có tên và đúng cặp mục/phân loại của BM02 — AI bịa mã lạ thì bỏ.</summary>
    private static List<AcademicWorkRequest> Works(JsonElement r)
    {
        if (!r.TryGetProperty("works", out var arr) || arr.ValueKind != JsonValueKind.Array) return new();
        var list = new List<AcademicWorkRequest>();
        foreach (var w in arr.EnumerateArray().Take(40))
        {
            if (w.ValueKind != JsonValueKind.Object) continue;
            var type = Str(w, "workType")?.ToUpperInvariant();
            var category = Str(w, "category")?.ToUpperInvariant();
            var title = Str(w, "title");
            if (type == null || category == null || title == null) continue;
            if (!WorkTypes.All.Contains(type) || !WorkCategories.For(type).Contains(category)) continue;
            list.Add(new AcademicWorkRequest
            {
                WorkType = type,
                Category = category,
                Title = title.Length > 500 ? title[..500] : title,
                Venue = Str(w, "venue"),
                Authors = Str(w, "authors"),
                Year = Int(w, "year") is int y && y is > 1900 and < 2100 ? y : null,
                SortOrder = list.Count,
            });
        }
        return list;
    }

    private static string? Str(JsonElement r, string key) =>
        r.TryGetProperty(key, out var v) && v.ValueKind == JsonValueKind.String && !string.IsNullOrWhiteSpace(v.GetString())
            ? v.GetString()!.Trim()
            : null;

    private static int? Int(JsonElement r, string key)
    {
        if (!r.TryGetProperty(key, out var v)) return null;
        if (v.ValueKind == JsonValueKind.Number && v.TryGetInt32(out var n)) return n;
        return v.ValueKind == JsonValueKind.String && int.TryParse(v.GetString(), out var s) ? s : null;
    }
}
