using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using FURPMS.Application.Interfaces.Services;
using Microsoft.Extensions.Configuration;

namespace FURPMS.Infrastructure.Services;

/// <summary>
/// Lưu file trên Cloudinary — dùng cho production (Render xoá sạch đĩa mỗi lần redeploy).
///
/// <para><b>Vì sao upload kiểu <c>raw</c>:</b> tài liệu ở đây là .docx/.pdf, không phải ảnh.
/// Cloudinary bắt buộc dùng <c>resource_type=raw</c> cho loại này.</para>
///
/// <para><b>Vì sao vẫn tải qua BE:</b> đã kiểm chứng — URL Cloudinary trả về **tải được mà
/// không cần đăng nhập**, kể cả kiểu <c>authenticated</c> (chữ ký nằm sẵn trong URL và không
/// hết hạn). Đề cương/hợp đồng là tài liệu mật ⇒ URL Cloudinary **chỉ tồn tại phía server**,
/// người dùng luôn đi qua <c>/documents/{id}/download</c> để BE còn kiểm quyền.
/// Lớp bảo vệ thực tế = endpoint có <c>[Authorize]</c> + tên file là GUID không đoán được.</para>
///
/// <para><b>Vì sao KHÔNG lưu URL Cloudinary vào DB:</b> cột <c>Document.StorageUrl</c> đang
/// chứa URL tải của BE và **được trả thẳng cho FE** — nhét URL Cloudinary vào đó là rò rỉ link
/// công khai. Thay vào đó URL được dựng lại từ <c>StorageBlobName</c> mỗi lần cần, nên không
/// phải thêm cột nào.</para>
///
/// <para>Không thêm SDK — Cloudinary chỉ cần REST + chữ ký SHA1, đỡ một dependency.</para>
/// </summary>
public class CloudinaryFileStorage : IFileStorage
{
    private readonly HttpClient _http;
    private readonly string _cloudName;
    private readonly string _apiKey;
    private readonly string _apiSecret;
    private readonly string _folder;

    /// <summary>
    /// Đọc dự phòng từ đĩa cho các file đã upload TRƯỚC khi bật Cloudinary.
    /// Không có cái này thì vừa bật Cloudinary là mọi tài liệu cũ 404 — đúng lỗi gặp lúc test.
    /// </summary>
    private readonly LocalDiskFileStorage _legacyDisk;

    public CloudinaryFileStorage(HttpClient http, IConfiguration config)
    {
        _legacyDisk = new LocalDiskFileStorage(config);
        _http = http;
        _cloudName = config["Cloudinary:CloudName"] ?? "";
        _apiKey = config["Cloudinary:ApiKey"] ?? "";
        _apiSecret = config["Cloudinary:ApiSecret"] ?? "";
        _folder = config["Cloudinary:Folder"] ?? "furpms";
    }

    public static bool IsConfigured(IConfiguration config) =>
        !string.IsNullOrWhiteSpace(config["Cloudinary:CloudName"])
        && !string.IsNullOrWhiteSpace(config["Cloudinary:ApiKey"])
        && !string.IsNullOrWhiteSpace(config["Cloudinary:ApiSecret"]);

    public string Description => $"Cloudinary (cloud {_cloudName}, thư mục {_folder})";

    /// <summary>Chữ ký Cloudinary: tham số sắp xếp a→z, nối "k=v&…", cộng api_secret, SHA1 hex.</summary>
    private string Sign(SortedDictionary<string, string> parameters)
    {
        var raw = string.Join("&", parameters.Select(p => $"{p.Key}={p.Value}")) + _apiSecret;
        return Convert.ToHexString(SHA1.HashData(Encoding.UTF8.GetBytes(raw))).ToLowerInvariant();
    }

    /// <summary>Với raw, public_id GỒM cả phần mở rộng — giữ nguyên blobName cho khớp DB.</summary>
    private string PublicId(string blobName) => $"{_folder}/{blobName}";

    /// <summary>
    /// Các public_id có thể đang chứa file. Thứ hai là tên HỎNG mà file upload từ ~25/09 tới 29/09
    /// bị Cloudinary gán (`…/x.docx.docx"`, xem <see cref="SaveAsync"/>). Thử thêm tên đó thì mọi
    /// tài liệu đã nộp trong khoảng này mở lại được luôn — không bắt người dùng nộp lại, không phải
    /// sửa DB hay đổi tên hàng loạt trên Cloudinary.
    /// </summary>
    private IEnumerable<string> CandidatePublicIds(string blobName)
    {
        yield return PublicId(blobName);
        var ext = Path.GetExtension(blobName);
        if (!string.IsNullOrEmpty(ext))
            yield return $"{PublicId(blobName)}{ext}\"";
    }

