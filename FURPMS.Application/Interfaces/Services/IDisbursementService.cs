using FURPMS.Application.DTOs.Contract;

namespace FURPMS.Application.Interfaces.Services;

public interface IDisbursementService
{
    Task<IEnumerable<DisbursementResponse>> GetByContractAsync(Guid contractId);
    Task<IEnumerable<DisbursementResponse>> GenerateAsync(Guid contractId);
    Task<DisbursementResponse> ConfirmAsync(int disbursementId, ConfirmDisbursementRequest request, Guid processedBy);

    /// <summary>
    /// Gắn (hoặc gỡ, khi <c>DeliverableId = null</c>) sản phẩm minh chứng cho một đợt giải ngân.
    /// Lúc sinh đợt chỉ PARTIAL mới tự ghép theo thứ tự, WHOLE không có gì —
    /// đây là chỗ để Staff sửa/bổ sung cho đúng thực tế.
    /// </summary>
    Task<DisbursementResponse> LinkDeliverableAsync(int disbursementId, LinkDeliverableRequest request);

    /// <summary>Xoá lịch (chưa đợt nào đã chi) và sinh lại theo loại đề tài hiện tại — kèm lý do.</summary>
    Task<IEnumerable<DisbursementResponse>> RegenerateAsync(Guid contractId, string reason, Guid staffId);
    /// <summary>Chỉnh cả lịch giải ngân (thêm / bớt / sửa tỉ lệ, điều kiện) — kèm lý do, ghi sổ quyết định.</summary>
    Task<IEnumerable<DisbursementResponse>> SaveScheduleAsync(Guid contractId, SaveDisbursementScheduleRequest request, Guid staffId);

    /// <summary>Huỷ một lần "đánh dấu đã giải ngân" bấm nhầm — kèm lý do.</summary>
    Task<DisbursementResponse> UndoConfirmAsync(int disbursementId, string reason, Guid staffId);
}
