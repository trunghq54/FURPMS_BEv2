using FURPMS.Infrastructure.Services;

namespace FURPMS.Tests.Notifications;

/// <summary>
/// Nghiệp vụ gọi <c>IEmailService.SendAsync</c> phải xong NGAY, không đợi SMTP — trước 29/09 nộp đề
/// cương trên Railway chờ thư từng người nhận, quá 15 giây trình duyệt báo mất kết nối.
/// </summary>
public class QueuedEmailServiceTests
{
    [Fact]
    public async Task SendAsync_OnlyQueues_AndKeepsEveryField()
    {
        var queue = new EmailQueue();
        var service = new QueuedEmailService(queue);
        var userId = Guid.NewGuid();

        var send = service.SendAsync("pi@example.com", "Tiêu đề", "Nội dung", "ACCOUNT_CREATED", userId, "/proposals/my");

        Assert.True(send.IsCompletedSuccessfully);
        var job = await queue.DequeueAsync(new CancellationTokenSource(TimeSpan.FromSeconds(1)).Token);
        Assert.Equal(new EmailJob("pi@example.com", "Tiêu đề", "Nội dung", "ACCOUNT_CREATED", userId, "/proposals/my"), job);
    }

    [Fact]
    public async Task Queue_KeepsOrder()
    {
        var queue = new EmailQueue();
        var service = new QueuedEmailService(queue);

        await service.SendAsync("a@example.com", "1", "", "T");
        await service.SendAsync("b@example.com", "2", "", "T");

        var ct = new CancellationTokenSource(TimeSpan.FromSeconds(1)).Token;
        Assert.Equal("a@example.com", (await queue.DequeueAsync(ct)).RecipientEmail);
        Assert.Equal("b@example.com", (await queue.DequeueAsync(ct)).RecipientEmail);
    }
}
