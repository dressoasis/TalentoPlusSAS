using Microsoft.AspNetCore.Mvc;
using TalentoPlus.Application.Auth;
using TalentoPlus.Infrastructure.Identity.Services;

namespace TalentoPlus.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly AuthService _authService;

    public AuthController(AuthService authService)
    {
        _authService = authService;
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
    public async Task<IActionResult> Register(RegisterRequest request)
    {
        var ok = await _authService.RegisterAsync(request);

        if (!ok)
            return BadRequest("No se pudo crear el usuario.");

        return Ok("Usuario creado.");
    }
}
