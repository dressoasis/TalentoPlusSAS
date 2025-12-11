using TalentoPlus.Application.DTOs.Employees;
using TalentoPlus.Domain.Entities;
using TalentoPlus.Application.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using TalentoPlus.Domain.Enums;

namespace TalentoPlus.Application.Services;

public partial class EmployeeService
{
    private readonly IApplicationDbContext _context;
    private readonly ILogger<EmployeeService> _logger;

    public EmployeeService(IApplicationDbContext context, ILogger<EmployeeService> logger)
    {
        _context = context;
        _logger = logger;
    }

    /// <summary>
    /// Importa empleados desde Excel. 
    /// Si el email ya existe, actualiza; si no, crea uno nuevo.
    /// </summary>
    public async Task<ImportResultDTO> ImportFromExcelAsync(List<EmployeeExcelDTO> employeeDtos)
    {
        var result = new ImportResultDTO { Success = true };

        foreach (var dto in employeeDtos)
        {
            try
            {
                // Buscar si ya existe por email
                var existingEmployee = await _context.Employees
                    .FirstOrDefaultAsync(e => e.Email.ToLower() == dto.Email.ToLower());

                if (existingEmployee != null)
                {
                    // ACTUALIZAR empleado existente
                    existingEmployee.Document = dto.Document;
                    existingEmployee.FirstName = dto.FirstName;
                    existingEmployee.LastName = dto.LastName;
                    existingEmployee.BirthDate = dto.BirthDate;
                    existingEmployee.Phone = dto.Phone;
                    existingEmployee.Address = dto.Address;
                    existingEmployee.Salary = dto.Salary;
                    existingEmployee.HireDate = dto.HireDate;
                    existingEmployee.ProfessionalProfile = dto.ProfessionalProfile;
                    existingEmployee.Department = dto.Department;
                    existingEmployee.Position = dto.Position;
                    existingEmployee.EducationLevel = dto.EducationLevel;
                    existingEmployee.Status = dto.Status;
                    existingEmployee.UpdatedAt = DateTime.UtcNow;

                    _context.Employees.Update(existingEmployee);
                    result.Updated++;
                }
                else
                {
                    // CREAR nuevo empleado
                    var newEmployee = new Employee
                    {
                        Document = dto.Document,
                        FirstName = dto.FirstName,
                        LastName = dto.LastName,
                        BirthDate = dto.BirthDate,
                        Email = dto.Email,
                        Phone = dto.Phone,
                        Address = dto.Address,
                        Salary = dto.Salary,
                        HireDate = dto.HireDate,
                        ProfessionalProfile = dto.ProfessionalProfile,
                        Department = dto.Department,
                        Position = dto.Position,
                        EducationLevel = dto.EducationLevel,
                        Status = dto.Status,
                        CreatedAt = DateTime.UtcNow
                    };

                    await _context.Employees.AddAsync(newEmployee);
                    result.Imported++;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al importar empleado: {Email}", dto.Email);
                result.Errors.Add($"Error con {dto.Email}: {ex.Message}");
            }
        }

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            var innerMessage = ex.InnerException?.Message ?? ex.Message;
            _logger.LogError(ex, "Error al guardar cambios en la base de datos. Inner: {Inner}", innerMessage);
            result.Success = false;
            result.Errors.Add($"Error al guardar: {innerMessage}");
            
            // Log stack trace para debugging
            _logger.LogError("Stack trace: {StackTrace}", ex.StackTrace);
            if (ex.InnerException != null)
            {
                _logger.LogError("Inner exception: {InnerException}", ex.InnerException.ToString());
            }
        }

        return result;
    }

    // ============================================
    // RESTORED CRUD METHODS
    // ============================================
    public async Task<List<EmployeeResponse>> GetAllAsync()
    {
        var list = await _context.Employees.ToListAsync();

        return list.Select(e => new EmployeeResponse
        {
            Id = e.Id,
            FullName = $"{e.FirstName} {e.LastName}",
            Email = e.Email,
            Phone = e.Phone,
            Salary = e.Salary,
            Status = e.Status.ToString(),

            Department = e.Department.ToString(),
            Position = e.Position.ToString(),
            EducationLevel = e.EducationLevel.ToString()

        }).ToList();
    }

    public async Task<EmployeeResponse?> GetByIdAsync(Guid id)
    {
        var e = await _context.Employees.FindAsync(id);
        if (e == null) return null;

        return new EmployeeResponse
        {
            Id = e.Id,
            FullName = $"{e.FirstName} {e.LastName}",
            Email = e.Email,
            Phone = e.Phone,
            Salary = e.Salary,
            Status = e.Status.ToString(),

            Department = e.Department.ToString(),
            Position = e.Position.ToString(),
            EducationLevel = e.EducationLevel.ToString()
        };
    }

    public async Task<bool> CreateAsync(EmployeeCreateRequest dto)
    {
        var employee = new Employee
        {
            Document = dto.Document,
            FirstName = dto.FirstName,
            LastName = dto.LastName,
            BirthDate = DateTime.SpecifyKind(dto.BirthDate, DateTimeKind.Utc),
            Address = dto.Address,
            Phone = dto.Phone,
            Email = dto.Email,
            Salary = dto.Salary,
            HireDate = DateTime.SpecifyKind(dto.HireDate, DateTimeKind.Utc),
            Status = (EmploymentStatus)dto.Status,
            ProfessionalProfile = dto.ProfessionalProfile,

            Department = (DepartmentEnum)dto.Department,
            Position = (Position)dto.Position,
            EducationLevel = (EducationLevelEnum)dto.EducationLevel,
            
            CreatedAt = DateTime.UtcNow
        };

        await _context.Employees.AddAsync(employee);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> UpdateAsync(EmployeeUpdateRequest dto)
    {
        var employee = await _context.Employees.FindAsync(dto.Id);
        if (employee == null) return false;

        employee.Document = dto.Document;
        employee.FirstName = dto.FirstName;
        employee.LastName = dto.LastName;
        employee.BirthDate = DateTime.SpecifyKind(dto.BirthDate, DateTimeKind.Utc);
        employee.Address = dto.Address;
        employee.Phone = dto.Phone;
        employee.Email = dto.Email;
        employee.Salary = dto.Salary;
        employee.HireDate = DateTime.SpecifyKind(dto.HireDate, DateTimeKind.Utc);
        employee.Status = (EmploymentStatus)dto.Status;
        employee.ProfessionalProfile = dto.ProfessionalProfile;

        employee.Department = (DepartmentEnum)dto.Department;
        employee.Position = (Position)dto.Position;
        employee.EducationLevel = (EducationLevelEnum)dto.EducationLevel;

        employee.UpdatedAt = DateTime.UtcNow;

        _context.Employees.Update(employee);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var employee = await _context.Employees.FindAsync(id);
        if (employee == null) return false;

        _context.Employees.Remove(employee);
        await _context.SaveChangesAsync();
        return true;
    }
}