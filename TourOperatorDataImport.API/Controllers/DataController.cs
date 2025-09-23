using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using TourOperatorDataImport.Application.Interfaces;
using TourOperatorDataImport.Core.Dtos;
using TourOperatorDataImport.Core.Entities;

namespace TourOperatorDataImport.API.Controllers;

[ApiController]
[Route("api/data/{tourOperatorId}")]
public class DataController(
    IPricingService pricingService,
    IDistributedCache cache,
    ILogger<DataController> logger)
    : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetPricingData(int tourOperatorId, [FromQuery] int page = 1, [FromQuery] int pageSize = 100)
    {
        var cacheKey = $"pricing_{tourOperatorId}_page{page}_size{pageSize}";
        
        try
        {
            var cachedData = await cache.GetStringAsync(cacheKey);
            if (!string.IsNullOrEmpty(cachedData))
            {
                logger.LogInformation("Cache hit for {CacheKey}", cacheKey);
                return Ok(JsonSerializer.Deserialize<PagedResult<PricingRecord>>(cachedData));
            }

            var result = await pricingService.GetPricingDataAsync(tourOperatorId, page, pageSize);

            var cacheOptions = new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5)
            };
            
            await cache.SetStringAsync(cacheKey, JsonSerializer.Serialize(result), cacheOptions);
            
            return Ok(result);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error retrieving data for tour operator {TourOperatorId}", tourOperatorId);
            return StatusCode(500, new { error = ex.Message });
        }
    }
}