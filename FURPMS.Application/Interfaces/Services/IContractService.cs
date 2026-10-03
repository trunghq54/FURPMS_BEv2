using FURPMS.Application.DTOs.Contract;

namespace FURPMS.Application.Interfaces.Services;

public interface IContractService
{
    Task<IEnumerable<ContractListResponse>> GetListAsync(Guid? piUserId = null);
    Task<ContractDetailResponse> GetByIdAsync(Guid contractId);
    Task<ContractDetailResponse> CreateAsync(CreateContractRequest request, Guid createdBy);
    Task<ContractDetailResponse> UpdateAsync(Guid contractId, UpdateContractRequest request);
    Task DeleteAsync(Guid contractId);
    Task<ContractDetailResponse> SignAsync(Guid contractId, Guid signedBy, DateOnly? signedOn = null);
    Task<ContractDetailResponse> TerminateAsync(Guid contractId, Guid terminatedBy, string reason);

    /// <summary>Khôi phục hợp đồng chấm dứt nhầm — kèm lý do, ghi sổ quyết định.</summary>
    Task<ContractDetailResponse> RestoreAsync(Guid contractId, Guid restoredBy, string reason);
}
