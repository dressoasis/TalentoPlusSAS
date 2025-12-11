using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TalentoPlus.Infrastructure.Integrations;
using TalentoPlus.Application.Services;

namespace TalentoPlus.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ImportController : ControllerBase
    {
        private readonly ExcelImporter _excelImporter;
        private readonly EmployeeService _employeeService;
        private readonly ILogger<ImportController> _logger;

        public ImportController(
            ExcelImporter importer,
            EmployeeService employeeService,
            ILogger<ImportController> logger)
        {
            _excelImporter = importer;
            _employeeService = employeeService;
            _logger = logger;
        }

        [HttpPost("employees")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ImportEmployees(IFormFile file)
        {
            try
            {
                if (file == null || file.Length == 0)
                    return BadRequest(new { error = "Debe proporcionar un archivo Excel." });

                var extension = Path.GetExtension(file.FileName).ToLower();
                if (extension != ".xlsx")
                    return BadRequest(new { error = "El archivo debe ser .xlsx" });

                if (file.Length > 10 * 1024 * 1024)
                    return BadRequest(new { error = "El archivo no debe superar los 10MB." });

                var employees = await _excelImporter.ParseEmployeeExcel(file);

                if (employees == null || employees.Count == 0)
                    return BadRequest(new { error = "El archivo no contiene empleados válidos." });

                var result = await _employeeService.ImportFromExcelAsync(employees);

                return Ok(new
                {
                    message = "Importación completada exitosamente.",
                    total = employees.Count,
                    imported = result.Imported,
                    updated = result.Updated,
                    errors = result.Errors,
                    success = result.Success
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al importar empleados desde Excel");

                return StatusCode(500, new
                {
                    error = "Error interno al procesar el archivo.",
                    details = ex.Message
                });
            }
        }
    }
}
