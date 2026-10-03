using FURPMS.Application.DTOs.Progress;

namespace FURPMS.Application.Interfaces.Services;

public interface IFinalReportService
{
    Task<FinalReportDto?> GetByContractAsync(Guid contractId);
    Task<FinalReportDto> SubmitAsync(Guid contractId, SubmitFinalReportRequest request, Guid userId);
    Task<FinalReportDto> RequestRevisionAsync(Guid reportId, RequestRevisionRequest request, Guid staffId);
    Task<FinalReportDto> AcceptAsync(Guid reportId, Guid staffId);
    Task<FinalReportDto> ArchiveAsync(Guid reportId, Guid staffId);

    /// <summary>Mở lại báo cáo đã tiếp nhận / đã lưu trữ (duyệt nhầm, cần nộp bản khác) — kèm lý do.</summary>
    Task<FinalReportDto> ReopenAsync(Guid reportId, string reason, Guid staffId);
}
