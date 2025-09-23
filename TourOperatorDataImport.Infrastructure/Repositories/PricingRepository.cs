using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using TourOperatorDataImport.Core.Entities;
using TourOperatorDataImport.Infrastructure.Data;

namespace TourOperatorDataImport.Infrastructure.Repositories;

public class PricingRepository(ApplicationDbContext context, ILogger<PricingRepository> logger)
    : IPricingRepository
{
    public async Task BulkInsertAsync(IEnumerable<PricingRecord> records, Func<int, int, Task>? progressCallback = null)
    {
        await using var transaction = await context.Database.BeginTransactionAsync();
        
        try
        {
            var recordList = records.ToList();
            var recordCount = recordList.Count;
            
            if (progressCallback != null)
            {
                await progressCallback(0, recordCount);
            }
            
            await context.PricingRecords.AddRangeAsync(recordList);
            await context.SaveChangesAsync();
            await transaction.CommitAsync();
            
            if (progressCallback != null)
            {
                await progressCallback(recordCount, recordCount);
            }
            
            logger.LogInformation("Bulk insert completed. {RecordCount} records inserted.", recordCount);
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            logger.LogError(ex, "Error during bulk insert of {RecordCount} records", records.Count());
            throw;
        }
    }

    public async Task BulkInsertBatchAsync(IEnumerable<PricingRecord> records, int batchSize = 1000, Func<int, int, int, Task>? progressCallback = null)
    {
        var recordList = records.ToList();
        var totalRecords = recordList.Count;
        var totalBatches = (int)Math.Ceiling((double)totalRecords / batchSize);

        if (progressCallback != null)
        {
            await progressCallback(0, totalBatches, totalRecords);
        }

        for (int i = 0; i < totalBatches; i++)
        {
            var batch = recordList.Skip(i * batchSize).Take(batchSize).ToList();
            var batchNumber = i + 1;
            var recordsProcessed = Math.Min((i + 1) * batchSize, totalRecords);

            if (progressCallback != null)
            {
                await progressCallback(batchNumber, totalBatches, recordsProcessed);
            }

            await BulkInsertAsync(batch);
            
            logger.LogInformation("Processed batch {BatchNumber} of {TotalBatches}", batchNumber, totalBatches);

            if (batchNumber < totalBatches)
            {
                await Task.Delay(100);
            }
        }

        // Report final completion
        if (progressCallback != null)
        {
            await progressCallback(totalBatches, totalBatches, totalRecords);
        }
        
        logger.LogInformation("Batch processing completed. {TotalRecords} records inserted in {TotalBatches} batches.", totalRecords, totalBatches);
    }

    public async Task<IEnumerable<PricingRecord>> GetByTourOperatorAsync(int tourOperatorId, int page, int pageSize)
    {
        if (page < 1) page = 1;
        if (pageSize < 1) pageSize = 100;
        if (pageSize > 1000) pageSize = 1000; 
        
        var skip = (page - 1) * pageSize;
        
        return await context.PricingRecords
            .Where(r => r.TourOperatorId == tourOperatorId)
            .OrderBy(r => r.Date)
            .ThenBy(r => r.RouteCode)
            .Skip(skip)
            .Take(pageSize)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<int> GetCountByTourOperatorAsync(int tourOperatorId)
    {
        return await context.PricingRecords
            .Where(r => r.TourOperatorId == tourOperatorId)
            .CountAsync();
    }

    public async Task ClearCacheAsync(IDistributedCache cache, int tourOperatorId)
    {
        var pattern = $"pricing_{tourOperatorId}_*";
    }
}