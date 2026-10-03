namespace FURPMS.Application.Constants;

/// <summary>
/// Hai loại đề tài CỐ ĐỊNH của hệ thống theo QĐ543 — mã <c>BASIC</c> (cơ bản) và <c>APPLIED</c> (ứng dụng /
/// triển khai). Không tạo thêm, không xoá; Admin chỉ sửa tên và trần kinh phí. Mọi luồng sau vòng 1 (số kỳ tiến
/// độ, lịch giải ngân mặc định, khoá đợt, lộ trình) rẽ nhánh theo MÃ, đọc qua các hàm ở đây.
/// <para>
/// 03/10: trước đây suy từ cờ <c>RequireOrderingUnit</c>, mà loại "Ứng dụng" tạo tay trên deploy chưa tick cờ
/// ⇒ đề tài ứng dụng chạy luồng cơ bản. Lịch theo loại chỉ là MẶC ĐỊNH — Phòng QLKH chỉnh được từng hợp đồng,
/// mỗi lần chỉnh ghi sổ quyết định kèm lý do.
/// </para>
/// </summary>
public static class ResearchKind
{
    public const string Basic = "BASIC";
    public const string Applied = "APPLIED";

    public static string Of(bool isApplied) => isApplied ? Applied : Basic;

    /// <summary>Mã loại có phải đề tài ứng dụng không (mã cố định, không phân biệt hoa thường).</summary>
    public static bool IsApplied(string? code) => string.Equals(code?.Trim(), Applied, StringComparison.OrdinalIgnoreCase);

    public static bool IsFixedCode(string? code) =>
        IsApplied(code) || string.Equals(code?.Trim(), Basic, StringComparison.OrdinalIgnoreCase);

    /// <summary>QĐ543 Điều 10.1 — ứng dụng 2 kỳ báo cáo tiến độ, cơ bản 1 kỳ.</summary>
    public static int ProgressRounds(bool isApplied) => isApplied ? 2 : 1;

    /// <summary>QĐ543 Điều 16 — ứng dụng 4 đợt 30–30–30–10, cơ bản 1 đợt 100% sau nghiệm thu.</summary>
    public static int DisbursementRounds(bool isApplied) => isApplied ? 4 : 1;

    /// <summary>
    /// Tên/mã loại gợi ý là đề tài ứng dụng ("Nghiên cứu ứng dụng", "UNG_DUNG"…) — <c>null</c> khi tên không
    /// nói rõ. Chỉ dùng lúc khởi động để gom các loại tạo tay cũ (trên deploy) về 2 loại cố định.
    /// </summary>
    public static bool? KindFromName(string? code, string? name)
    {
        var text = $"{code} {name}".ToLowerInvariant().Replace('_', ' ');
        var basic = text.Contains("cơ bản") || text.Contains("co ban") || text.Contains("basic");
        var applied = text.Contains("ứng dụng") || text.Contains("ung dung") || text.Contains("applied")
                      || text.Contains("triển khai") || text.Contains("trien khai");
        if (basic == applied) return null;
        return applied;
    }
}
