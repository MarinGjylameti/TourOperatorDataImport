using MediatR;
using Microsoft.EntityFrameworkCore;
using TourOperatorDataImport.Infrastructure.Data;

namespace TourOperatorDataImport.Application.Features.Auth.Queries;

public class UserExistsQueryHandler(ApplicationDbContext context) : IRequestHandler<UserExistsQuery, bool>
{
    public async Task<bool> Handle(UserExistsQuery request, CancellationToken cancellationToken)
    {
        return await context.Users.AnyAsync(u => u.Username == request.Username || u.Email == request.Email, cancellationToken: cancellationToken);
    }
}