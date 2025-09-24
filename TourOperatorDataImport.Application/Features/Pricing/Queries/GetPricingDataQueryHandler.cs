using MediatR;
using Microsoft.Extensions.Logging;
using TourOperatorDataImport.Application.Interfaces;
using TourOperatorDataImport.Core.Dtos;
using TourOperatorDataImport.Core.Entities;
using TourOperatorDataImport.Infrastructure.Repositories;

namespace TourOperatorDataImport.Application.Features.Pricing.Queries;

public class GetPricingDataQueryHandler(
    IPricingRepository pricingRepository,
    ILogger<GetPricingDataQueryHandler> logger)
    : IRequestHandler<GetPricingDataQuery, PagedResult<PricingRecord>>
{
    public async Task<PagedResult<PricingRecord>> Handle(GetPricingDataQuery request, CancellationToken cancellationToken)
    {
        try
        {
            logger.LogInformation(
                "Retrieving pricing data for tour operator {TourOperatorId}, Page: {Page}, PageSize: {PageSize}",
                request.TourOperatorId, request.Page, request.PageSize);

            var result = await GetPricingDataAsync(
                request.TourOperatorId, 
                request.Page, 
                request.PageSize);

            logger.LogInformation(
                "Retrieved {Count} pricing records for tour operator {TourOperatorId}",
                result.Items.Count(), request.TourOperatorId);

            return result;
        }
        catch (Exception ex)
        {
            logger.LogError(
                ex, 
                "Error retrieving pricing data for tour operator {TourOperatorId}", 
                request.TourOperatorId);
            throw;
        }
    }

    private async Task<PagedResult<PricingRecord>> GetPricingDataAsync(int tourOperatorId, int page, int pageSize)
    {
        var records = await pricingRepository.GetByTourOperatorAsync(tourOperatorId, page, pageSize);
        var totalCount = await pricingRepository.GetCountByTourOperatorAsync(tourOperatorId);

        return new PagedResult<PricingRecord>
        {
            Page = page,
            PageSize = pageSize,
            TotalCount = totalCount,
            Items = records
        };
    }

}