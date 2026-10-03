using System.Security.Claims;
using FURPMS.Application.Common;
using FURPMS.Application.DTOs.Settlements;
using FURPMS.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FURPMS.API.Controllers;

[ApiController]
[Authorize]
public class ContractSettlementsController : ControllerBase
{
    private readonly IContractSettlementService _service;

    public ContractSettlementsController(IContractSettlementService service) => _service = service;

    [ProducesResponseType(typeof(ApiResponse<SettlementDto?>), StatusCodes.Status200OK)]
    [HttpGet("api/contracts/{contractId:guid}/settlement")]
    public async Task<IActionResult> Get(Guid contractId)
    {
        var dto = await _service.GetByContractAsync(contractId);
        return Ok(ApiResponse<SettlementDto?>.Ok(dto));
    }

    [ProducesResponseType(typeof(ApiResponse<SettlementDto>), StatusCodes.Status200OK)]
    [HttpPost("api/contracts/{contractId:guid}/settlement")]
    [Authorize(Roles = "Admin,Staff")]
    public async Task<IActionResult> Create(Guid contractId, [FromBody] CreateSettlementRequest request)
    {
        var dto = await _service.CreateAsync(contractId, request);
        return Ok(ApiResponse<SettlementDto>.Ok(dto));
    }

    [ProducesResponseType(typeof(ApiResponse<SettlementDto>), StatusCodes.Status200OK)]
    [HttpPut("api/settlements/{id:int}")]
    [Authorize(Roles = "Admin,Staff")]
    public async Task<IActionResult> Update(int id, [FromBody] CreateSettlementRequest request)
    {
        var dto = await _service.UpdateAsync(id, request);
        return Ok(ApiResponse<SettlementDto>.Ok(dto, "Đã cập nhật hồ sơ quyết toán."));
    }

    // Xoá hồ sơ CHƯA ký để lập lại — ?reason= bắt buộc, ghi sổ quyết định.
    [HttpDelete("api/settlements/{id:int}")]
    [Authorize(Roles = "Admin,Staff")]
    public async Task<IActionResult> Delete(int id, [FromQuery] string? reason = null)
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        await _service.DeleteAsync(id, reason ?? "", userId);
        return Ok(ApiResponse.Ok("Đã xoá hồ sơ quyết toán."));
    }

    // Huỷ chữ ký BM13 — lý do bắt buộc.
    [ProducesResponseType(typeof(ApiResponse<SettlementDto>), StatusCodes.Status200OK)]
    [HttpPost("api/settlements/{id:int}/unsign")]
    [Authorize(Roles = "Admin,Staff")]
    public async Task<IActionResult> Unsign(int id, [FromBody] FURPMS.Application.DTOs.Contract.DisbursementFixRequest request)
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var dto = await _service.UnsignAsync(id, request.Reason, userId);
        return Ok(ApiResponse<SettlementDto>.Ok(dto, "Đã huỷ chữ ký biên bản thanh lý."));
    }

    [ProducesResponseType(typeof(ApiResponse<SettlementDto>), StatusCodes.Status200OK)]
    [HttpPost("api/settlements/{id:int}/sign")]
    [Authorize(Roles = "Admin,Staff")]
    public async Task<IActionResult> Sign(int id, [FromBody] SignSettlementRequest request)
    {
        var dto = await _service.SignAsync(id, request);
        return Ok(ApiResponse<SettlementDto>.Ok(dto));
    }

    [ProducesResponseType(typeof(ApiResponse<SettlementDto>), StatusCodes.Status200OK)]
    [HttpPost("api/settlements/{id:int}/accounting-cleared")]
    [Authorize(Roles = "Admin,Staff")]
    public async Task<IActionResult> MarkAccountingCleared(int id, [FromBody] MarkClearedRequest request)
    {
        var dto = await _service.MarkAccountingClearedAsync(id, request.ClearedDate, request.Clear);
        return Ok(ApiResponse<SettlementDto>.Ok(dto));
    }

    [ProducesResponseType(typeof(ApiResponse<SettlementDto>), StatusCodes.Status200OK)]
    [HttpPost("api/settlements/{id:int}/assets-cleared")]
    [Authorize(Roles = "Admin,Staff")]
    public async Task<IActionResult> MarkAssetsCleared(int id, [FromBody] MarkClearedRequest request)
    {
        var dto = await _service.MarkAssetsClearedAsync(id, request.ClearedDate, request.Clear);
        return Ok(ApiResponse<SettlementDto>.Ok(dto));
    }
}
