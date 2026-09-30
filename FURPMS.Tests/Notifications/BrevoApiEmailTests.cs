using System.Net;
using System.Text.Json;
using FURPMS.Application.Settings;
using FURPMS.Infrastructure.Repositories;
using FURPMS.Infrastructure.Services;
using FURPMS.Tests.Helpers;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace FURPMS.Tests.Notifications;

/// <summary>
/// Đường gửi thư qua API HTTPS của Brevo — dùng khi Railway chặn SMTP (30/09: cổng 587 và 2525
/// đều treo tới trần 10 giây, mọi lá thư FAILED). Không gọi mạng thật: bắt request bằng handler giả.
/// </summary>
public class BrevoApiEmailTests
{
    private sealed class CaptureHandler(HttpStatusCode status, string body = "{}") : HttpMessageHandler
    {
        public HttpRequestMessage? Request { get; private set; }
        public string? Json { get; private set; }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken ct)
        {
            Request = request;
            Json = request.Content == null ? null : await request.Content.ReadAsStringAsync(ct);
            return new HttpResponseMessage(status) { Content = new StringContent(body) };
        }
    }

    private static (SmtpEmailService service, FURPMS.Infrastructure.Data.FURPMSDbContext db) Build(
        HttpMessageHandler handler, string apiKey)
    {
        var db = TestDbContextFactory.Create($"brevo-{Guid.NewGuid()}");
        var settings = Options.Create(new EmailSettings
        {
            FromEmail = "sender@example.com",
            FromName = "FURPMS",
            BrevoApiKey = apiKey,
            FrontendUrl = "https://furpms.example"
        });
        var service = new SmtpEmailService(
            new NotificationRepository(db), settings,
            new SystemSettingService(new MasterDataRepository(db)), new HttpClient(handler));
        return (service, db);
    }

    [Fact]
    public async Task WithApiKey_PostsToBrevo_AndLogsSent()
    {
        var handler = new CaptureHandler(HttpStatusCode.Created, """{"messageId":"<x@brevo>"}""");
        var (service, db) = Build(handler, " xkeysib-test ");

        await service.SendAsync("pi@fpt.edu.vn", "Tài khoản đã tạo", "Nội dung", "ACCOUNT_CREATED",
            actionUrl: "/change-password");

        Assert.Equal("https://api.brevo.com/v3/smtp/email", handler.Request!.RequestUri!.ToString());
        Assert.Equal("xkeysib-test", handler.Request.Headers.GetValues("api-key").Single());

        using var json = JsonDocument.Parse(handler.Json!);
        var root = json.RootElement;
        Assert.Equal("sender@example.com", root.GetProperty("sender").GetProperty("email").GetString());
        Assert.Equal("pi@fpt.edu.vn", root.GetProperty("to")[0].GetProperty("email").GetString());
        Assert.Equal("Tài khoản đã tạo", root.GetProperty("subject").GetString());
        Assert.Contains("https://furpms.example/change-password", root.GetProperty("htmlContent").GetString());
        Assert.Contains("Nội dung", root.GetProperty("textContent").GetString());

        var log = await db.EmailLogs.SingleAsync();
        Assert.Equal("SENT", log.Status);
    }

    [Fact]
    public async Task BrevoRejects_LogsFailedWithBrevoReason()
    {
        var handler = new CaptureHandler(HttpStatusCode.Unauthorized, """{"code":"unauthorized","message":"Key not found"}""");
        var (service, db) = Build(handler, "xkeysib-wrong");

        await service.SendAsync("pi@fpt.edu.vn", "Tiêu đề", "Nội dung", "ACCOUNT_CREATED");

        var log = await db.EmailLogs.SingleAsync();
        Assert.Equal("FAILED", log.Status);
        Assert.Contains("401", log.ErrorMessage);
        Assert.Contains("Key not found", log.ErrorMessage);
    }
}
