using System.Security.Claims;
using FURPMS.Application.Common;
using FURPMS.Application.DTOs.MasterData;
using FURPMS.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FURPMS.API.Controllers;

[ApiController]
[Route("api/system-settings")]
[Authorize]
public class SystemSettingsController : ControllerBase
{
    private readonly ISystemSettingService _service;

    public SystemSettingsController(ISystemSettingService service)
    {
        _service = service;
    }

    /// <summary>Danh sách cấu hình vận hành (Admin xem/chỉnh).</summary>
    [HttpGet]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<ApiResponse<IEnumerable<SystemSettingResponse>>>> GetAll()
    {
        var result = await _service.GetAllAsync();
        return Ok(ApiResponse<IEnumerable<SystemSettingResponse>>.Ok(result));
    }

    /// <summary>Giới hạn upload hiện hành — mọi user đăng nhập đọc được để FE validate trước khi gửi file.</summary>
    [HttpGet("upload-policy")]
    public async Task<ActionResult<ApiResponse<UploadPolicyResponse>>> GetUploadPolicy()
    {
        var result = await _service.GetUploadPolicyAsync();
        return Ok(ApiResponse<UploadPolicyResponse>.Ok(result));
    }

    /// <summary>
    /// Bước nhảy điểm — mọi user đăng nhập đọc được. Hội đồng cần nó để dựng ô nhập điểm;
    /// bắt qua endpoint chỉ-Admin thì reviewer ăn 403 và cấu hình của Admin thành vô nghĩa.
    /// </summary>
    [HttpGet("scoring-policy")]
    public async Task<ActionResult<ApiResponse<ScoringPolicyResponse>>> GetScoringPolicy()
    {
        var result = await _service.GetScoringPolicyAsync();
        return Ok(ApiResponse<ScoringPolicyResponse>.Ok(result));
    }

    /// <summary>
    /// Chính sách hội đồng mà Staff cần để giao diện không bày thao tác "trả lời thay" khi Admin đã tắt.
    /// Endpoint chỉ trả đúng cờ cần dùng, không làm lộ toàn bộ cấu hình vận hành.
    /// </summary>
    [HttpGet("council-policy")]
    public async Task<ActionResult<ApiResponse<CouncilPolicyResponse>>> GetCouncilPolicy()
    {
        var result = await _service.GetCouncilPolicyAsync();
        return Ok(ApiResponse<CouncilPolicyResponse>.Ok(result));
    }

    /// <summary>
    /// Mặc định khi lập hợp đồng (01/10). Trước đây ô "Đại diện Bên A" để trống kèm chữ "để trống
    /// dùng tên mặc định" — người dùng không biết tên mặc định là ai. Thầy góp ý: hiện luôn tên,
    /// muốn đổi thì vào Cấu hình. Staff không đọc được endpoint chỉ-Admin nên tách riêng giá trị này.
    /// </summary>
    [HttpGet("contract-defaults")]
    public async Task<ActionResult<ApiResponse<ContractDefaultsResponse>>> GetContractDefaults()
    {
        var name = await _service.GetStringAsync(
            FURPMS.Application.Constants.SystemSettingKeys.ContractSideARepresentative,
            FURPMS.Application.Constants.SystemSettingKeys.DefaultContractSideARepresentative);
        return Ok(ApiResponse<ContractDefaultsResponse>.Ok(new ContractDefaultsResponse { SideARepresentative = name }));
    }

    [HttpPut("{key}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<ApiResponse<SystemSettingResponse>>> Update(
        string key, [FromBody] UpdateSystemSettingRequest request)
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var result = await _service.UpdateAsync(key, request.Value, userId);
        return Ok(ApiResponse<SystemSettingResponse>.Ok(result, "Đã cập nhật cấu hình."));
    }
}
