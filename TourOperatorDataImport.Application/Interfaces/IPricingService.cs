using TourOperatorDataImport.Core.Dtos;
using TourOperatorDataImport.Core.Entities;

namespace TourOperatorDataImport.Application.Interfaces;

public interface IPricingService
{
    Task ProcessPricingFileAsync(int tourOperatorId, Stream fileStream, string? connectionId = null);
    Task<PagedResult<PricingRecord>> GetPricingDataAsync(int tourOperatorId, int page, int pageSize);
}