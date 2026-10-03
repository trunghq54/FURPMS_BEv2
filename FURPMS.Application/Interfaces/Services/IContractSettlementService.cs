using FURPMS.Application.DTOs.Settlements;

namespace FURPMS.Application.Interfaces.Services;

public interface IContractSettlementService
{
    Task<SettlementDto?> GetByContractAsync(Guid contractId);
    Task<SettlementDto> CreateAsync(Guid contractId, CreateSettlementRequest request);
    Task<SettlementDto> SignAsync(int settlementId, SignSettlementRequest request);
    Task<SettlementDto> MarkAccountingClearedAsync(int settlementId, DateOnly? clearedDate, bool clear = true);
    Task<SettlementDto> MarkAssetsClearedAsync(int settlementId, DateOnly? clearedDate, bool clear = true);

    /// <summary>Sửa số liệu / tóm tắt sản phẩm / ghi chú khi biên bản CHƯA ký.</summary>
    Task<SettlementDto> UpdateAsync(int settlementId, CreateSettlementRequest request);

    /// <summary>Xoá hồ sơ quyết toán chưa ký (lập nhầm, sai số liệu cần làm lại) — kèm lý do.</summary>
    Task DeleteAsync(int settlementId, string reason, Guid staffId);

    /// <summary>Huỷ chữ ký biên bản thanh lý (ký nhầm) — hợp đồng về Đang hiệu lực, kèm lý do.</summary>
    Task<SettlementDto> UnsignAsync(int settlementId, string reason, Guid staffId);
}
