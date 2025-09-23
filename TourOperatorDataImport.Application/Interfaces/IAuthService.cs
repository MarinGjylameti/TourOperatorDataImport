using TourOperatorDataImport.Core.Enums;

namespace TourOperatorDataImport.Application.Interfaces;

public interface IAuthService
{    
    Task<string> LoginAsync(string username, string password);
    Task<(string Token, object User)> RegisterAsync(string username, string email, string password, Role role, int? tourOperatorId);
    Task<bool> UserExistsAsync(string username, string email);
}