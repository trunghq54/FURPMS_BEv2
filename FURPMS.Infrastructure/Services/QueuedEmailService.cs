using System.Threading.Channels;
using FURPMS.Application.Interfaces.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace FURPMS.Infrastructure.Services;

/// <summary>Một lá thư chờ gửi — đủ tham số để gọi lại <see cref="IEmailService.SendAsync"/>.</summary>
public sealed record EmailJob(
    string RecipientEmail,
    string Subject,
    string Body,
    string EmailType,
    Guid? RecipientUserId,
    string? ActionUrl);

/// <summary>
/// Hàng đợi thư trong bộ nhớ, dùng chung giữa request (ghi) và <see cref="EmailDispatchWorker"/> (đọc)
/// ⇒ BẮT BUỘC đăng ký singleton.
/// </summary>
public class EmailQueue
{
    private readonly Channel<EmailJob> _channel =
        Channel.CreateUnbounded<EmailJob>(new UnboundedChannelOptions { SingleReader = true });

    public void Enqueue(EmailJob job) => _channel.Writer.TryWrite(job);

    public ValueTask<EmailJob> DequeueAsync(CancellationToken ct) => _channel.Reader.ReadAsync(ct);
}

/// <summary>
/// <see cref="IEmailService"/> mà mọi service nghiệp vụ nhận: chỉ XẾP HÀNG thư rồi trả về ngay.
///
/// <para><b>Vì sao (29/09):</b> trước đây thư được gửi NGAY TRONG request, lần lượt từng người nhận.
/// Trên Railway kết nối SMTP chậm/bị chặn nên mỗi lá chờ tới trần 10 giây — nộp đề cương báo cho vài
/// người là quá 15 giây của trình duyệt ⇒ người dùng thấy "Không kết nối được máy chủ" trong khi
/// nghiệp vụ ĐÃ LƯU XONG; bấm lại thì ăn lỗi trùng. Thư là việc phụ, không được giữ chân nghiệp vụ.</para>
///
/// <para><b>Đánh đổi:</b> hàng đợi nằm trong bộ nhớ — thư chưa kịp gửi mà ứng dụng khởi động lại
/// (redeploy) thì mất. Chấp nhận được: thông báo trong ứng dụng đã lưu DB trước khi xếp thư, người
/// nhận vẫn thấy trên chuông; và thư nào đã THỬ gửi đều có dòng trong <c>email_logs</c>.</para>
/// </summary>
public class QueuedEmailService : IEmailService
{
    private readonly EmailQueue _queue;

    public QueuedEmailService(EmailQueue queue) => _queue = queue;

    public Task SendAsync(
        string recipientEmail,
        string subject,
        string body,
        string emailType,
        Guid? recipientUserId = null,
        string? actionUrl = null)
    {
        _queue.Enqueue(new EmailJob(recipientEmail, subject, body, emailType, recipientUserId, actionUrl));
        return Task.CompletedTask;
    }
}

/// <summary>
/// Gửi thư đã xếp hàng, từng lá một, bằng <see cref="SmtpEmailService"/> — bản gửi thật, vẫn ghi
/// <c>email_logs</c> SENT/FAILED/SKIPPED như cũ.
/// </summary>
public class EmailDispatchWorker : BackgroundService
{
    private readonly EmailQueue _queue;
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<EmailDispatchWorker> _logger;

    public EmailDispatchWorker(
        EmailQueue queue, IServiceScopeFactory scopeFactory, ILogger<EmailDispatchWorker> logger)
    {
        _queue = queue;
        _scopeFactory = scopeFactory;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        // Nhả luồng ngay — xem chú thích ở AiSummaryPregenerationService.
        await Task.Yield();

        while (!stoppingToken.IsCancellationRequested)
        {
            EmailJob job;
            try
            {
                job = await _queue.DequeueAsync(stoppingToken);
            }
            catch (OperationCanceledException)
            {
                return;
            }

            // Mỗi lá một scope: SmtpEmailService ghi email_logs qua DbContext scoped. Lỗi một lá
            // KHÔNG được giết worker — lỗi lọt khỏi ExecuteAsync là dừng cả tiến trình.
            try
            {
                using var scope = _scopeFactory.CreateScope();
                var sender = scope.ServiceProvider.GetRequiredService<SmtpEmailService>();
                await sender.SendAsync(
                    job.RecipientEmail, job.Subject, job.Body, job.EmailType, job.RecipientUserId, job.ActionUrl);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Không gửi được thư {EmailType} tới {Recipient}", job.EmailType, job.RecipientEmail);
            }
        }
    }
}
