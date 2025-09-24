using TourOperatorDataImport.Application.Hubs;

namespace TourOperatorDataImport.API.Extensions;

public static class HubRegistrationExtensions
{
    public static IServiceCollection AddApplicationHubs(this IServiceCollection services)
    {
        services.AddSignalR();
        return services;
    }

    public static WebApplication MapApplicationHubs(this WebApplication app)
    {
        app.MapHub<ProgressHub>("/progressHub");
        return app;
    }
}