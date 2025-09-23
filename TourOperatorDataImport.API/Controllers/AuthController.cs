using Microsoft.AspNetCore.Mvc;
using TourOperatorDataImport.Core.Enums;
using Microsoft.AspNetCore.Authorization;
using TourOperatorDataImport.Application.Interfaces;

namespace TourOperatorDataImport.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController(IAuthService authService, ILogger<AuthController> logger)
    : ControllerBase
{
    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginRequest request)
    {
        try
        {
            var token = await authService.LoginAsync(request.Username, request.Password);
            return Ok(new { token });
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(new { error = ex.Message });
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error during login");
            return StatusCode(500, new { error = "Internal server error" });
        }
    }

    [HttpPost("register")]
    [AllowAnonymous]
    public async Task<IActionResult> Register(RegisterRequest request)
    {
        try
        {
            var result = await authService.RegisterAsync(
                request.Username, 
                request.Email, 
                request.Password, 
                request.Role, 
                request.TourOperatorId);

            return Ok(new { token = result.Token, user = result.User });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error during registration");
            return StatusCode(500, new { error = "Internal server error" });
        }
    }

    [HttpPost("logout")]
    public IActionResult Logout()
    {
        return Ok(new { message = "Logged out successfully" });
    }
}

public record LoginRequest(string Username, string Password);
public record RegisterRequest(string Username, string Email, string Password, Role Role, int? TourOperatorId);