using Microsoft.AspNetCore.Mvc;
using TalentoPlus.Domain.Enums;

namespace TalentoPlus.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DepartmentsController : ControllerBase
{
    /// <summary>
    /// Obtiene la lista de todos los departamentos disponibles (público)
    /// GET: api/departments
    /// </summary>
    [HttpGet]
    public IActionResult GetAll()
    {
        var departments = Enum.GetValues<DepartmentEnum>()
            .Select(d => new
            {
                id = (int)d,
                name = d.ToString(),
                displayName = GetDepartmentDisplayName(d)
            })
            .ToList();

        return Ok(departments);
    }

    /// <summary>
    /// Obtiene todas las posiciones disponibles (público)
    /// GET: api/departments/positions
    /// </summary>
    [HttpGet("positions")]
    public IActionResult GetPositions()
    {
        var positions = Enum.GetValues<Position>()
            .Select(p => new
            {
                id = (int)p,
                name = p.ToString()
            })
            .ToList();

        return Ok(positions);
    }

    /// <summary>
    /// Obtiene todos los niveles educativos (público)
    /// GET: api/departments/education-levels
    /// </summary>
    [HttpGet("education-levels")]
    public IActionResult GetEducationLevels()
    {
        var levels = Enum.GetValues<EducationLevelEnum>()
            .Select(e => new
            {
                id = (int)e,
                name = e.ToString()
            })
            .ToList();

        return Ok(levels);
    }

    private string GetDepartmentDisplayName(DepartmentEnum dept)
    {
        return dept switch
        {
            DepartmentEnum.Tecnologia => "Tecnología",
            DepartmentEnum.RecursosHumanos => "Recursos Humanos",
            DepartmentEnum.Ventas => "Ventas",
            DepartmentEnum.Marketing => "Marketing",
            DepartmentEnum.Contabilidad => "Contabilidad",
            DepartmentEnum.Operaciones => "Operaciones",
            _ => dept.ToString()
        };
    }
}