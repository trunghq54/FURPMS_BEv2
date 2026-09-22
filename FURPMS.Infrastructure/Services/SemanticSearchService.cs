using FURPMS.Application.Common;
using FURPMS.Application.DTOs.Ai;
using FURPMS.Application.Interfaces;
using FURPMS.Application.Interfaces.Services;
using FURPMS.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace FURPMS.Infrastructure.Services;

/// <inheritdoc cref="ISemanticSearchService"/>
public class SemanticSearchService : ISemanticSearchService
{
    /// <summary>Cùng giá trị với <see cref="DuplicateCheckService.VectorEntityType"/> — hai tính năng dùng chung kho vector.</summary>
    private const string VectorEntityType = DuplicateCheckService.VectorEntityType;

    private readonly FURPMSDbContext _db;
    private readonly IGeminiService _gemini;
    private readonly ILogger<SemanticSearchService> _logger;

    public SemanticSearchService(
        FURPMSDbContext db,
        IGeminiService gemini,
        ILogger<SemanticSearchService> logger)
    {
        _db = db;
        _gemini = gemini;
        _logger = logger;
    }

    public async Task<SemanticSearchResponse> SearchAsync(
        SemanticSearchRequest request,
        Guid userId,
        IEnumerable<string> roles,
        CancellationToken ct = default)
    {
        var query = (request.Query ?? string.Empty).Trim();
        if (query.Length == 0)
            throw new AppException("SEARCH_QUERY_EMPTY", "Nhập nội dung cần tìm.", 400);

        // Cắt trước khi gọi mô hình: câu quá dài vừa tốn token vừa loãng nghĩa, mà phần đuôi
        // gần như không đổi được vector.
        if (query.Length > SemanticSearchDefaults.MaxQueryLength)
            query = query[..SemanticSearchDefaults.MaxQueryLength];

        var topK = Math.Clamp(
            request.TopK ?? SemanticSearchDefaults.TopK,
            1,
            SemanticSearchDefaults.MaxTopK);

        // ── 1. Kho vector ────────────────────────────────────────────────────
        // Tải trước khi gọi Gemini: kho rỗng thì khỏi tốn một lần gọi mô hình chỉ để so với không gì.
        var stored = await _db.SemanticSearchVectors
            .Where(v => v.EntityType == VectorEntityType && v.Embedding != null)
            .Select(v => new { v.EntityId, v.Embedding, v.ContentSnapshot, v.ModelUsed })
            .ToListAsync(ct);

        if (stored.Count == 0)
        {
            return new SemanticSearchResponse
            {
                Query = query,
                IndexedCount = 0
            };
        }

        // ── 2. Nhúng câu hỏi ─────────────────────────────────────────────────
        float[] queryVector;
        try
        {
            queryVector = await _gemini.EmbedAsync(query, ct);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Không nhúng được câu hỏi tìm kiếm ngữ nghĩa");
            throw new AppException(
                "SEARCH_EMBEDDING_FAILED",
                "Không gọi được mô hình nhúng. Thử lại sau ít phút.",
                503);
        }

        if (queryVector.Length == 0)
            throw new AppException(
                "SEARCH_EMBEDDING_FAILED",
                "Mô hình nhúng trả về vector rỗng.",
                503);

        // ── 3. Cosine, xếp hạng ──────────────────────────────────────────────
        // Quét tuần tự trong bộ nhớ: kho cỡ vài trăm bản ghi nên dưới một giây. Lý do không dùng
        // pgvector nằm ở chú thích của SemanticSearchVector.Embedding.
        var scored = new List<(string EntityId, double Score, string? Snippet)>(stored.Count);
        var mismatched = 0;

        foreach (var row in stored)
        {
            var vec = VectorMath.Deserialize(row.Embedding);

            // CosineSimilarity trả 0 khi lệch số chiều — đếm lại để báo, vì đó là dấu hiệu đã đổi
            // model mà chưa vector hoá lại, chứ không phải "không có gì liên quan".
            if (vec.Length > 0 && vec.Length != queryVector.Length) mismatched++;

            var score = VectorMath.CosineSimilarity(queryVector, vec);
            if (score <= 0) continue;

            scored.Add((row.EntityId, score, row.ContentSnapshot));
        }

        if (mismatched > 0)
        {
            _logger.LogWarning(
                "{Count}/{Total} vector lệch số chiều so với câu hỏi — nhiều khả năng đã đổi model nhúng mà chưa vector hoá lại",
                mismatched, stored.Count);
        }

        var top = scored
            .OrderByDescending(x => x.Score)
            .Take(topK)
            .ToList();

        var response = new SemanticSearchResponse
        {
            Query = query,
            IndexedCount = stored.Count,
            ModelUsed = stored.Select(s => s.ModelUsed).FirstOrDefault(m => m != null)
        };

        if (top.Count == 0) return response;

        // ── 4. Ghép tên đề tài, chủ nhiệm, năm ───────────────────────────────
        var ids = top
            .Select(x => Guid.TryParse(x.EntityId, out var g) ? g : Guid.Empty)
            .Where(g => g != Guid.Empty)
            .ToList();

        // IgnoreQueryFilters: kho đối chiếu phải gồm cả đề tài đã kết thúc hoặc đã ẩn — tìm đề tài
        // cũ chính là mục đích của màn này.
        var rows = await _db.Proposals
            .IgnoreQueryFilters()
            .Where(p => ids.Contains(p.Id))
            .Select(p => new
            {
                p.Id,
                p.TitleVi,
                ProjectCode = p.Project.ProjectCode,
                PiName = p.Project.PiUser.FullName,
                CycleYear = (int?)p.Project.CycleTrack.Cycle.CycleYear
            })
            .ToListAsync(ct);

        var byId = rows.ToDictionary(r => r.Id);

        foreach (var (entityId, score, snippet) in top)
        {
            if (!Guid.TryParse(entityId, out var id) || !byId.TryGetValue(id, out var row)) continue;

            response.Hits.Add(new SemanticSearchHitDto
            {
                Id = id.ToString(),
                Title = row.TitleVi ?? "(không có tiêu đề)",
                Snippet = Shorten(snippet, row.TitleVi),
                Relevance = Math.Round(score * 100, 1),
                Type = "proposal",
                ProjectCode = row.ProjectCode,
                PiName = row.PiName,
                CycleYear = row.CycleYear
            });
        }

        return response;
    }

