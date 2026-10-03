namespace FURPMS.Application.DTOs.Contract;

public class DisbursementResponse
{
    public int Id { get; set; }
    public Guid ContractId { get; set; }
    public int RoundNumber { get; set; }
    public decimal Percentage { get; set; }
    public decimal PlannedAmount { get; set; }
    public decimal? ActualAmount { get; set; }
    public string ConditionDescription { get; set; } = null!;
    public DateTime? ConditionMetAt { get; set; }
    public DateTime? DisbursedAt { get; set; }
    public string? BankReference { get; set; }
    public string Status { get; set; } = null!;
    public string? Notes { get; set; }

    // ── Sản phẩm minh chứng cho đợt này (rule #15: hệ thống không quản tiền,
    // chỉ theo dõi MỐC + MINH CHỨNG). Trả kèm tên/trạng thái để FE hiện thẳng,
    // khỏi phải gọi thêm API deliverables rồi tự ghép.
    public int? DeliverableId { get; set; }
    public string? DeliverableName { get; set; }
    /// <summary>PENDING / PASSED / FAILED — null nếu đợt chưa gắn sản phẩm.</summary>
    public string? DeliverableAcceptanceStatus { get; set; }
    public DateTime? DeliverableSubmittedAt { get; set; }
    /// <summary>Đợt có gắn sản phẩm nhưng sản phẩm chưa nghiệm thu Đạt ⇒ chưa được đánh dấu đã giải ngân.</summary>
    public bool IsBlockedByDeliverable { get; set; }
    /// <summary>Đã có ít nhất một file hợp đồng/chứng từ làm minh chứng cho lần xác nhận này.</summary>
    public bool HasEvidence { get; set; }

    /// <summary>
    /// Vì sao đợt này CHƯA được giải ngân theo QĐ543 Điều 16 (chưa ký HĐ / chờ tiến độ kỳ k Đạt / chờ
    /// nghiệm thu Đạt…). <c>null</c> = đủ điều kiện. Trước 03/10 màn hình chỉ biết điều kiện sản phẩm
    /// nên đợt 2–3 (mở theo báo cáo tiến độ) lúc nào cũng hiện "Đang chờ", không nói chờ cái gì.
    /// </summary>
    public string? LockReason { get; set; }
}

/// <summary>Staff sinh lại lịch giải ngân / huỷ một lần xác nhận đã chi — luôn kèm lý do.</summary>
public class DisbursementFixRequest
{
    public string Reason { get; set; } = "";
}

/// <summary>
/// Phòng QLKH chỉnh cả lịch giải ngân của một hợp đồng (03/10): thêm / bớt / sửa tỉ lệ và điều kiện từng đợt.
/// Gửi CẢ danh sách theo đúng thứ tự đợt; đợt có <c>Id</c> là đợt cũ, không có là đợt mới.
/// </summary>
public class SaveDisbursementScheduleRequest
{
    public string Reason { get; set; } = "";
    public List<DisbursementScheduleRow> Rows { get; set; } = new();
}

public class DisbursementScheduleRow
{
    public int? Id { get; set; }
    public decimal Percentage { get; set; }
    public string? ConditionDescription { get; set; }
}

/// <summary>Staff gắn / gỡ sản phẩm minh chứng cho một đợt giải ngân. <c>null</c> = gỡ.</summary>
public class LinkDeliverableRequest
{
    public int? DeliverableId { get; set; }
}

// Rule tuần 10: hệ thống KHÔNG quản tiền — "xác nhận" = đánh dấu đã giải ngân (kèm ghi chú/minh chứng).
// ActualAmount/BankReference giữ optional để tương thích dữ liệu cũ, KHÔNG bắt buộc nhập.
public class ConfirmDisbursementRequest
{
    public decimal? ActualAmount { get; set; }
    public string? BankReference { get; set; }
    public string? Notes { get; set; }
}
