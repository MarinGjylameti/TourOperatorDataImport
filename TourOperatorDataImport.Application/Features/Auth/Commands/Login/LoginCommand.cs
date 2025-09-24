using MediatR;

namespace TourOperatorDataImport.Application.Features.Auth.Commands.Login;

public record LoginCommand(string Username, string Password) : IRequest<LoginResponse>;