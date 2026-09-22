using FURPMS.Application.DTOs.Ai;

namespace FURPMS.Application.Interfaces.Services;

/// <summary>
/// Tìm kiếm đề cương theo <b>ý nghĩa</b> chứ không theo từ khoá — gõ "dự đoán sinh viên bỏ học"
/// vẫn ra đề tài đặt tên "Mô hình cảnh báo sớm nguy cơ thôi học".
///
/// <para><b>Dùng chung kho vector với rà trùng lặp.</b> Cùng bảng <c>semantic_search_vectors</c>,
/// cùng <c>EntityType = "Proposal"</c>, cùng model nhúng. Nên bật tính năng này <b>không tốn thêm
/// quota</b> cho việc vector hoá — chỉ tốn đúng một lần gọi để nhúng câu hỏi.</para>
///
/// <para><b>Phạm vi:</b> chỉ tìm trong <b>đề cương đã nộp</b>. Đơn đặt hàng nghiên cứu
/// (<c>research_orders</c>) <b>chưa</b> được vector hoá — <c>EmbeddingWorker</c> chỉ xếp hàng đề
/// cương. Muốn tìm cả đơn đặt hàng thì phải mở rộng worker trước, đừng hứa trong tài liệu khi
/// chưa làm.</para>
///
/// <para><b>Không phải AI sinh chữ.</b> Đây là phép cosine trên vector — tất định, cùng câu hỏi
/// luôn cho cùng thứ tự. Không có mô hình nào "phán" kết quả nào liên quan hơn.</para>
/// </summary>
public interface ISemanticSearchService
{
    /// <summary>
    /// Tìm các đề cương gần nghĩa nhất với <paramref name="request"/>.
    /// </summary>
    /// <exception cref="Common.AppException">
    /// Câu hỏi rỗng, hoặc mô hình nhúng không trả về vector dùng được.
    /// </exception>
    Task<SemanticSearchResponse> SearchAsync(
        SemanticSearchRequest request,
        Guid userId,
        IEnumerable<string> roles,
        CancellationToken ct = default);
}
