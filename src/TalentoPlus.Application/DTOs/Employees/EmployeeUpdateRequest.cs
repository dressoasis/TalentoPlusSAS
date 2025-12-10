using TalentoPlus.Domain.Enums;

namespace TalentoPlus.Application.DTOs.Employees;

public class EmployeeUpdateRequest
{
    public Guid Id { get; set; }

    public string FirstName { get; set; }
    public string LastName { get; set; }

    public DateTime BirthDate { get; set; }

    public string Email { get; set; }
    public string Phone { get; set; }
    public string Address { get; set; }

    public decimal Salary { get; set; }
    public DateTime HireDate { get; set; }
    public string ProfessionalProfile { get; set; }

    // ENUMS DIRECTOS
    public DepartmentEnum Department { get; set; }
    public Position Position { get; set; }
    public EducationLevelEnum EducationLevel { get; set; }
    public EmploymentStatus Status { get; set; }
}
