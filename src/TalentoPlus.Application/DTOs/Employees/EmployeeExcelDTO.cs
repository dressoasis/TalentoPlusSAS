using TalentoPlus.Domain.Enums;

namespace TalentoPlus.Application.DTOs.Employees;

public class EmployeeExcelDTO
{
    public string Document { get; set; } = string.Empty;
    public string FirstName { get; set; }
    public string LastName { get; set; }

    // Calculado a partir de la edad del Excel
    public DateTime BirthDate { get; set; }

    public string Email { get; set; }
    public string Phone { get; set; }
    public string Address { get; set; }

    public decimal Salary { get; set; }
    public DateTime HireDate { get; set; }
    public string ProfessionalProfile { get; set; }

    // AHORA USAMOS ENUMS DIRECTAMENTE
    public DepartmentEnum Department { get; set; }
    public Position Position { get; set; }
    public EducationLevelEnum EducationLevel { get; set; }
    public EmploymentStatus Status { get; set; }
}
public class ImportResultDTO
{
    public bool Success { get; set; }
    public int Imported { get; set; }
    public int Updated { get; set; }
    public List<string> Errors { get; set; } = new();
}
