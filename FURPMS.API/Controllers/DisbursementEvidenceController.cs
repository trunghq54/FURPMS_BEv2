using System.Security.Claims;
using FURPMS.Application.Common;
using FURPMS.Application.DTOs.Proposals;
using FURPMS.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FURPMS.Application.Interfaces.Repositories;

namespace FURPMS.API.Controllers;

// Minh chứng giải ngân (rule tuần 10 — thầy Đức): hệ thống KHÔNG chuyển tiền, chỉ lưu file
// hợp đồng/chứng từ do Staff upload làm minh chứng cho từng đợt giải ngân.
[ApiController]
[Route("api/disbursements/{disbursementId:int}/evidence")]
[Authorize]
public class DisbursementEvidenceController : ControllerBase
{
    private readonly IProposalDocumentService _docs;
    private readonly IContractRepository _contracts;

    public DisbursementEvidenceController(IProposalDocumentService docs, IContractRepository contracts)
    {
        _docs = docs;
        _contracts = contracts;
    }

    /// <summary>
    /// XEM chứng từ: Staff/Admin, và CHỦ NHIỆM của chính hợp đồng đó (03/10). Trước đây cả controller chỉ
    /// cho Admin/Staff, trong khi thông báo gửi PI lại bảo "xem minh chứng ở hợp đồng" — PI bấm vào là 403.
    /// Tải LÊN vẫn chỉ Staff (tiền chi ngoài hệ thống, Phòng QLKH là bên giữ chứng từ).
    /// </summary>
    private async Task AuthorizeViewAsync(int disbursementId)
    {
        if (User.IsInRole("Admin") || User.IsInRole("Staff")) return;
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var piId = await _contracts.Disbursements
            .Where(d => d.Id == disbursementId)
            .Select(d => (Guid?)d.Contract.Project.PiUserId)
            .FirstOrDefaultAsync()
            ?? throw new KeyNotFoundException("Không tìm thấy đợt giải ngân.");
        if (piId != userId)
            throw new ForbiddenException("Chỉ chủ nhiệm đề tài hoặc Phòng QLKH mới xem được chứng từ giải ngân này.");
    }

    [HttpGet]
    public async Task<IActionResult> List(int disbursementId)
    {
        await AuthorizeViewAsync(disbursementId);
        var result = await _docs.ListForDisbursementAsync(disbursementId);
        return Ok(ApiResponse<IEnumerable<ProposalDocumentDto>>.Ok(result));
    }

    [HttpPost]
    [Authorize(Roles = "Admin,Staff")]
    public async Task<IActionResult> Upload(int disbursementId, IFormFile file)
    {
        if (file == null || file.Length == 0)
            throw new ArgumentException("Chưa chọn file hoặc file rỗng.");

        var callerId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        await using var stream = file.OpenReadStream();
        var result = await _docs.UploadForDisbursementAsync(
            disbursementId, stream, file.FileName, file.ContentType, file.Length, callerId);
        return Ok(ApiResponse<ProposalDocumentDto>.Ok(result, "Đã lưu minh chứng."));
    }

    [HttpGet("{documentId:guid}/download")]
    public async Task<IActionResult> Download(int disbursementId, Guid documentId)
    {
        await AuthorizeViewAsync(disbursementId);
        // File phải thuộc ĐÚNG đợt vừa kiểm quyền — không thì ghép mã đợt của mình với mã file của đề tài
        // khác là tải được chứng từ người khác.
        var owned = await _docs.ListForDisbursementAsync(disbursementId);
        if (!owned.Any(d => d.Id == documentId))
            throw new KeyNotFoundException("Không tìm thấy minh chứng của đợt giải ngân này.");
        var (stream, contentType, fileName) = await _docs.DownloadEvidenceAsync(documentId);
        return File(stream, contentType, fileName);
    }
}
