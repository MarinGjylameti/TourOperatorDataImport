using FluentValidation;
using MediatR;
using TourOperatorDataImport.Core.Dtos;
using TourOperatorDataImport.Core.Entities;

namespace TourOperatorDataImport.Application.Features.Pricing.Queries;

public record GetPricingDataQuery(int TourOperatorId, int Page, int PageSize) 
    : IRequest<PagedResult<PricingRecord>>;

