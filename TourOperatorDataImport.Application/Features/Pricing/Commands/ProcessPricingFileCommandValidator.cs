using FluentValidation;

namespace TourOperatorDataImport.Application.Features.Pricing.Commands;

public class ProcessPricingFileCommandValidator : AbstractValidator<ProcessPricingFileCommand>
{
    public ProcessPricingFileCommandValidator()
    {
        RuleFor(x => x.FileStream).NotNull().WithMessage("File stream is required");
        RuleFor(x => x.FileStream.Length).GreaterThan(0).WithMessage("File cannot be empty");
        RuleFor(x => x.ConnectionId).NotNull()
            .WithMessage(
                "ConnectionId is required. You can find connection ID in the html page (/ui/CustomSwaggerIndex.html) where you can also see the live notification from SignalR");
    }
}