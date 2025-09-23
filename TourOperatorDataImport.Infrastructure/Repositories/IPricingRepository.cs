using Microsoft.Extensions.Caching.Distributed;
using TourOperatorDataImport.Core.Entities;

namespace TourOperatorDataImport.Infrastructure.Repositories;

public interface IPricingRepository
{
    Task BulkInsertAsync(IEnumerable<PricingRecord> records, Func<int, int, Task>? progressCallback = null);
    Task BulkInsertBatchAsync(IEnumerable<PricingRecord> records, int batchSize = 1000, Func<int, int, int, Task>? progressCallback = null);
    Task<IEnumerable<PricingRecord>> GetByTourOperatorAsync(int tourOperatorId, int page, int pageSize);
    Task<int> GetCountByTourOperatorAsync(int tourOperatorId);
    Task ClearCacheAsync(IDistributedCache cache, int tourOperatorId);
}