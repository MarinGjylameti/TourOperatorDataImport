using MediatR;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using TourOperatorDataImport.Application.Hubs;
using TourOperatorDataImport.Core.Entities;
using TourOperatorDataImport.Infrastructure.Repositories;

namespace TourOperatorDataImport.Application.Features.Pricing.Commands;

public class ProcessPricingFileCommandHandler(
    IHubContext<ProgressHub> hubContext,
    IPricingRepository pricingRepository,
    ILogger<ProcessPricingFileCommandHandler> logger)
    : IRequestHandler<ProcessPricingFileCommand, ProcessPricingFileResponse>
{
    public async Task<ProcessPricingFileResponse> Handle(ProcessPricingFileCommand request, CancellationToken cancellationToken)
    {
        try
        {
            logger.LogInformation(
                "Starting pricing file processing for tour operator {TourOperatorId}", 
                request.TourOperatorId);

            await ProcessPricingFileAsync(
                request.TourOperatorId, 
                request.FileStream, 
                request.ConnectionId);

            logger.LogInformation(
                "Pricing file processing completed for tour operator {TourOperatorId}", 
                request.TourOperatorId);

            return new ProcessPricingFileResponse("File processed successfully", request.ConnectionId ?? string.Empty);
        }
        catch (Exception ex)
        {
            logger.LogError(
                ex, 
                "Error processing pricing file for tour operator {TourOperatorId}", 
                request.TourOperatorId);
            throw;
        }
    }

    private async Task ProcessPricingFileAsync(int tourOperatorId, Stream fileStream, string? connectionId = null)
    {
        await ReportProgressAsync("🔍 Starting CSV file validation...", connectionId);

        var records = await ParseCsvFileAsync(fileStream, tourOperatorId, connectionId);

        await ReportProgressAsync($"📊 CSV parsing completed: {records.Count} valid records found", connectionId);
        await ReportProgressAsync("💾 Starting database insertion...", connectionId);

        async Task BatchProgressCallback(int currentBatch, int totalBatches, int recordsProcessed)
        {
            var percentage = (double)recordsProcessed / records.Count * 100;
            var message = $"📦 Batch {currentBatch}/{totalBatches} ({recordsProcessed}/{records.Count} records, {percentage:F1}%)";
            await ReportProgressAsync(message, connectionId);
        }

        await pricingRepository.BulkInsertBatchAsync(records, batchSize: 1000, BatchProgressCallback);

        await ReportProgressAsync("✅ Upload completed successfully!", connectionId);
        
        logger.LogInformation("File processed successfully for tour operator {TourOperatorId}. {RecordCount} records inserted.",
            tourOperatorId, records.Count);
    }

    private async Task<List<PricingRecord>> ParseCsvFileAsync(Stream fileStream, int tourOperatorId, string? connectionId)
    {
        var records = new List<PricingRecord>();
        using var reader = new StreamReader(fileStream);
        
        var lines = new List<string>();
        while (await reader.ReadLineAsync() is { } line)
        {
            lines.Add(line);
        }

        var totalLines = lines.Count - 1; 
        var processedCount = 0;
        var errors = new List<string>();

        for (int i = 1; i < lines.Count; i++)
        {
            processedCount++;
            var line = lines[i];

            try
            {
                var record = ParseCsvLine(line, tourOperatorId, processedCount, errors);
                if (record != null)
                {
                    records.Add(record);
                }

                if (processedCount % 1000 == 0 && !string.IsNullOrEmpty(connectionId))
                {
                    var progressPercentage = (double)processedCount / totalLines * 100;
                    await ReportProgressAsync($"{processedCount}/{totalLines} lines processed ({progressPercentage:F2}%)", connectionId);
                }
            }
            catch (Exception ex)
            {
                errors.Add($"Line {processedCount}: Error parsing - {ex.Message}");
                logger.LogWarning(ex, "Error parsing line {LineNumber}: {Line}", processedCount, line);
            }
        }

        if (errors.Any())
        {
            await ReportProgressAsync($"Completed with {errors.Count} errors. Check logs for details.", connectionId);
            logger.LogWarning("CSV parsing completed with {ErrorCount} errors", errors.Count);
        }

        return records;
    }

    private PricingRecord? ParseCsvLine(string line, int tourOperatorId, int lineNumber, List<string> errors)
    {
        var values = line.Split(',');

        if (values.Length != 7)
        {
            errors.Add($"Line {lineNumber}: Expected 7 columns, got {values.Length}");
            return null;
        }

        var record = new PricingRecord
        {
            TourOperatorId = tourOperatorId,
            RouteCode = values[0].Trim(),
            SeasonCode = values[1].Trim(),
            Date = DateTime.Parse(values[2].Trim()),
            EconomyPrice = decimal.Parse(values[3].Trim()),
            BusinessPrice = decimal.Parse(values[4].Trim()),
            EconomySeats = int.Parse(values[5].Trim()),
            BusinessSeats = int.Parse(values[6].Trim()),
            CreatedAt = DateTime.UtcNow
        };

        if (record.EconomyPrice < 0 || record.BusinessPrice < 0)
        {
            errors.Add($"Line {lineNumber}: Prices cannot be negative");
            return null;
        }

        if (record.EconomySeats < 0 || record.BusinessSeats < 0)
        {
            errors.Add($"Line {lineNumber}: Seat counts cannot be negative");
            return null;
        }

        return record;
    }

    private async Task ReportProgressAsync(string message, string? connectionId)
    {
        if (!string.IsNullOrEmpty(connectionId))
        {
            await hubContext.Clients.All.SendAsync($"ReceiveProgress", message);
        }
    }
}