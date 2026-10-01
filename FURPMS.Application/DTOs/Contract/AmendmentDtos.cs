namespace FURPMS.Application.DTOs.Contract;

public class AmendmentListResponse
{
    public Guid Id { get; set; }
    public Guid ContractId { get; set; }
    public int CategoryId { get; set; }
    public string? CategoryName { get; set; }
    public string ChangeDescription { get; set; } = null!;
    public string Justification { get; set; } = null!;
    public string Status { get; set; } = null!;
    public DateTime RequestedAt { get; set; }
    public string? OldValue { get; set; }
    public string? NewValue { get; set; }
    /// <summary>
    /// Lý do Staff duyệt/từ chối. Nằm ở DANH SÁCH chứ không chỉ ở chi tiết: màn "Đơn của tôi" của
    /// PI chỉ gọi danh sách, thiếu field này thì đơn bị từ chối mà PI không biết vì sao.
    /// </summary>
    public string? ReviewerComments { get; set; }
    public DateTime? ReviewedAt { get; set; }
}

/// <summary>
/// Một đơn điều chỉnh đang CHỜ DUYỆT, kèm đủ ngữ cảnh để Phòng QLKH xử lý ngay từ danh sách chung
/// (01/10). Trước đó đơn chỉ nằm trong tab "Điều chỉnh" của từng hợp đồng — PI gửi xong không ai
/// biết, còn menu "Yêu cầu thay đổi" của Staff lại trỏ tới một luồng khác duyệt xong không có tác dụng.
/// </summary>
public class PendingAmendmentResponse : AmendmentListResponse
{
    public string? CategoryCode { get; set; }
    public string? ContractNumber { get; set; }
    public string? ProjectTitle { get; set; }
    public string? PiName { get; set; }
}

public class AmendmentDetailResponse : AmendmentListResponse
{
    public decimal? ChangePercentage { get; set; }
    public bool RequiresRectorApproval { get; set; }
}

public class CreateAmendmentRequest
{
    public int CategoryId { get; set; }
    public string ChangeDescription { get; set; } = null!;
    public string Justification { get; set; } = null!;
    public decimal? ChangePercentage { get; set; }
    public string? OldValue { get; set; }
    public string? NewValue { get; set; }
    public bool RequiresRectorApproval { get; set; }
}

public class ReviewAmendmentRequest
{
    public string? ReviewerComments { get; set; }
}