    private static string? ReadPublicId(string uploadResponse)
    {
        try
        {
            using var doc = JsonDocument.Parse(uploadResponse);
            return doc.RootElement.TryGetProperty("public_id", out var id) ? id.GetString() : null;
        }
        catch (JsonException)
        {
            return null;
        }
    }

    /// <summary>URL đọc, dựng lại được từ blobName (bỏ phần version) — khỏi lưu thêm cột nào.</summary>
    private string DeliveryUrl(string blobName) =>
        $"https://res.cloudinary.com/{_cloudName}/raw/upload/{PublicId(blobName)}";

    /// <summary>
    /// URL tải file <b>có chữ ký</b>, đi qua API chứ không qua CDN.
    ///
    /// <para><b>Vì sao phải có (18/08):</b> Cloudinary <b>chặn phát hành (delivery) file kiểu
    /// <c>raw</c></b> — mọi tài liệu của hệ thống đều là .docx/.pdf nên rơi hết vào diện này. Bấm
    /// vào URL CDN dựng theo <see cref="DeliveryUrl"/> nay trả <c>401</c> kèm
    /// <c>X-Cld-Error: deny or ACL failure</c>, và BE hiểu thành "file không còn trên storage" ⇒
    /// người dùng ăn 404 khi mở tài liệu <b>vừa mới nộp</b>. File cũ (tải lên trước khi Cloudinary
    /// đổi mặc định) vẫn phát hành được, nên lỗi chỉ lộ ra với file mới — rất dễ tưởng là hết.</para>
    ///
    /// <para>Đã đo từng cách: URL trần <c>401</c> · thêm chữ ký <c>s--…--</c> (SHA1/SHA256, có/không
    /// version) <c>401</c> · upload kèm <c>access_mode=public</c> vẫn <c>401</c>. Chỉ endpoint
    /// <c>/download</c> có chữ ký này trả <c>200</c> kèm đúng nội dung.</para>
    ///
    /// <para>Cách này còn <b>an toàn hơn</b> chỗ khác: tài liệu vẫn không phát hành công khai, ai
    /// có URL cũng không tải được — muốn lấy phải qua BE, mà BE thì có <c>[Authorize]</c>.</para>
    /// </summary>
    private string SignedDownloadUrl(string publicId)
    {
        var signed = new SortedDictionary<string, string>
        {
            ["public_id"] = publicId,
            ["timestamp"] = DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString(),
            ["type"] = "upload",
        };

        var query = string.Join("&", signed.Select(p => $"{p.Key}={Uri.EscapeDataString(p.Value)}"));
        return $"https://api.cloudinary.com/v1_1/{_cloudName}/raw/download" +
               $"?{query}&api_key={_apiKey}&signature={Sign(signed)}";
    }

