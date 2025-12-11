using Microsoft.AspNetCore.Identity;
using TalentoPlus.Application.Auth;
using TalentoPlus.Application.Interfaces;
using TalentoPlus.Domain.Entities;
using TalentoPlus.Domain.Enums;
using TalentoPlus.Infrastructure.Identity.Identity;

namespace TalentoPlus.Infrastructure.Identity.Services;

public class AuthService
{
    private readonly UserManager<AppUser> _userManager;
    private readonly JwtService _jwtService;
    private readonly IApplicationDbContext _context;

    public AuthService(
        UserManager<AppUser> userManager, 
        JwtService jwtService,
        IApplicationDbContext context)
    {
        _userManager = userManager;
        _jwtService = jwtService;
        _context = context;
    }

    public async Task<TokenResponse> LoginAsync(LoginRequest request)
    {
        var user = await _userManager.FindByEmailAsync(request.Email);

        if (user == null || !await _userManager.CheckPasswordAsync(user, request.Password))
            return null;

        var roles = await _userManager.GetRolesAsync(user);

        var token = _jwtService.GenerateToken(user, roles.First());

        return new TokenResponse
        {
            Token = token,
            Email = user.Email,
            Role = roles.First()
        };
    }

    public async Task<(bool success, string? errorMessage)> RegisterAsync(RegisterRequest request)
    {
        // 1. Crear el Employee primero
        var employee = new Employee
        {
            Document = request.Document,
            FirstName = request.FirstName,
            LastName = request.LastName,
            BirthDate = DateTime.SpecifyKind(request.BirthDate, DateTimeKind.Utc),
            Email = request.Email,
            Phone = request.Phone,
            Address = request.Address,
            Salary = request.Salary,
            HireDate = DateTime.SpecifyKind(request.HireDate, DateTimeKind.Utc),
            ProfessionalProfile = request.ProfessionalProfile,
            Department = request.Department,
            Position = request.Position,
            EducationLevel = request.EducationLevel,
            Status = EmploymentStatus.Activo,  // Por defecto activo
            CreatedAt = DateTime.UtcNow
        };

        await _context.Employees.AddAsync(employee);
        await _context.SaveChangesAsync();

        // 2. Crear el AppUser vinculado al Employee
        var user = new AppUser
        {
            Email = request.Email,
            UserName = request.Email,
            FirstName = request.FirstName,
            LastName = request.LastName,
            FullName = $"{request.FirstName} {request.LastName}",
            EmployeeId = employee.Id  // Vincular al Employee creado
        };

        var result = await _userManager.CreateAsync(user, request.Password);

        if (!result.Succeeded)
        {
            // Si falla la creación del usuario, eliminar el empleado creado
            _context.Employees.Remove(employee);
            await _context.SaveChangesAsync();
            
            // Log detallado de los errores
            var errors = string.Join(", ", result.Errors.Select(e => $"{e.Code}: {e.Description}"));
            Console.WriteLine($"❌ Error al crear usuario: {errors}");
            
            return (false, errors);
        }

        await _userManager.AddToRoleAsync(user, "User");

        return (true, null);
    }
}
