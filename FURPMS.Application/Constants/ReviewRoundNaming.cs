namespace FURPMS.Application.Constants;

/// <summary>
/// Tên hiển thị của một vòng chấm (01/10).
/// <para>
/// <c>RoundNumber</c> đếm CHUNG mọi loại vòng trong một (đợt + lĩnh vực): xét duyệt 1, nghiệm thu 2,
/// nghiệm thu 3… Hiện ra "Vòng 3" làm người đọc tưởng đề tài phải qua 3 vòng, trong khi QĐ543 chỉ có
/// hai hội đồng (xét duyệt, nghiệm thu). Một vòng thực chất là MỘT PHIÊN chấm cho một nhóm đề tài
/// cùng loại — nên gọi theo loại + số phiên của chính loại đó.
/// </para>
/// </summary>
public static class ReviewRoundNaming
{
    public static string TypeVi(string? roundType) => roundType switch
    {
        "SCREENING" => "Sơ loại",
        "REVIEW" => "Xét duyệt đề cương",
        "PROGRESS_CHECK" => "Kiểm tra tiến độ",
        "ACCEPTANCE" => "Nghiệm thu",
        _ => "Chấm"
    };

    /// <param name="sessionNo">Số thứ tự của vòng TRONG CÙNG LOẠI (1, 2…), không phải RoundNumber.</param>
    public static string Label(string? roundType, int sessionNo) => $"{TypeVi(roundType)} · phiên {sessionNo}";
}
