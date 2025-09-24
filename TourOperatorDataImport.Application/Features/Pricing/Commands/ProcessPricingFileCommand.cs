using MediatR;

namespace TourOperatorDataImport.Application.Features.Pricing.Commands;

public record ProcessPricingFileCommand(
    int TourOperatorId, 
    Stream FileStream, 
    string? ConnectionId = null) : IRequest<ProcessPricingFileResponse>;