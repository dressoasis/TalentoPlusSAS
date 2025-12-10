using TalentoPlus.Domain.Enums;

namespace TalentoPlus.Domain.Entities;

public class Employee
{
    public Guid Id { get; set; } = Guid.NewGuid();

    // ===========================
    // Datos personales
    // ===========================
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public DateTime BirthDate { get; set; }

    // ===========================
    // Contacto
    // ===========================
    public string Email { get; set; }
    public string Phone { get; set; }
    public string Address { get; set; }

    // ===========================
    // Laboral
    // ===========================
    public decimal Salary { get; set; }
    public DateTime HireDate { get; set; }
    public string ProfessionalProfile { get; set; }

    // ===========================
    // ENUMS (ANTES ERAN FK)
    // ===========================
    public DepartmentEnum Department { get; set; }
    public Position Position { get; set; }
    public EducationLevelEnum EducationLevel { get; set; }
    public EmploymentStatus Status { get; set; }

    // ===========================
    // Tracking
    // ===========================
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
}
