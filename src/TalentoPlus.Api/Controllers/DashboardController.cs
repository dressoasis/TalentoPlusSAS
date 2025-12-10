using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TalentoPlus.Infrastructure.Data.Context;
using TalentoPlus.Infrastructure.Integrations.AI;

namespace TalentoPlus.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Admin")]
public class DashboardController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly IAiService _aiService;
    private readonly ILogger<DashboardController> _logger;

    public DashboardController(
        AppDbContext context,
        IAiService aiService,
        ILogger<DashboardController> logger)
    {
        _context = context;
        _aiService = aiService;
        _logger = logger;
    }

    /// <summary>
    /// Obtiene las estadísticas del dashboard (3 tarjetas + más)
    /// GET: api/dashboard/stats
    /// </summary>
    [HttpGet("stats")]
    public async Task<IActionResult> GetStats()
    {
        try
        {
            var employees = await _context.Employees.ToListAsync();

            var stats = new
            {
                // 3 TARJETAS PRINCIPALES (requeridas)
                totalEmployees = employees.Count,
                onVacation = employees.Count(e => e.Status == Domain.Enums.EmploymentStatus.Vacaciones),
                activeEmployees = employees.Count(e => e.Status == Domain.Enums.EmploymentStatus.Activo),

                // TARJETAS ADICIONALES
                inactiveEmployees = employees.Count(e => e.Status == Domain.Enums.EmploymentStatus.Inactivo),
                averageSalary = employees.Any() ? employees.Average(e => e.Salary) : 0,

                // ESTADÍSTICAS POR DEPARTAMENTO
                employeesByDepartment = employees
                    .GroupBy(e => e.Department.ToString())
                    .Select(g => new { department = g.Key, count = g.Count() })
                    .ToList(),

                // ESTADÍSTICAS POR POSICIÓN
                employeesByPosition = employees
                    .GroupBy(e => e.Position.ToString())
                    .Select(g => new { position = g.Key, count = g.Count() })
                    .ToList(),

                // ESTADÍSTICAS POR NIVEL EDUCATIVO
                employeesByEducation = employees
                    .GroupBy(e => e.EducationLevel.ToString())
                    .Select(g => new { level = g.Key, count = g.Count() })
                    .ToList(),

                // DISTRIBUCIÓN DE SALARIOS
                salaryStats = new
                {
                    average = employees.Any() ? employees.Average(e => e.Salary) : 0,
                    max = employees.Any() ? employees.Max(e => e.Salary) : 0,
                    min = employees.Any() ? employees.Min(e => e.Salary) : 0,
                    total = employees.Sum(e => e.Salary)
                }
            };

            return Ok(stats);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener estadísticas del dashboard");
            return StatusCode(500, new { error = "Error al obtener estadísticas." });
        }
    }

    /// <summary>
    /// Chat con IA - Consultas en lenguaje natural
    /// POST: api/dashboard/ai-query
    /// </summary>
    [HttpPost("ai-query")]
    public async Task<IActionResult> AiQuery([FromBody] AiQueryRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Query))
            return BadRequest(new { error = "La consulta no puede estar vacía." });

        try
        {
            var response = await _aiService.ProcessQueryAsync(request.Query);
            
            return Ok(new
            {
                query = request.Query,
                response = response,
                timestamp = DateTime.UtcNow
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al procesar consulta de IA");
            return StatusCode(500, new { error = "Error al procesar la consulta con IA." });
        }
    }
}

public class AiQueryRequest
{
    public string Query { get; set; }
}