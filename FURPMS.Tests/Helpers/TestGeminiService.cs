using FURPMS.Application.Interfaces;

namespace FURPMS.Tests.Helpers;

/// <summary>
/// <see cref="IGeminiService"/> giả cho test không cần gọi AI thật (vd <c>GetFlagsAsync</c>,
/// <c>ReviewAsync</c> — chỉ đọc vector đã có sẵn trong DB).
///
/// <para><c>IsConfigured = false</c> và mọi phương thức ném lỗi: nếu code dưới test lỡ gọi vào đây,
/// test phải ĐỎ ngay thay vì âm thầm nhận về chuỗi rỗng rồi qua được một cách giả tạo.</para>
/// </summary>
public class TestGeminiService : IGeminiService
{
    private readonly Func<string, float[]>? _embed;

    /// <param name="embed">
    /// Cho phép test cần vector (vd tìm kiếm ngữ nghĩa) trả về một vector định sẵn.
    /// Bỏ trống thì <see cref="EmbedAsync"/> vẫn ném lỗi như cũ — mặc định là "không được gọi".
    /// </param>
    public TestGeminiService(Func<string, float[]>? embed = null) => _embed = embed;

    public bool IsConfigured => false;
    public string EmbeddingModel => "test-embedding-model";

    public Task<string> GenerateTextAsync(string prompt, CancellationToken ct = default) =>
        throw new NotSupportedException("Test không được gọi Gemini thật.");

    public Task<GeminiUsage> GenerateWithUsageAsync(string prompt, CancellationToken ct = default) =>
        throw new NotSupportedException("Test không được gọi Gemini thật.");

    public Task<string> GenerateFromInlineDataAsync(
        byte[] data, string mimeType, string prompt, CancellationToken ct = default) =>
        throw new NotSupportedException("Test không được gọi Gemini thật.");

    public Task<float[]> EmbedAsync(string text, CancellationToken ct = default) =>
        _embed is null
            ? throw new NotSupportedException("Test không được gọi Gemini thật.")
            : Task.FromResult(_embed(text));
}
