namespace TourOperatorDataImport.Core.Entities;

public class PricingRecord
{
    public int Id { get; set; }
    public int TourOperatorId { get; set; }
    public string RouteCode { get; set; } = string.Empty;
    public string SeasonCode { get; set; } = string.Empty;
    public DateTime Date { get; set; }
    public decimal EconomyPrice { get; set; }
    public decimal BusinessPrice { get; set; }
    public int EconomySeats { get; set; }
    public int BusinessSeats { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}