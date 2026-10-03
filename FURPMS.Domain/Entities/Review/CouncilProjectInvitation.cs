namespace FURPMS.Domain.Entities.Review;

/// <summary>
/// Lời mời chấm MỘT ĐỀ TÀI cụ thể cho MỘT thành viên hội đồng (03/10).
/// <para>
/// Trước đây lời mời chỉ có ở cấp hội đồng ("một lần xác nhận áp dụng cho toàn bộ đề tài") ⇒ hội đồng
/// đã có người nhận lời mà được gán thêm đề tài thì không ai được hỏi lại, và Phòng QLKH không xem được
/// ai đã nhận chấm đề tài nào. Nay mỗi cặp thành viên × đề tài có trạng thái riêng; trả lời lời mời hội
/// đồng thì áp luôn cho các đề tài đang chờ nên luồng cũ chạy y nguyên.
/// </para>
/// </summary>
public class CouncilProjectInvitation
{
    public int Id { get; set; }
    public Guid CouncilMemberId { get; set; }
    public Guid CouncilId { get; set; }
    public Guid ProjectId { get; set; }

    /// <summary>INVITED / CONFIRMED / DECLINED.</summary>
    public string Status { get; set; } = "INVITED";
    public DateTime InvitedAt { get; set; } = DateTime.UtcNow;
    public DateTime? RespondedAt { get; set; }
    public string? DeclineReason { get; set; }
    /// <summary>Phòng QLKH ghi nhận hộ (thành viên trả lời ngoài hệ thống).</summary>
    public Guid? RespondedOnBehalfBy { get; set; }

    public CouncilMember Member { get; set; } = null!;
}
