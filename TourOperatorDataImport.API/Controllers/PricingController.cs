using System.Security.Claims;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TourOperatorDataImport.Application.Features.Pricing.Commands;

namespace TourOperatorDataImport.API.Controllers;

[ApiController]
[Route("api/pricing-upload")]
public class PricingController(
    IMediator mediator,
    ILogger<PricingController> logger)
    : ControllerBase
{
    [Authorize(Roles = "TourOperator")]
    [HttpPost]
    public async Task<IActionResult> UploadPricingData(IFormFile file, string connectionId)
    {
        var tourOperatorIdClaim = User.FindFirst("TourOperatorId")?.Value;
        if (!int.TryParse(tourOperatorIdClaim, out var tourOperatorId))
        {
            return Unauthorized("TourOperatorId claim missing or invalid.");
        }

        try
        {
            await using var stream = file.OpenReadStream();
            
            var command = new ProcessPricingFileCommand(tourOperatorId, stream, connectionId);
            var result = await mediator.Send(command);

            return Ok(new
            {
                message = result.Message,
                connectionId = result.ConnectionId
            });
        }
        catch (FluentValidation.ValidationException ex)
        {
            logger.LogWarning("Validation failed for pricing upload: {Errors}", ex.Message);
            return BadRequest(new { errors = ex.Errors.Select(e => e.ErrorMessage) });
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error processing file for tour operator {TourOperatorId}", tourOperatorId);
            return StatusCode(500, new { error = ex.Message });
        }
    }
}