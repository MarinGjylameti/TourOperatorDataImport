using MediatR;

namespace TourOperatorDataImport.Application.Features.Auth.Queries;

public record UserExistsQuery(string Username, string Email) : IRequest<bool>;