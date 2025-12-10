using Mscc.GenerativeAI;
using Microsoft.EntityFrameworkCore;
using TalentoPlus.Infrastructure.Data.Context;
using System.Text.Json;

namespace TalentoPlus.Infrastructure.Integrations.AI;

public interface IAiService
{
    Task<string> ProcessQueryAsync(string userQuery);
}

public class AiService : IAiService
{
    private readonly AppDbContext _context;
    private readonly IConfiguration _configuration;
    private readonly ILogger<AiService> _logger;

    public AiService(
        AppDbContext context,
        IConfiguration configuration,
        ILogger<AiService> logger)
    {
        _context = context;
        _configuration = configuration;
        _logger = logger;
    }

    public async Task<string> ProcessQueryAsync(string userQuery)
    {
        try
        {
            // 1. Obtener estadísticas reales de la base de datos
            var stats = await GetDatabaseStatsAsync();

            // 2. Crear el contexto para la IA
            var systemPrompt = $@"
Eres un asistente de análisis de datos para TalentoPlus S.A.S.
Tienes acceso a las siguientes estadísticas REALES de la base de datos:

EMPLEADOS TOTALES: {stats.TotalEmployees}
EMPLEADOS ACTIVOS: {stats.ActiveEmployees}
EMPLEADOS INACTIVOS: {stats.InactiveEmployees}
EMPLEADOS EN VACACIONES: {stats.OnVacationEmployees}

EMPLEADOS POR DEPARTAMENTO:
{string.Join("\n", stats.EmployeesByDepartment.Select(d => $"- {d.Key}: {d.Value}"))}

EMPLEADOS POR POSICIÓN:
{string.Join("\n", stats.EmployeesByPosition.Select(p => $"- {p.Key}: {p.Value}"))}

EMPLEADOS POR NIVEL EDUCATIVO:
{string.Join("\n", stats.EmployeesByEducation.Select(e => $"- {e.Key}: {e.Value}"))}

SALARIO PROMEDIO: ${stats.AverageSalary:N0}
SALARIO MÁXIMO: ${stats.MaxSalary:N0}
SALARIO MÍNIMO: ${stats.MinSalary:N0}

IMPORTANTE: 
- Usa SOLO estos datos reales, NO inventes números.
- Si te preguntan algo que no está en estos datos, di que no tienes esa información específica.
- Responde de forma concisa y clara.
- Si te preguntan por un cargo o puesto, búscalo en 'EMPLEADOS POR POSICIÓN'.
";

            var userMessage = $"Pregunta del usuario: {userQuery}";

            // 3. Llamar a Gemini
            var apiKey = _configuration["GEMINI:ApiKey"];
            if (string.IsNullOrEmpty(apiKey))
            {
                return "Error: API Key de Gemini no configurada.";
            }

            var model = new GenerativeModel(apiKey);
            var chat = model.StartChat(new StartChatParams
            {
                History = new[]
                {
                    new Content
                    {
                        Role = "user",
                        Parts = new[] { new Part { Text = systemPrompt } }
                    },
                    new Content
                    {
                        Role = "model",
                        Parts = new[] { new Part { Text = "Entendido. Usaré solo los datos reales proporcionados para responder preguntas sobre los empleados de TalentoPlus." } }
                    }
                }
            });

            var response = await chat.SendMessageAsync(userMessage);
            return response.Text ?? "No se pudo generar una respuesta.";
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al procesar consulta con IA");
            return $"Error al procesar la consulta: {ex.Message}";
        }
    }

    private async Task<DatabaseStats> GetDatabaseStatsAsync()
    {
        var employees = await _context.Employees.ToListAsync();

        return new DatabaseStats
        {
            TotalEmployees = employees.Count,
            ActiveEmployees = employees.Count(e => e.Status == Domain.Enums.EmploymentStatus.Activo),
            InactiveEmployees = employees.Count(e => e.Status == Domain.Enums.EmploymentStatus.Inactivo),
            OnVacationEmployees = employees.Count(e => e.Status == Domain.Enums.EmploymentStatus.Vacaciones),
            
            EmployeesByDepartment = employees
                .GroupBy(e => e.Department.ToString())
                .ToDictionary(g => g.Key, g => g.Count()),
            
            EmployeesByPosition = employees
                .GroupBy(e => e.Position.ToString())
                .ToDictionary(g => g.Key, g => g.Count()),
            
            EmployeesByEducation = employees
                .GroupBy(e => e.EducationLevel.ToString())
                .ToDictionary(g => g.Key, g => g.Count()),
            
            AverageSalary = employees.Any() ? employees.Average(e => e.Salary) : 0,
            MaxSalary = employees.Any() ? employees.Max(e => e.Salary) : 0,
            MinSalary = employees.Any() ? employees.Min(e => e.Salary) : 0
        };
    }
}

public class DatabaseStats
{
    public int TotalEmployees { get; set; }
    public int ActiveEmployees { get; set; }
    public int InactiveEmployees { get; set; }
    public int OnVacationEmployees { get; set; }
    public Dictionary<string, int> EmployeesByDepartment { get; set; } = new();
    public Dictionary<string, int> EmployeesByPosition { get; set; } = new();
    public Dictionary<string, int> EmployeesByEducation { get; set; } = new();
    public decimal AverageSalary { get; set; }
    public decimal MaxSalary { get; set; }
    public decimal MinSalary { get; set; }
}