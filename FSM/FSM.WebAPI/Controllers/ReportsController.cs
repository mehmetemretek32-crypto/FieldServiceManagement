using FSM.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FSM.WebAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Admin,Dispatcher")] // Test aşamasında olduğumuz için şimdilik kapalı
public class ReportsController : ControllerBase
{
    private readonly IReportService _reportService;

    public ReportsController(IReportService reportService)
    {
        _reportService = reportService;
    }

    [HttpGet("work-orders/excel")]
    public async Task<IActionResult> DownloadWorkOrdersExcel()
    {
        var fileBytes = await _reportService.GenerateWorkOrdersExcelAsync();
        var fileName = $"IsEmirleri_{DateTime.Now:yyyyMMdd_HHmm}.xlsx";

        return File(
            fileBytes,
            "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            fileName);
    }

    [HttpGet("technicians/pdf")]
    public async Task<IActionResult> DownloadTechnicianPerformancePdf()
    {
        var fileBytes = await _reportService.GenerateTechnicianPerformancePdfAsync();
        var fileName = $"TeknisyenPerformans_{DateTime.Now:yyyyMMdd_HHmm}.pdf";

        return File(fileBytes, "application/pdf", fileName);
    }
}