namespace FURPMS.Application.DTOs.Users;

public class UserDto
{
    public Guid Id { get; set; }
    public string Email { get; set; } = null!;
    public string FullName { get; set; } = null!;
    public string? PhoneNumber { get; set; }
    public string? Department { get; set; }
    public string? AcademicDegree { get; set; }
    public string AccountType { get; set; } = null!;  // primary role
    public List<string> Roles { get; set; } = new();
    public bool IsActive { get; set; }
    public bool MustChangePassword { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? LastLoginAt { get; set; }

    /// <summary>Lĩnh vực chuyên môn (id <c>research_tracks</c>) — dùng xét người chấm đúng ngành.</summary>
    public List<int> ResearchTrackIds { get; set; } = new();
}

public class CreateUserRequest
{
    public string Email { get; set; } = null!;
    public string FullName { get; set; } = null!;
    public string? PhoneNumber { get; set; }
    public string? Department { get; set; }
    public int? AcademicDegree { get; set; }
    public List<int> Roles { get; set; } = new();  // Role.Id values
    public string TemporaryPassword { get; set; } = null!;

    /// <summary>Lĩnh vực chuyên môn, không bắt buộc. <c>null</c>/rỗng ⇒ chưa khai.</summary>
    public List<int>? ResearchTrackIds { get; set; }
}

public class UpdateUserRequest
{
    public string FullName { get; set; } = null!;
    public string? PhoneNumber { get; set; }
    public string? Department { get; set; }
    public int? AcademicDegree { get; set; }
    public List<int> Roles { get; set; } = new();

    /// <summary>
    /// Lĩnh vực chuyên môn. <c>null</c> ⇒ GIỮ NGUYÊN (client cũ không gửi trường này không được
    /// vô tình xoá sạch chuyên môn); danh sách rỗng ⇒ xoá hết.
    /// </summary>
    public List<int>? ResearchTrackIds { get; set; }
}
