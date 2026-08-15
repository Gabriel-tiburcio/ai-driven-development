using AllStay.Application.DTOs;
using AllStay.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace AllStay.Api.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController(IAuthService authService) : ControllerBase
{
    [HttpPost("login")]
    public async Task<ActionResult<LoginResponse>> Login(LoginRequest request, CancellationToken ct)
    {
        var result = await authService.LoginAsync(request, ct);
        if (result is null) return Unauthorized(new { message = "Invalid email or password." });
        return Ok(result);
    }
}
