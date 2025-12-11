using System.ComponentModel.DataAnnotations;
using TalentoPlus.Domain.Enums;

namespace TalentoPlus.Application.Auth;

public class RegisterRequest
{
    [Required(ErrorMessage = "El documento es requerido")]
    [StringLength(20, ErrorMessage = "El documento no puede exceder 20 caracteres")]
    public string Document { get; set; }

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
