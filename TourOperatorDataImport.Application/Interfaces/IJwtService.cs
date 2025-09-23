namespace TourOperatorDataImport.Application.Interfaces;

public interface IJwtService
{
    string GenerateToken(string username, string role, int? tourOperatorId);
}