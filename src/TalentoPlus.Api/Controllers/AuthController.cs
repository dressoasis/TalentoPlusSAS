using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using TalentoPlus.Application.Auth;
using TalentoPlus.Infrastructure.Identity.Services;
using TalentoPlus.Infrastructure.Integrations.Email;

namespace TalentoPlus.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly AuthService _authService;
    private readonly IEmailService _emailService;
    private readonly ILogger<AuthController> _logger;

    public AuthController(
        AuthService authService,
        IEmailService emailService,
        ILogger<AuthController> logger)
    {
        _authService = authService;
        _emailService = emailService;
        _logger = logger;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginRequest request)
    {
        var result = await _authService.LoginAsync(request);

        if (result == null)
            return Unauthorized(new { message = "Credenciales incorrectas." });

        return Ok(result);
    }

    [HttpPost("register")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Register(RegisterRequest request)
    {
        _logger.LogInformation("Intentando registrar usuario: {Email}", request.Email);
        
        var (success, errorMessage) = await _authService.RegisterAsync(request);

        if (!success)
        {
            _logger.LogWarning("Fallo al registrar usuario {Email}: {Error}", request.Email, errorMessage);
            return BadRequest(new { error = errorMessage ?? "No se pudo crear el usuario." });
        }

        // Enviar correo de bienvenida
        try
        {
            var fullName = $"{request.FirstName} {request.LastName}";
            await _emailService.SendWelcomeEmailAsync(request.Email, fullName);
            _logger.LogInformation("Correo de bienvenida enviado a {Email}", request.Email);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al enviar correo de bienvenida a {Email}", request.Email);
            // No fallar el registro si el correo falla
        }

        return Ok("Usuario creado exitosamente.");
    }
}
