using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using TourOperatorDataImport.Application.Interfaces;
using TourOperatorDataImport.Core.Entities;
using TourOperatorDataImport.Core.Enums;
using TourOperatorDataImport.Infrastructure.Data;

namespace TourOperatorDataImport.Application.Services;

public class AuthService(
    ApplicationDbContext context, 
    IJwtService jwtService,
    ILogger<AuthService> logger) : IAuthService
{
    public async Task<string> LoginAsync(string username, string password)
    {
        var user = await context.Users
            .FirstOrDefaultAsync(u => u.Username == username);

        if (user == null || !BCrypt.Net.BCrypt.Verify(password, user.PasswordHash))
        {
            throw new UnauthorizedAccessException("Invalid username or password");
        }

        return jwtService.GenerateToken(user.Username, user.Role.ToString(), user.TourOperatorId);
    }

    public async Task<(string Token, object User)> RegisterAsync(string username, string email, string password, Role role, int? tourOperatorId)
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

    public async Task<bool> UserExistsAsync(string username, string email)
    {
        return await context.Users.AnyAsync(u => u.Username == username || u.Email == email);
    }
}