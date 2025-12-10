using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TalentoPlus.Infrastructure.Data.Context;
using TalentoPlus.Infrastructure.Integrations.Pdf;

namespace TalentoPlus.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Admin")]
public class PdfController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly IPdfService _pdfService;
    private readonly ILogger<PdfController> _logger;

    public PdfController(
        AppDbContext context,
        IPdfService pdfService,
        ILogger<PdfController> logger)
    {
        _context = context;
        _pdfService = pdfService;
        _logger = logger;
    }

    /// <summary>
    /// Genera y descarga la hoja de vida de un empleado (Solo Admin)
    /// GET: api/pdf/employee/{employeeId}
    /// </summary>
    /// <param name="employeeId">ID del empleado</param>
    [HttpGet("employee/{employeeId}")]
    public async Task<IActionResult> DownloadEmployeeCV(Guid employeeId)
    {
        try
        {
            var employee = await _context.Employees
                .FirstOrDefaultAsync(e => e.Id == employeeId);

            if (employee == null)
                return NotFound(new { error = "Empleado no encontrado." });

            // Generar PDF
            var pdfBytes = _pdfService.GenerateEmployeeCV(employee);

            // Nombre del archivo
            var fileName = $"CV_{employee.FirstName}_{employee.LastName}_{DateTime.Now:yyyyMMdd}.pdf";

            // Retornar archivo
            return File(pdfBytes, "application/pdf", fileName);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al generar PDF para empleado {EmployeeId}", employeeId);
            return StatusCode(500, new { error = "Error al generar el PDF.", details = ex.Message });
        }
    }
}