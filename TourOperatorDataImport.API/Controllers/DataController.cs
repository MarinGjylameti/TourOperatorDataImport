using System.Text.Json;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using TourOperatorDataImport.Application.Features.Pricing.Queries;
using TourOperatorDataImport.Application.Interfaces;
using TourOperatorDataImport.Core.Dtos;
using TourOperatorDataImport.Core.Entities;

namespace TourOperatorDataImport.API.Controllers;

[ApiController]
[Route("api/data/{tourOperatorId}")]
public class DataController(
    IMediator mediator,
    IDistributedCache cache,
    ILogger<DataController> logger)
    : ControllerBase
{
    [Authorize(Roles = "Admin")]
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

            var query = new GetPricingDataQuery(tourOperatorId, page, pageSize);
            var result = await mediator.Send(query);

            var cacheOptions = new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5)
            };
            
            await cache.SetStringAsync(cacheKey, JsonSerializer.Serialize(result), cacheOptions);
            
            return Ok(result);
        }
        catch (FluentValidation.ValidationException ex)
        {
            logger.LogWarning("Validation failed for pricing data query: {Errors}", ex.Message);
            return BadRequest(new { errors = ex.Errors.Select(e => e.ErrorMessage) });
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error retrieving data for tour operator {TourOperatorId}", tourOperatorId);
            return StatusCode(500, new { error = ex.Message });
        }
    }
}