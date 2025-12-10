using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TalentoPlus.Application.DTOs.Employees;
using TalentoPlus.Domain.Entities;
using TalentoPlus.Domain.Enums;
using TalentoPlus.Infrastructure.Data.Context;
using TalentoPlus.Infrastructure.Identity.Identity;
using TalentoPlus.Infrastructure.Identity.Services;
using TalentoPlus.Infrastructure.Integrations.Email;

namespace TalentoPlus.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class RegisterController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly UserManager<AppUser> _userManager;
    private readonly JwtService _jwtService;
    private readonly IEmailService _emailService;
    private readonly ILogger<RegisterController> _logger;

    public RegisterController(
        AppDbContext context,
        UserManager<AppUser> userManager,
        JwtService jwtService,
        IEmailService emailService,
        ILogger<RegisterController> logger)
    {
        _context = context;
        _userManager = userManager;
        _jwtService = jwtService;
        _emailService = emailService;
        _logger = logger;
    }

    /// <summary>
    /// Autoregistro de empleado (público)
    /// POST: api/register
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> RegisterEmployee([FromBody] EmployeeRegisterRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        try
        {
            // Verificar si el email ya existe
            var existingUser = await _userManager.FindByEmailAsync(request.Email);
            if (existingUser != null)
                return BadRequest(new { error = "El email ya está registrado." });

            // Crear el Employee
            var employee = new Employee
            {
                FirstName = request.FirstName,
                LastName = request.LastName,
                BirthDate = DateTime.SpecifyKind(request.BirthDate, DateTimeKind.Utc),
                Email = request.Email,
                Phone = request.Phone,
                Address = request.Address,
                Salary = request.Salary,
                HireDate = DateTime.SpecifyKind(request.HireDate, DateTimeKind.Utc),
                ProfessionalProfile = request.ProfessionalProfile ?? "",
                Department = request.Department,
                Position = request.Position,
                EducationLevel = request.EducationLevel,
                Status = EmploymentStatus.Activo, // Por defecto activo
                CreatedAt = DateTime.UtcNow
            };

            _context.Employees.Add(employee);
            await _context.SaveChangesAsync();

            // Crear el AppUser
            var appUser = new AppUser
            {
                UserName = request.Email,
                Email = request.Email,
                FullName = $"{request.FirstName} {request.LastName}",
                EmailConfirmed = false, // Requiere confirmación
                EmployeeId = employee.Id
            };

            var result = await _userManager.CreateAsync(appUser, request.Password);

            if (!result.Succeeded)
            {
                // Si falla, eliminar el empleado creado
                _context.Employees.Remove(employee);
                await _context.SaveChangesAsync();

                return BadRequest(new
                {
                    error = "Error al crear el usuario.",
                    details = result.Errors.Select(e => e.Description)
                });
            }

            // Asignar rol "User"
            await _userManager.AddToRoleAsync(appUser, "User");

            // Enviar correo de bienvenida
            var emailSent = await _emailService.SendWelcomeEmailAsync(
                request.Email,
                $"{request.FirstName} {request.LastName}"
            );

            if (!emailSent)
            {
                _logger.LogWarning("No se pudo enviar el correo de bienvenida a {Email}", request.Email);
            }

            return Ok(new
            {
                message = "Registro completado exitosamente. Revisa tu correo electrónico.",
                employeeId = employee.Id,
                emailSent = emailSent
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al registrar empleado");
            return StatusCode(500, new { error = "Error interno al procesar el registro." });
        }
    }

    /// <summary>
    /// Login de empleado (público)
    /// POST: api/register/login
    /// </summary>
    [HttpPost("login")]
    public async Task<IActionResult> LoginEmployee([FromBody] EmployeeLoginRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        try
        {
            var user = await _userManager.FindByEmailAsync(request.Email);
            if (user == null)
                return Unauthorized(new { error = "Credenciales incorrectas." });

            var isPasswordValid = await _userManager.CheckPasswordAsync(user, request.Password);
            if (!isPasswordValid)
                return Unauthorized(new { error = "Credenciales incorrectas." });

            // Generar token JWT
            var roles = await _userManager.GetRolesAsync(user);
            var token = _jwtService.GenerateToken(user, roles.FirstOrDefault() ?? "User");

            return Ok(new
            {
                token = token,
                email = user.Email,
                fullName = user.FullName,
                roles = roles
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al hacer login");
            return StatusCode(500, new { error = "Error interno al procesar el login." });
        }
    }
}