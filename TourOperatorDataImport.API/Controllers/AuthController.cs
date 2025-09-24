using MediatR;
using Microsoft.AspNetCore.Mvc;
using TourOperatorDataImport.Core.Enums;
using Microsoft.AspNetCore.Authorization;
using TourOperatorDataImport.Application.Features.Auth.Commands.Login;
using TourOperatorDataImport.Application.Features.Auth.Commands.Register;

namespace TourOperatorDataImport.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController(IMediator mediator, ILogger<AuthController> logger) : ControllerBase
{
    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginRequest request)
    {
        try
        {
            var command = new LoginCommand(request.Username, request.Password);
            var result = await mediator.Send(command);
            return Ok(new { token = result.Token });
        }
        catch (UnauthorizedAccessException ex)
        {
            logger.LogWarning("Login failed for {Username}: {Message}", request.Username, ex.Message);
            return Unauthorized(new { error = ex.Message });
        }
        catch (FluentValidation.ValidationException ex)
        {
            logger.LogWarning("Validation failed during login: {Errors}", ex.Message);
            return BadRequest(new { errors = ex.Errors.Select(e => e.ErrorMessage) });
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error during login for user {Username}", request.Username);
            return StatusCode(500, new { error = "Internal server error" });
        }
    }

    [HttpPost("register")]
    [AllowAnonymous]
    public async Task<IActionResult> Register(RegisterRequest request)
    {
        try
        {
            var command = new RegisterCommand(
                request.Username, 
                request.Email, 
                request.Password, 
                request.Role, 
                request.TourOperatorId);

            var result = await mediator.Send(command);
            
            return Ok(new { token = result.Token, user = result.User });
        }
        catch (InvalidOperationException ex)
        {
            logger.LogWarning("Registration failed: {Message}", ex.Message);
            return BadRequest(new { error = ex.Message });
        }
        catch (FluentValidation.ValidationException ex)
        {
            logger.LogWarning("Validation failed during registration: {Errors}", ex.Message);
            return BadRequest(new { errors = ex.Errors.Select(e => e.ErrorMessage) });
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error during registration for user {Username}", request.Username);
            return StatusCode(500, new { error = "Internal server error" });
        }
    }

    [HttpPost("logout")]
    public IActionResult Logout()
    {
        logger.LogInformation("User logged out");
        return Ok(new { message = "Logged out successfully" });
    }
}

public record LoginRequest(string Username, string Password);
public record RegisterRequest(string Username, string Email, string Password, Role Role, int? TourOperatorId);