    public async Task<StoredFile> SaveAsync(
        string blobName, Stream content, string contentType, CancellationToken ct = default)
    {
        var timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString();
        var signed = new SortedDictionary<string, string>
        {
            ["public_id"] = PublicId(blobName),
            ["timestamp"] = timestamp,
        };

        using var form = new MultipartFormDataContent();
        using var ms = new MemoryStream();
        await content.CopyToAsync(ms, ct);
        ms.Position = 0;

        var fileContent = new StreamContent(ms);
        fileContent.Headers.ContentType = MediaTypeHeaderValue.Parse(
            string.IsNullOrWhiteSpace(contentType) ? "application/octet-stream" : contentType);
        // Tự dựng Content-Disposition, KHÔNG dùng form.Add(content, name, fileName): overload đó
        // ghi thêm `filename*=utf-8''%22x.docx%22` — mã hoá LUÔN cả hai dấu nháy. Từ khoảng 25/09
        // Cloudinary đọc `filename*`, lấy đuôi `docx"` rồi NỐI vào public_id ⇒ file nằm ở
        // `…/x.docx.docx"` trong khi BE đi tìm `…/x.docx` ⇒ mọi tài liệu nộp mới đều không mở được,
        // AI cũng không đọc được (gặp thật 29/09 trên bản deploy). Đã upload thử để đo: có
        // `filename*` → sai tên; chỉ `filename="…"` → đúng tên.
        fileContent.Headers.ContentDisposition = new ContentDispositionHeaderValue("form-data")
        {
            Name = "\"file\"",
            FileName = $"\"{Path.GetFileName(blobName)}\""
        };
        form.Add(fileContent);

        // StringContent mặc định kèm "Content-Type: text/plain; charset=utf-8" cho từng field;
        // Cloudinary bỏ qua các field đó ⇒ tưởng là unsigned upload rồi trả 400. Phải gỡ header.
        // Ngoài ra .NET ghi `name=api_key` KHÔNG có dấu nháy; Cloudinary đòi `name="api_key"`.
        void Field(string name, string value)
        {
            var part = new StringContent(value);
            part.Headers.ContentType = null;
            form.Add(part, $"\"{name}\"");
        }

        foreach (var p in signed) Field(p.Key, p.Value);
        Field("api_key", _apiKey);
        Field("signature", Sign(signed));

        HttpResponseMessage resp;
        try
        {
            resp = await _http.PostAsync(
                $"https://api.cloudinary.com/v1_1/{_cloudName}/raw/upload", form, ct);
        }
        catch (Exception ex) when (ex is HttpRequestException or TaskCanceledException)
        {
            // Mất mạng / DNS hỏng ⇒ KHÔNG chặn người dùng nộp bài. Lưu tạm xuống đĩa;
            // đọc lại vẫn chạy vì OpenAsync đã có nhánh dự phòng đọc từ đĩa.
            ms.Position = 0;
            return await _legacyDisk.SaveAsync(blobName, ms, contentType, ct);
        }

        using (resp)
        {
            var body = await resp.Content.ReadAsStringAsync(ct);
            if (!resp.IsSuccessStatusCode)
                throw new InvalidOperationException(
                    $"Không tải được file lên Cloudinary ({(int)resp.StatusCode}): {Trim(body)}");

            // Cloudinary lặng lẽ đổi public_id là hỏng kiểu KHÔNG AI THẤY: nộp báo thành công, tới
            // lúc mở mới 404. Kiểm ngay tại đây để lần sau lỗi lộ ra lúc nộp chứ không phải lúc chấm.
            var actual = ReadPublicId(body);
            if (actual != null && actual != PublicId(blobName))
                throw new InvalidOperationException(
                    $"Cloudinary lưu file dưới tên khác dự kiến ('{actual}' thay vì '{PublicId(blobName)}') — file sẽ không mở lại được.");
        }

        // Cố ý KHÔNG trả secure_url của Cloudinary ra ngoài: caller ghi giá trị này vào
        // Document.StorageUrl, mà cột đó lại đi thẳng tới FE.
        return new StoredFile(blobName, DeliveryUrl(blobName));
    }

    public async Task<Stream?> OpenAsync(string blobName, string? storageUrl, CancellationToken ct = default)
    {
        var bytes = await ReadAllBytesAsync(blobName, storageUrl, ct);
        return bytes == null ? null : new MemoryStream(bytes);
    }

    public async Task<byte[]?> ReadAllBytesAsync(string blobName, string? storageUrl, CancellationToken ct = default)
    {
        // storageUrl bỏ qua có chủ ý — nó là URL tải của BE, không phải của Cloudinary.
        foreach (var publicId in CandidatePublicIds(blobName))
        {
            try
            {
                // Qua API có chữ ký, KHÔNG qua CDN: file raw bị chặn phát hành (xem SignedDownloadUrl).
                using var resp = await _http.GetAsync(SignedDownloadUrl(publicId), ct);
                if (resp.IsSuccessStatusCode)
                    return await resp.Content.ReadAsByteArrayAsync(ct);
            }
            catch (HttpRequestException)
            {
                // Mất mạng / DNS hỏng KHÔNG được thành 500. Thử đĩa, không có thì trả null → 404.
                break;
            }
            catch (TaskCanceledException)
            {
                break;
            }
        }

        // Không lấy được từ Cloudinary ⇒ có thể là file cũ còn nằm trên đĩa.
        return await _legacyDisk.ReadAllBytesAsync(blobName, storageUrl, ct);
    }

    public async Task DeleteAsync(string blobName, string? storageUrl, CancellationToken ct = default)
    {
        var basic = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{_apiKey}:{_apiSecret}"));
        var ids = string.Join("&", CandidatePublicIds(blobName)
            .Select(id => $"public_ids[]={Uri.EscapeDataString(id)}"));
        using var req = new HttpRequestMessage(
            HttpMethod.Delete,
            $"https://api.cloudinary.com/v1_1/{_cloudName}/resources/raw/upload?{ids}");
        req.Headers.Authorization = new AuthenticationHeaderValue("Basic", basic);

        // Xoá hụt trên Cloudinary không được làm hỏng nghiệp vụ — DB đã đánh dấu IsDeleted.
        try { await _http.SendAsync(req, ct); } catch (HttpRequestException) { }
    }

    private static string Trim(string s) => s.Length > 300 ? s[..300] : s;
}
