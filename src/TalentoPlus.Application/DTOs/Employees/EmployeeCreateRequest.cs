using System.ComponentModel.DataAnnotations;
using TalentoPlus.Domain.Enums;

namespace TalentoPlus.Application.DTOs.Employees;

// ========================================
// DTO para Creación (Admin)
// ========================================
public class EmployeeCreateRequest
{
    public string Document { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public DateTime BirthDate { get; set; }
    public string Email { get; set; }
    public string Phone { get; set; }
    public string Address { get; set; }
    public decimal Salary { get; set; }
    public DateTime HireDate { get; set; }
    public string ProfessionalProfile { get; set; }

    public int Department { get; set; }
    public int Position { get; set; }
    public int EducationLevel { get; set; }
    public int Status { get; set; }
}

// ========================================
// DTO para AUTOREGISTRO (público)
// ========================================
public class EmployeeRegisterRequest
{
    [Required(ErrorMessage = "El nombre es requerido")]
    [StringLength(100, ErrorMessage = "El nombre no puede exceder 100 caracteres")]
    public string FirstName { get; set; }

    [Required(ErrorMessage = "El apellido es requerido")]
    [StringLength(100, ErrorMessage = "El apellido no puede exceder 100 caracteres")]
    public string LastName { get; set; }

    [Required(ErrorMessage = "La fecha de nacimiento es requerida")]
    public DateTime BirthDate { get; set; }

    [Required(ErrorMessage = "El email es requerido")]
    [EmailAddress(ErrorMessage = "Email inválido")]
    public string Email { get; set; }

    [Required(ErrorMessage = "El teléfono es requerido")]
    [Phone(ErrorMessage = "Teléfono inválido")]
    public string Phone { get; set; }

    [Required(ErrorMessage = "La dirección es requerida")]
    [StringLength(200)]
    public string Address { get; set; }

    [Required(ErrorMessage = "El salario es requerido")]
    [Range(0, double.MaxValue, ErrorMessage = "El salario debe ser mayor a 0")]
    public decimal Salary { get; set; }

    [Required(ErrorMessage = "La fecha de ingreso es requerida")]
    public DateTime HireDate { get; set; }

    [StringLength(1000)]
    public string ProfessionalProfile { get; set; }

    [Required(ErrorMessage = "El departamento es requerido")]
    public DepartmentEnum Department { get; set; }

    [Required(ErrorMessage = "La posición es requerida")]
    public Position Position { get; set; }

    [Required(ErrorMessage = "El nivel educativo es requerido")]
    public EducationLevelEnum EducationLevel { get; set; }

    [Required(ErrorMessage = "La contraseña es requerida")]
    [StringLength(100, MinimumLength = 6, ErrorMessage = "La contraseña debe tener al menos 6 caracteres")]
    public string Password { get; set; }
}

// ========================================
// DTO para LOGIN de empleado
// ========================================
public class EmployeeLoginRequest
{
    [Required(ErrorMessage = "El email es requerido")]
    [EmailAddress(ErrorMessage = "Email inválido")]
    public string Email { get; set; }

    [Required(ErrorMessage = "La contraseña es requerida")]
    public string Password { get; set; }
}