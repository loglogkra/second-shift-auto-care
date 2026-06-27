using SecondShiftAutoCare.Shared.Models;

namespace SecondShiftAutoCare.Api.Entities;

public sealed class Quote
{
    public Guid Id { get; set; }
    public Guid ServiceRequestId { get; set; }
    public ServiceRequest ServiceRequest { get; set; } = null!;
    public decimal? EstimateLow { get; set; }
    public decimal? EstimateHigh { get; set; }
    public decimal? LaborAmount { get; set; }
    public decimal? PartsAmount { get; set; }
    public decimal? ShopSuppliesAmount { get; set; }
    public decimal? TotalEstimate { get; set; }
    public string? PartsNeeded { get; set; }
    public string? InternalQuoteNotes { get; set; }
    public string? QuoteTemplate { get; set; }
    public string? AssumptionDisclaimerText { get; set; }
    public string? GoodOption { get; set; }
    public string? BetterOption { get; set; }
    public string? BestOption { get; set; }
    public string CustomerApprovalStatus { get; set; } = ServiceRequestApprovalStatuses.Pending;
    public DateTime? QuoteExpirationDate { get; set; }
    public DateTime CreatedUtc { get; set; }
    public DateTime UpdatedUtc { get; set; }
}
