using MediatR;
using TourOperatorDataImport.Core.Enums;

namespace TourOperatorDataImport.Application.Features.Auth.Commands.Register;

public record RegisterCommand(
    string Username, 
    string Email, 
    string Password, 
    Role Role, 
    int? TourOperatorId) : IRequest<RegisterResponse>;

