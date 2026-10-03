using FURPMS.Application.Interfaces;

namespace FURPMS.Application.Common;

/// <summary>
/// Đếm ngược tới hạn — <b>một phép tính duy nhất cho cả hệ thống</b>.
///
/// <para><b>Vì sao phải nằm ở máy chủ (25/08):</b> lúc đầu giao diện tự trừ ngày bằng
/// <c>Date.now()</c> của trình duyệt. Kết quả: thẻ "Hạn sắp tới" trên bảng điều khiển hiện
/// <i>"Còn 7 ngày"</i> còn tab Sản phẩm của <b>cùng sản phẩm đó</b> hiện <i>"Còn 6 ngày"</i> —
/// máy chủ chạy giờ UTC, máy người dùng ở UTC+7, sát nửa đêm là lệch nguyên một ngày. Hai con
/// số khác nhau cho cùng một thứ trên cùng một màn hình thì người xem không tin số nào cả.</para>
///
/// <para>Đi qua <see cref="IClock"/> nên công cụ tua thời gian dùng để kiểm thử các mốc dài ngày
/// cũng tác động đúng vào đây, giống hệt bộ quét nhắc hạn qua email.</para>
/// </summary>
public static class DeadlineMath
{
    /// <summary>Số ngày từ hôm nay tới <paramref name="due"/>; âm = đã quá hạn; null nếu chưa đặt hạn.</summary>
    public static int? DaysLeft(DateOnly? due, DateOnly today) =>
        due is null ? null : due.Value.DayNumber - today.DayNumber;

    /// <summary>Bản tiện dụng nhận thẳng đồng hồ hệ thống.</summary>
    public static int? DaysLeft(DateOnly? due, IClock clock) =>
        DaysLeft(due, DateOnly.FromDateTime(clock.UtcNow));

    /// <summary>
    /// Cộng <paramref name="workingDays"/> ngày làm việc (bỏ thứ Bảy, Chủ nhật) — QĐ543 tính nhiều mốc
    /// theo NGÀY LÀM VIỆC (Điều 8.3.a, 12.3.a họp hội đồng; Điều 13.3 hoàn tất giao nộp 60 ngày làm việc).
    /// Không trừ ngày lễ: lịch nghỉ lễ đổi theo năm, hệ thống không có nguồn tin cậy để biết.
    /// </summary>
    public static DateOnly AddWorkingDays(DateOnly start, int workingDays)
    {
        var d = start;
        var added = 0;
        while (added < workingDays)
        {
            d = d.AddDays(1);
            if (d.DayOfWeek is not (DayOfWeek.Saturday or DayOfWeek.Sunday)) added++;
        }
        return d;
    }
}
