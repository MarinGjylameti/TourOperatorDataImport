using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using TourOperatorDataImport.Application.Interfaces;
using TourOperatorDataImport.Infrastructure.Data;

namespace TourOperatorDataImport.Application.Features.Auth.Commands.Login;

public class LoginCommandHandler(IJwtService jwtService,ILogger<LoginCommandHandler> logger, ApplicationDbContext context, IConfiguration configuration)
    : IRequestHandler<LoginCommand, LoginResponse>
{
    public async Task<LoginResponse> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        try
        {
            logger.LogInformation("Login attempt for user: {Username}", request.Username);
            
            var token = await LoginAsync(request.Username, request.Password);
            
            logger.LogInformation("Login successful for user: {Username}", request.Username);
            
            return new LoginResponse(token);
        }
        catch (UnauthorizedAccessException ex)
        {
            logger.LogWarning("Login failed for user {Username}: {Message}", 
                request.Username, ex.Message);
            throw;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unexpected error during login for user {Username}", request.Username);
            throw;
        }
    }

    private async Task<string> LoginAsync(string username, string password)
    {
        var user = await context.Users
            .FirstOrDefaultAsync(u => u.Username == username);

        if (user == null || !BCrypt.Net.BCrypt.Verify(password, user.PasswordHash))
        {
            throw new UnauthorizedAccessException("Invalid username or password");
        }

        return jwtService.GenerateToken(user.Username, user.Role.ToString(), user.TourOperatorId);
    }
}