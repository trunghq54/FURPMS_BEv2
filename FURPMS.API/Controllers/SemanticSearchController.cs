using System.Security.Claims;
using FURPMS.Application.Common;
using FURPMS.Application.DTOs.Ai;
using FURPMS.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FURPMS.API.Controllers;

/// <summary>
/// Tìm kiếm đề cương theo ý nghĩa — phục vụ FE-14 của chủ nhiệm đề tài.
///
/// <para>Dùng chung kho vector với rà trùng lặp (<c>semantic_search_vectors</c>), nên bật tính
/// năng này không tốn thêm quota vector hoá. Mỗi lần tìm tốn đúng một lần gọi mô hình nhúng cho
/// câu hỏi.</para>
/// </summary>
[ApiController]
[Authorize]
public class SemanticSearchController : ControllerBase
{
    private readonly ISemanticSearchService _service;

    public SemanticSearchController(ISemanticSearchService service)
    {
        _service = service;
    }

    private Guid CurrentUserId => Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
    private IEnumerable<string> CurrentRoles => User.FindAll(ClaimTypes.Role).Select(r => r.Value);

    /// <summary>
    /// Tìm các đề cương gần nghĩa nhất với câu hỏi.
    ///
    /// <para><c>indexedCount = 0</c> nghĩa là kho vector còn rỗng — khác hẳn với "tìm không ra".
    /// Giao diện phải phân biệt hai trường hợp đó.</para>
    /// </summary>
    [ProducesResponseType(typeof(ApiResponse<SemanticSearchResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status503ServiceUnavailable)]
    [HttpPost("api/ai/search")]
    public async Task<IActionResult> Search(
        [FromBody] SemanticSearchRequest request, CancellationToken ct)
    {
        var result = await _service.SearchAsync(request, CurrentUserId, CurrentRoles, ct);
        return Ok(ApiResponse<SemanticSearchResponse>.Ok(result));
    }
}
