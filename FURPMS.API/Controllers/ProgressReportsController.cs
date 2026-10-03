using System.Security.Claims;
using FURPMS.Application.Common;
using FURPMS.Application.DTOs.Progress;
using FURPMS.Application.DTOs.Proposals;
using FURPMS.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FURPMS.API.Controllers;

[ApiController]
[Route("api/progress-reports")]
[Authorize]
public class ProgressReportsController : ControllerBase
{
    private readonly IProgressReportService _service;
    private readonly IProposalDocumentService _docs;

    public ProgressReportsController(IProgressReportService service, IProposalDocumentService docs)
    {
        _service = service;
        _docs = docs;
    }

    // ── File báo cáo (BM06) — PI upload PDF, Staff mở xem rồi mới đánh giá ─────

    // GET /api/progress-reports/{id}/documents
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<ProposalDocumentDto>>), StatusCodes.Status200OK)]
    [HttpGet("{id:guid}/documents")]
    public async Task<IActionResult> ListDocuments(Guid id)
    {
        var result = await _docs.ListForProgressReportAsync(id);
        return Ok(ApiResponse<IEnumerable<ProposalDocumentDto>>.Ok(result));
    }

    // POST /api/progress-reports/{id}/documents — chỉ PI của đề tài (hoặc Staff/Admin nộp hộ)
    [ProducesResponseType(typeof(ApiResponse<ProposalDocumentDto>), StatusCodes.Status200OK)]
    [HttpPost("{id:guid}/documents")]
    public async Task<IActionResult> UploadDocument(Guid id, IFormFile file, [FromQuery] string? kind = null)
    {
        if (file == null || file.Length == 0)
            throw new ArgumentException("Chưa chọn file hoặc file rỗng.");

        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var isStaffOrAdmin = User.IsInRole("Admin") || User.IsInRole("Staff");
        if (!isStaffOrAdmin && !await _service.IsPiOfReportAsync(id, userId))
            throw new ForbiddenException("Chỉ chủ nhiệm đề tài được nộp file báo cáo này.");

        // Biên bản họp Hội đồng đánh giá tiến độ (QĐ543 Điều 10.1) — Phòng QLKH tải lên, PI không được.
        var isMinutes = string.Equals(kind, "minutes", StringComparison.OrdinalIgnoreCase);
        if (isMinutes && !isStaffOrAdmin)
            throw new ForbiddenException("Chỉ Phòng QLKH được tải biên bản họp hội đồng đánh giá tiến độ.");

        await using var stream = file.OpenReadStream();
        var result = await _docs.UploadForProgressReportAsync(
            id, stream, file.FileName, file.ContentType, file.Length, userId,
            isMinutes ? "PROGRESS_MINUTES" : "PROGRESS_REPORT");
        return Ok(ApiResponse<ProposalDocumentDto>.Ok(result, "Đã tải file báo cáo lên."));
    }

    // GET /api/progress-reports/{id}/documents/{documentId}/download
    [HttpGet("{id:guid}/documents/{documentId:guid}/download")]
    public async Task<IActionResult> DownloadDocument(Guid id, Guid documentId)
    {
        var (stream, contentType, fileName) = await _docs.DownloadProgressReportDocAsync(documentId);
        return File(stream, contentType, fileName);
    }

    // GET /api/progress-reports?contractId={id}
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<ProgressReportSummaryDto>>), StatusCodes.Status200OK)]
    [HttpGet]
    public async Task<IActionResult> GetByContract([FromQuery] Guid contractId)
    {
        var result = await _service.GetByContractAsync(contractId);
        return Ok(ApiResponse<IEnumerable<ProgressReportSummaryDto>>.Ok(result));
    }

    // GET /api/progress-reports/{id}
    [ProducesResponseType(typeof(ApiResponse<ProgressReportDto>), StatusCodes.Status200OK)]
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var result = await _service.GetByIdAsync(id);
        return Ok(ApiResponse<ProgressReportDto>.Ok(result));
    }

    // POST /api/progress-reports/generate?contractId={id} — Staff sinh sẵn các kỳ định kỳ (QĐ543 Điều 10)
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<ProgressReportSummaryDto>>), StatusCodes.Status200OK)]
    [HttpPost("generate")]
    [Authorize(Roles = "Admin,Staff")]
    public async Task<IActionResult> Generate([FromQuery] Guid contractId, [FromQuery] int? roundCount = null)
    {
        var result = await _service.GenerateScheduledRoundsAsync(contractId, roundCount);
        return Ok(ApiResponse<IEnumerable<ProgressReportSummaryDto>>.Ok(result, "Đã tạo các kỳ báo cáo định kỳ."));
    }

    // POST /api/progress-reports?contractId={id}
    [ProducesResponseType(typeof(ApiResponse<ProgressReportDto>), StatusCodes.Status200OK)]
    [HttpPost]
    public async Task<IActionResult> Create(
        [FromQuery] Guid contractId,
        [FromBody] CreateProgressReportRequest request)
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var result = await _service.CreateAsync(contractId, request, userId);
        return Ok(ApiResponse<ProgressReportDto>.Ok(result));
    }

    // PUT /api/progress-reports/{id} — PI sửa nội dung khi còn nháp
    [ProducesResponseType(typeof(ApiResponse<ProgressReportDto>), StatusCodes.Status200OK)]
    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateProgressReportRequest request)
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var result = await _service.UpdateAsync(id, request, userId);
        return Ok(ApiResponse<ProgressReportDto>.Ok(result));
    }

    // POST /api/progress-reports/{id}/submit
    [ProducesResponseType(typeof(ApiResponse<ProgressReportDto>), StatusCodes.Status200OK)]
    [HttpPost("{id:guid}/submit")]
    public async Task<IActionResult> Submit(Guid id)
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var result = await _service.SubmitAsync(id, userId);
        return Ok(ApiResponse<ProgressReportDto>.Ok(result));
    }

    // PATCH /api/progress-reports/{id}/schedule — Staff đặt hạn nộp + lịch họp + link
    [ProducesResponseType(typeof(ApiResponse<ProgressReportDto>), StatusCodes.Status200OK)]
    [HttpPatch("{id:guid}/schedule")]
    [Authorize(Roles = "Admin,Staff")]
    public async Task<IActionResult> Schedule(Guid id, [FromBody] ScheduleProgressReportRequest request)
    {
        var result = await _service.ScheduleAsync(id, request);
        return Ok(ApiResponse<ProgressReportDto>.Ok(result));
    }

    // POST /api/progress-reports/{id}/evaluate
    [ProducesResponseType(typeof(ApiResponse<ProgressReportDto>), StatusCodes.Status200OK)]
    [HttpPost("{id:guid}/evaluate")]
    [Authorize(Roles = "Admin,Staff")]
    public async Task<IActionResult> Evaluate(Guid id, [FromBody] EvaluateProgressReportRequest request)
    {
        var staffId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var result = await _service.EvaluateAsync(id, request, staffId);
        return Ok(ApiResponse<ProgressReportDto>.Ok(result));
    }

    // Mở lại kết luận đã ghi — bắt buộc lý do, ghi vào sổ quyết định.
    [ProducesResponseType(typeof(ApiResponse<ProgressReportDto>), StatusCodes.Status200OK)]
    [HttpPost("{id:guid}/reopen")]
    [Authorize(Roles = "Admin,Staff")]
    public async Task<IActionResult> Reopen(Guid id, [FromBody] ReopenRequest request)
    {
        var staffId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var result = await _service.ReopenAsync(id, request.Reason, staffId);
        return Ok(ApiResponse<ProgressReportDto>.Ok(result, "Đã mở lại kết luận."));
    }

    // Xoá kỳ báo cáo: PI chỉ bản NHÁP; Phòng QLKH xoá được cả bản đã nộp (kèm ?reason=).
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, [FromQuery] string? reason = null)
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var isStaff = User.IsInRole("Admin") || User.IsInRole("Staff");
        await _service.DeleteAsync(id, userId, isStaff, reason);
        return Ok(ApiResponse.Ok("Đã xoá kỳ báo cáo."));
    }
}

public class ReopenRequest
{
    public string Reason { get; set; } = "";
}
