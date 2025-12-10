using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TalentoPlus.Infrastructure.Data.Context;
using TalentoPlus.Infrastructure.Integrations.Pdf;

namespace TalentoPlus.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize] // Cualquier usuario autenticado
public class MeController : ControllerBase  // ← Mejor nombre: "Me"
{
    private readonly AppDbContext _context;
    private readonly IPdfService _pdfService;
    private readonly ILogger<MeController> _logger;

    public MeController(
        AppDbContext context,
        IPdfService pdfService,
        ILogger<MeController> logger)
    {
        _context = context;
        _pdfService = pdfService;
        _logger = logger;
    }

    /// <summary>
    /// Obtiene la información del empleado autenticado
    /// GET: api/me
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetMyInfo()
    {
        try
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            
            if (string.IsNullOrEmpty(userId))
                return Unauthorized(new { error = "No se pudo identificar al usuario." });

            // Buscar el AppUser y su Employee asociado
            var appUser = await _context.Users
                .Include(u => u.Employee)
                .FirstOrDefaultAsync(u => u.Id == userId);

            if (appUser?.Employee == null)
                return NotFound(new { error = "No se encontró información del empleado." });

            var employee = appUser.Employee;

            return Ok(new
            {
                id = employee.Id,
                firstName = employee.FirstName,
                lastName = employee.LastName,
                email = employee.Email,
                phone = employee.Phone,
                address = employee.Address,
                birthDate = employee.BirthDate,
                salary = employee.Salary,
                hireDate = employee.HireDate,
                professionalProfile = employee.ProfessionalProfile,
                department = employee.Department.ToString(),
                position = employee.Position.ToString(),
                educationLevel = employee.EducationLevel.ToString(),
                status = employee.Status.ToString()
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener información del empleado");
            return StatusCode(500, new { error = "Error al obtener la información." });
        }
    }

    /// <summary>
    /// Descarga la hoja de vida del empleado autenticado
    /// GET: api/me/cv
    /// </summary>
    [HttpGet("cv")]
    public async Task<IActionResult> DownloadMyCV()
    {
        try
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            
            if (string.IsNullOrEmpty(userId))
                return Unauthorized(new { error = "No se pudo identificar al usuario." });

            // Buscar el AppUser y su Employee asociado
            var appUser = await _context.Users
                .Include(u => u.Employee)
                .FirstOrDefaultAsync(u => u.Id == userId);

            if (appUser?.Employee == null)
                return NotFound(new { error = "No se encontró información del empleado." });

            var employee = appUser.Employee;

            // Generar PDF
            var pdfBytes = _pdfService.GenerateEmployeeCV(employee);

            // Nombre del archivo
            var fileName = $"MI_CV_{employee.FirstName}_{employee.LastName}_{DateTime.Now:yyyyMMdd}.pdf";

            // Retornar archivo
            return File(pdfBytes, "application/pdf", fileName);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al generar PDF del empleado");
            return StatusCode(500, new { error = "Error al generar el PDF." });
        }
    }
}