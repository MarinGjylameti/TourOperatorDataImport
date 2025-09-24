using FluentValidation;
using TourOperatorDataImport.Core.Enums;

namespace TourOperatorDataImport.Application.Features.Auth.Commands.Register;

public class RegisterCommandValidator : AbstractValidator<RegisterCommand>
{
    public RegisterCommandValidator()
    {
        RuleFor(x => x.Username).NotEmpty().MinimumLength(3);
        RuleFor(x => x.Email).NotEmpty().EmailAddress();
        RuleFor(x => x.Password).NotEmpty().MinimumLength(6);
        RuleFor(x => x.Role).IsInEnum();
        RuleFor(x => x.TourOperatorId)
            .NotNull().When(x => x.Role == Role.TourOperator)
            .WithMessage("TourOperatorId is required for TourOperator role");
    }
}