    /// <summary>
    /// Cắt đoạn trích ở ranh giới từ để không đứt giữa chữ.
    ///
    /// <para>Bỏ phần đầu trùng <paramref name="title"/>: <c>ContentSnapshot</c> được ghép từ
    /// tiêu đề + tóm tắt + mục tiêu, nên nếu để nguyên thì trên màn hình tiêu đề hiện hai lần
    /// liền nhau — vừa thừa chỗ vừa che mất phần nội dung thật sự đáng đọc.</para>
    /// </summary>
    private static string Shorten(string? text, string? title = null)
    {
        if (string.IsNullOrWhiteSpace(text)) return string.Empty;

        var clean = text.Replace('\n', ' ').Replace('\r', ' ').Trim();
        while (clean.Contains("  ")) clean = clean.Replace("  ", " ");

        if (!string.IsNullOrWhiteSpace(title))
        {
            var t = title.Trim();
            while (t.Contains("  ")) t = t.Replace("  ", " ");

            if (t.Length > 0 && clean.StartsWith(t, StringComparison.OrdinalIgnoreCase))
            {
                var rest = clean[t.Length..].TrimStart(' ', '-', '–', '—', ':', '.', ',');

                // Chỉ bỏ khi phần còn lại đủ dài để đáng hiện. Đề cương mà nội dung vector hoá
                // gần như chỉ có tiêu đề thì thà hiện lại tiêu đề còn hơn hiện một dòng trống.
                if (rest.Length >= 40) clean = rest;
            }
        }

        if (clean.Length <= SemanticSearchDefaults.SnippetLength) return clean;

        var cut = clean[..SemanticSearchDefaults.SnippetLength];
        var lastSpace = cut.LastIndexOf(' ');
        if (lastSpace > SemanticSearchDefaults.SnippetLength / 2) cut = cut[..lastSpace];

        return cut.TrimEnd() + "…";
    }
}
