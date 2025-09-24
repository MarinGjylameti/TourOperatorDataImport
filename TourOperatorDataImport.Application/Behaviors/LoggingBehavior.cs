using MediatR;
using Microsoft.Extensions.Logging;

namespace TourOperatorDataImport.Application.Behaviors;

public class LoggingBehavior<TRequest, TResponse>(ILogger<LoggingBehavior<TRequest, TResponse>> logger)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    public async Task<TResponse> Handle(
        TRequest request, 
        RequestHandlerDelegate<TResponse> next, 
        CancellationToken cancellationToken)
    {
        var requestName = typeof(TRequest).Name;
        
        logger.LogInformation("Handling {RequestName}", requestName);

        try
        {
            var response = await next(cancellationToken);
            
            logger.LogInformation("Handled {RequestName} successfully", requestName);
            
            return response;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error handling {RequestName}", requestName);
            throw;
        }
    }
}