using FURPMS.Application.DTOs.ReviewScoring;

namespace FURPMS.Application.Interfaces.Services;

public interface IReviewScoringService
{
    Task<IEnumerable<RubricTemplateDto>> GetRubricTemplatesAsync();
    Task<RubricTemplateDto> GetRubricTemplateByIdAsync(int templateId);
    Task<ReviewScoreDto> SubmitScoreAsync(Guid councilId, Guid userId, SubmitScoreRequest request);
    Task<ReviewScoreDto?> GetMyScoreAsync(Guid councilId, Guid userId, Guid? projectId = null);
    Task<IEnumerable<ReviewScoreDto>> GetCouncilScoresAsync(Guid councilId, Guid? projectId = null);
    Task<CouncilDecisionDto> FinalizeDecisionAsync(Guid councilId, FinalizeDecisionRequest request);
    // Workflow biên bản: Thư ký soạn nháp → Chủ tịch duyệt = khoá + cập nhật status đề tài.
    Task<CouncilDecisionDto> SaveMinutesAsync(Guid councilId, Guid secretaryUserId, SaveMinutesRequest request);
    Task<CouncilDecisionDto> ApproveMinutesAsync(Guid councilId, Guid chairUserId, Guid? projectId = null);

    /// <summary>
    /// Các biên bản đã khoá của một đề tài. Được xem: Admin/Staff, chủ nhiệm đề tài, thành viên hội đồng
    /// đã chấm đề tài đó.
    /// </summary>
    Task<List<ProjectMinutesDto>> GetProjectMinutesAsync(Guid projectId, Guid userId, IEnumerable<string> roles);

    /// <summary>Được tải biên bản Word của hội đồng này không (cùng luật với <see cref="GetProjectMinutesAsync"/>).</summary>
    Task<bool> CanReadMinutesAsync(Guid councilId, Guid projectId, Guid userId, IEnumerable<string> roles);

    /// <summary>
    /// Chủ tịch <b>trả biên bản lại cho Thư ký sửa</b>, kèm ghi chú nêu rõ cần sửa gì.
    /// <para>
    /// QĐ543 Điều 8.3.c: Thư ký <i>ghi</i> biên bản, hội đồng <i>thông qua</i> — quy định không cho
    /// Chủ tịch tự sửa chữ của Thư ký. Nhưng trước đây hệ thống chỉ có hai đường: duyệt (khoá luôn)
    /// hoặc không làm gì, nên hai người phải liên lạc ngoài hệ thống và biên bản không lưu dấu vết.
    /// </para>
    /// <para>Không đổi trạng thái khoá — biên bản vốn đang là nháp; chỉ ghi yêu cầu và báo Thư ký.</para>
    /// </summary>
    Task<CouncilDecisionDto> RequestMinutesRevisionAsync(
        Guid councilId, Guid chairUserId, string note, Guid? projectId = null);
    Task<CouncilDecisionDto?> GetDecisionAsync(Guid councilId, Guid? projectId = null);
    /// <summary>BM12 mục 10.1 — kết quả bỏ phiếu chi tiết từng thành viên.</summary>
    Task<BallotTallyDto> GetBallotTallyAsync(Guid councilId, Guid? projectId);
}
