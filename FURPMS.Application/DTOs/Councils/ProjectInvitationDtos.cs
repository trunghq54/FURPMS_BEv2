namespace FURPMS.Application.DTOs.Councils;

/// <summary>Trạng thái nhận lời của MỘT thành viên với MỘT đề tài trong hội đồng (03/10).</summary>
public class ProjectInvitationStatusDto
{
    public Guid MemberId { get; set; }
    public Guid UserId { get; set; }
    public string? FullName { get; set; }
    public string? MemberRole { get; set; }
    /// <summary>NOT_SENT (chưa gửi thư mời) / INVITED / CONFIRMED / DECLINED.</summary>
    public string Status { get; set; } = "NOT_SENT";
    public DateTime? RespondedAt { get; set; }
    public string? DeclineReason { get; set; }
    public bool RespondedOnBehalf { get; set; }
}

/// <summary>Tóm tắt nhận lời của hội đồng với một đề tài — hiện thành chip "3/5 nhận lời" trên bảng.</summary>
public class ProjectInviteSummaryDto
{
    public Guid ProjectId { get; set; }
    public int Total { get; set; }
    public int Confirmed { get; set; }
    public int Pending { get; set; }
    public int Declined { get; set; }
    public int NotSent { get; set; }
}
