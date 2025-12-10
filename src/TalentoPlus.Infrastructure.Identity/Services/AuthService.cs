using Microsoft.AspNetCore.Identity;
using TalentoPlus.Application.Auth;
using TalentoPlus.Infrastructure.Identity.Identity;

namespace TalentoPlus.Infrastructure.Identity.Services;

public class AuthService
{
    private readonly UserManager<AppUser> _userManager;
    private readonly JwtService _jwtService;

    public AuthService(UserManager<AppUser> userManager, JwtService jwtService)
    {
        _userManager = userManager;
        _jwtService = jwtService;
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

    public async Task<bool> RegisterAsync(RegisterRequest request)
    {
        var user = new AppUser
        {
            Email = request.Email,
            UserName = request.Email,
            FullName = request.FullName
        };

        var result = await _userManager.CreateAsync(user, request.Password);

        if (!result.Succeeded)
            return false;

        await _userManager.AddToRoleAsync(user, "User");

        return true;
    }
}
