using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using TourOperatorDataImport.Application.Interfaces;

namespace TourOperatorDataImport.API.Controllers;

[ApiController]
[Route("api/touroperators/{tourOperatorId}/pricing-upload")]
public class PricingController(
    IPricingService pricingService,
    ILogger<PricingController> logger)
    : ControllerBase
{
    [Authorize(Roles = "TourOperator")]
    [HttpPost]
    public async Task<IActionResult> UploadPricingData(int tourOperatorId, IFormFile file, string connectionId)
    {
        if (file.Length == 0)
            return BadRequest("No file uploaded");

        try
        {
            await using var stream = file.OpenReadStream();
            await pricingService.ProcessPricingFileAsync(tourOperatorId, stream, connectionId);

            return Ok(new
            {
                message = "File processed successfully",
                connectionId = connectionId
            });
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error processing file for tour operator {TourOperatorId}", tourOperatorId);
            return StatusCode(500, new { error = ex.Message });
        }
    }
}