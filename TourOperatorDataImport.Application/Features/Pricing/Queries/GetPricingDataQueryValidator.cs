using FluentValidation;

namespace TourOperatorDataImport.Application.Features.Pricing.Queries;

public class GetPricingDataQueryValidator : AbstractValidator<GetPricingDataQuery>
{
    public GetPricingDataQueryValidator()
    {
        RuleFor(x => x.TourOperatorId).GreaterThan(0);
        RuleFor(x => x.Page).GreaterThan(0);
        RuleFor(x => x.PageSize).InclusiveBetween(1, 1000);
    }
}