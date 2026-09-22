namespace FURPMS.Application.DTOs.Ai;

/// <summary>Câu hỏi tìm kiếm ngữ nghĩa do người dùng gõ.</summary>
public class SemanticSearchRequest
{
    /// <summary>Chuỗi cần tìm. Không phải từ khoá — viết cả câu cho kết quả tốt hơn.</summary>
    public string Query { get; set; } = string.Empty;

    /// <summary>
    /// Số kết quả muốn lấy. Bỏ trống thì dùng <see cref="SemanticSearchDefaults.TopK"/>.
    ///
    /// <para>Đây là <b>tham số giao diện</b>, không phải tham số nghiệp vụ — nó chỉ đổi số dòng
    /// hiển thị, không đổi kết luận nào. Vì vậy nó nằm ở request chứ không nằm trong
    /// <c>system_settings</c> như các ngưỡng của rà trùng lặp.</para>
    /// </summary>
    public int? TopK { get; set; }
}

/// <summary>Hằng số mặc định cho tìm kiếm ngữ nghĩa.</summary>
public static class SemanticSearchDefaults
{
    /// <summary>Số kết quả mặc định.</summary>
    public const int TopK = 10;

    /// <summary>Trần số kết quả — chặn người gọi xin 10000 dòng.</summary>
    public const int MaxTopK = 50;

    /// <summary>Độ dài tối đa của câu hỏi; dài hơn thì cắt bớt trước khi gọi mô hình nhúng.</summary>
    public const int MaxQueryLength = 2000;

    /// <summary>Độ dài đoạn trích hiển thị dưới mỗi kết quả.</summary>
    public const int SnippetLength = 220;
}

/// <summary>Một kết quả tìm được.</summary>
public class SemanticSearchHitDto
{
    /// <summary>Id đề cương — giao diện dùng để mở chi tiết.</summary>
    public string Id { get; set; } = string.Empty;

    public string Title { get; set; } = string.Empty;

    /// <summary>Đoạn trích từ nội dung đã vector hoá, cắt ngắn để hiện dưới tiêu đề.</summary>
    public string Snippet { get; set; } = string.Empty;

    /// <summary>
    /// Độ liên quan quy về thang <b>0–100</b> để giao diện tô màu, làm tròn 1 chữ số thập phân.
    ///
    /// <para>Nguồn là cosine similarity trong [0, 1] nhân 100 — <b>không</b> phải xác suất, và
    /// không so sánh được giữa hai câu hỏi khác nhau. Nó chỉ dùng để xếp thứ tự trong cùng một
    /// lần tìm.</para>
    /// </summary>
    public double Relevance { get; set; }

    /// <summary>Luôn là <c>proposal</c> ở bản này — xem ghi chú phạm vi ở <c>ISemanticSearchService</c>.</summary>
    public string Type { get; set; } = "proposal";

    public string? ProjectCode { get; set; }
    public string? PiName { get; set; }
    public int? CycleYear { get; set; }
}

/// <summary>Kết quả một lần tìm kiếm.</summary>
public class SemanticSearchResponse
{
    /// <summary>Câu hỏi đã dùng (sau khi cắt bớt nếu quá dài).</summary>
    public string Query { get; set; } = string.Empty;

    /// <summary>
    /// Số đề cương đang có vector trong kho tại thời điểm tìm.
    ///
    /// <para>Bằng 0 nghĩa là <b>chưa có gì để tìm</b>, không phải "không tìm thấy" — giao diện phải
    /// phân biệt hai trường hợp này, nếu không người dùng tưởng hệ thống hỏng.</para>
    /// </summary>
    public int IndexedCount { get; set; }

    /// <summary>Model đã sinh ra các vector đang so — để đối chiếu khi đổi model mà quên vector hoá lại.</summary>
    public string? ModelUsed { get; set; }

    public List<SemanticSearchHitDto> Hits { get; set; } = new();
}
