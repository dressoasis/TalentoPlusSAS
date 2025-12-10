using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TalentoPlus.Application.DTOs.Employees;
using TalentoPlus.Application.Services;

namespace TalentoPlus.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class EmployeesController : ControllerBase  // ← Plural: Employees
{
    private readonly EmployeeService _service;

    public EmployeesController(EmployeeService service)
    {
        _service = service;
    }

    // ============================================================
    // GET: api/employees
    // ============================================================
    [HttpGet]
    [Authorize(Roles = "Admin,User")]
    public async Task<IActionResult> GetAll()
    {
        var result = await _service.GetAllAsync();
        return Ok(result);
    }

    // ============================================================
    // GET: api/employees/{id}
    // ============================================================
    [HttpGet("{id}")]
    [Authorize(Roles = "Admin,User")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var result = await _service.GetByIdAsync(id);
        if (result == null) return NotFound();
        return Ok(result);
    }

    // ============================================================
    // POST: api/employees
    // ============================================================
    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Create(EmployeeCreateRequest dto)
    {
        var ok = await _service.CreateAsync(dto);
        if (!ok) return BadRequest("Error creating employee.");
        return Ok(new { message = "Employee created successfully." });
    }

    // ============================================================
    // PUT: api/employees/{id}
    // ============================================================
    [HttpPut("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Update(Guid id, EmployeeUpdateRequest dto)
    {
        if (id != dto.Id)
            return BadRequest("ID mismatch.");

        var ok = await _service.UpdateAsync(dto);
        if (!ok) return NotFound();

        return Ok(new { message = "Employee updated successfully." });
    }

    // ============================================================
    // DELETE: api/employees/{id}
    // ============================================================
    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var ok = await _service.DeleteAsync(id);
        if (!ok) return NotFound();

        return Ok(new { message = "Employee deleted successfully." });
    }
}