namespace FURPMS.Application.DTOs.Councils;

public class CouncilMemberResponse
{
    public Guid Id { get; set; }
    public Guid CouncilId { get; set; }
    public Guid UserId { get; set; }
    public string ReviewerName { get; set; } = null!;
    public string ReviewerEmail { get; set; } = null!;
    public string MemberRole { get; set; } = null!;
    public bool IsExternal { get; set; }
    public string Status { get; set; } = null!;
    public DateTime? InvitationSentAt { get; set; }
    public DateTime? ConfirmedAt { get; set; }
    public DateTime? DeclinedAt { get; set; }
    /// <summary>
    /// Lý do thành viên từ chối lời mời. Trước 03/10 có lưu trong DB nhưng API không trả ra — người
    /// được mời gõ lý do mà Phòng QLKH không bao giờ đọc được.
    /// </summary>
    public string? DeclineReason { get; set; }
    /// <summary>Lý do mời dù khác lĩnh vực — null nếu đúng lĩnh vực.</summary>
    public string? ExpertiseNote { get; set; }
}
