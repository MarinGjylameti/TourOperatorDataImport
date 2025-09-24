using FluentValidation;
using MediatR;
using TourOperatorDataImport.Application.Behaviors;
using TourOperatorDataImport.Application.Features.Auth.Commands.Login;

namespace TourOperatorDataImport.API.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddMediatR(cfg => 
        {
            cfg.RegisterServicesFromAssembly(typeof(LoginCommand).Assembly);
        });

        services.AddValidatorsFromAssembly(typeof(LoginCommand).Assembly);

        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>));
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));

        return services;
    }
}