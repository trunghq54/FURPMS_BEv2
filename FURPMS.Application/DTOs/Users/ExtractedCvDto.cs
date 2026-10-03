namespace FURPMS.Application.DTOs.Users;

/// <summary>
/// Thông tin AI đọc được từ CV / lý lịch khoa học tải lên (03/10). CHỈ để điền sẵn — người dùng xem
/// lại rồi tự bấm Lưu; hệ thống không ghi gì vào hồ sơ khi chưa có người xác nhận.
/// </summary>
public class ExtractedCvDto
{
    public string? AcademicTitle { get; set; }
    public string? ScientificRank { get; set; }
    /// <summary>Bachelor / Master / PhD / Professor — khớp ô chọn của form.</summary>
    public string? DegreeLevel { get; set; }
    public string? Specialization { get; set; }
    public string? SpecializationAreas { get; set; }
    /// <summary>yyyy-MM-dd.</summary>
    public string? DateOfBirth { get; set; }
    /// <summary>Male / Female / Other.</summary>
    public string? Gender { get; set; }
    public string? Hometown { get; set; }
    public string? Nationality { get; set; }
    public int? GsPgsYear { get; set; }
    public string? GsPgsInstitution { get; set; }
    public string? Institution { get; set; }
    public string? InstitutionAddress { get; set; }

    /// <summary>Công trình đọc được — đã lọc chỉ giữ đúng mục/phân loại của Biểu mẫu 02.</summary>
    public List<AcademicWorkRequest> Works { get; set; } = new();

    /// <summary>Có giá trị khi AI không đọc được — form vẫn nhập tay bình thường.</summary>
    public string? Warning { get; set; }
}
