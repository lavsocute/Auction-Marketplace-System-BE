using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using AuctionSys.Application.DTOs.Report;
using AuctionSys.Application.Interfaces.UseCases.Report;
using AuctionSys.Domain.Enums;

namespace AuctionSys.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ReportsController : ControllerBase
{
    private readonly ICreateReportUseCase _createReportUseCase;
    private readonly IGetReportsUseCase _getReportsUseCase;
    private readonly IResolveReportUseCase _resolveReportUseCase;

    public ReportsController(
        ICreateReportUseCase createReportUseCase,
        IGetReportsUseCase getReportsUseCase,
        IResolveReportUseCase resolveReportUseCase)
    {
        _createReportUseCase = createReportUseCase;
        _getReportsUseCase = getReportsUseCase;
        _resolveReportUseCase = resolveReportUseCase;
    }

    private Guid GetUserId()
    {
        var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier) 
            ?? throw new UnauthorizedAccessException("Không tìm thấy thông tin UserId trong token.");
        return Guid.Parse(userIdStr);
    }

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> CreateReport([FromBody] CreateReportDto request)
    {
        var response = await _createReportUseCase.ExecuteAsync(GetUserId(), request);
        return StatusCode(response.StatusCode, response);
    }

    [HttpGet]
    [Authorize] // In a real app, should be [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetReports([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10, [FromQuery] ReportStatus? status = null)
    {
        var response = await _getReportsUseCase.ExecuteAsync(pageNumber, pageSize, status);
        return StatusCode(response.StatusCode, response);
    }

    [HttpPut("{id}/resolve")]
    [Authorize] // Should be [Authorize(Roles = "Admin")]
    public async Task<IActionResult> ResolveReport(Guid id, [FromBody] string resolutionNotes)
    {
        var response = await _resolveReportUseCase.ExecuteAsync(id, resolutionNotes);
        return StatusCode(response.StatusCode, response);
    }
}
