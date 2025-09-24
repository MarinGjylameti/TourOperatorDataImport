using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using TourOperatorDataImport.Application.Interfaces;
using TourOperatorDataImport.Core.Entities;
using TourOperatorDataImport.Core.Enums;
using TourOperatorDataImport.Infrastructure.Data;

namespace TourOperatorDataImport.Application.Features.Auth.Commands.Register;

public class RegisterCommandHandler(IJwtService jwtService, ILogger<RegisterCommandHandler> logger, ApplicationDbContext context)
    : IRequestHandler<RegisterCommand, RegisterResponse>
{
    public async Task<RegisterResponse> Handle(RegisterCommand request, CancellationToken cancellationToken)
    {
        try
        {
            logger.LogInformation("Registration attempt for user: {Username}", request.Username);
            
            var result = await RegisterAsync(
                request.Username,
                request.Email,
                request.Password,
                request.Role,
                request.TourOperatorId);

            logger.LogInformation("Registration successful for user: {Username}", request.Username);
            
            return new RegisterResponse(result.Token, result.User);
        }
        catch (InvalidOperationException ex)
        {
            logger.LogWarning("Registration failed for user {Username}: {Message}", 
                request.Username, ex.Message);
            throw;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unexpected error during registration for user {Username}", request.Username);
            throw;
        }
    }

    private async Task<(string Token, object User)> RegisterAsync(string username, string email, string password, Role role, int? tourOperatorId)
    {
        if (await UserExistsAsync(username, email))
        {
            throw new InvalidOperationException("Username or email already exists");
        }

        var user = new User
        {
            Username = username,
            Email = email,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(password),
            Role = role,
            TourOperatorId = role == Role.TourOperator ? tourOperatorId : null
        };

        context.Users.Add(user);
        await context.SaveChangesAsync();

        var token = jwtService.GenerateToken(user.Username, user.Role.ToString(), user.TourOperatorId);
        var userDto = new { user.Id, user.Username, user.Email, user.Role, user.TourOperatorId };

        return (token, userDto);
    }

    private async Task<bool> UserExistsAsync(string username, string email)
    {
        return await context.Users.AnyAsync(u => u.Username == username || u.Email == email);
    }
}