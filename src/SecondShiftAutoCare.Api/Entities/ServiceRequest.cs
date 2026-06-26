using SecondShiftAutoCare.Shared.Models;

namespace SecondShiftAutoCare.Api.Entities;

public sealed class ServiceRequest
{
    public Guid Id { get; set; }
    public string CustomerName { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string? Email { get; set; }
    public int VehicleYear { get; set; }
    public string VehicleMake { get; set; } = string.Empty;
    public string VehicleModel { get; set; } = string.Empty;
    public int? Mileage { get; set; }
    public string ServiceType { get; set; } = string.Empty;
    public string? ServiceSpecificAnswers { get; set; }
    public string Symptoms { get; set; } = string.Empty;
    public string? PreferredAvailability { get; set; }
    public string UrgencyLevel { get; set; } = ServiceRequestUrgencyLevels.Routine;
    public string? IsVehicleDrivable { get; set; }
    public string? VehicleLocation { get; set; }
    public string? AlternateContactName { get; set; }
    public string? AlternateContactPhone { get; set; }
    public bool ConsentAccepted { get; set; }
    public bool WantsPhotoUploadLater { get; set; }
    public bool IsArchived { get; set; }
    public string Status { get; set; } = ServiceRequestStatuses.New;
    public decimal? EstimateLow { get; set; }
    public decimal? EstimateHigh { get; set; }
    public string? PartsNeeded { get; set; }
    public string? InternalQuoteNotes { get; set; }
    public decimal? LaborAmount { get; set; }
    public decimal? PartsAmount { get; set; }
    public decimal? ShopSuppliesAmount { get; set; }
    public decimal? TotalEstimate { get; set; }
    public string? QuoteTemplate { get; set; }
    public string? AssumptionDisclaimerText { get; set; }
    public string? GoodOption { get; set; }
    public string? BetterOption { get; set; }
    public string? BestOption { get; set; }
    public DateTime? QuoteExpirationDate { get; set; }
    public string CustomerApprovalStatus { get; set; } = ServiceRequestApprovalStatuses.Pending;
    public string? InternalNotes { get; set; }
    public DateTime CreatedUtc { get; set; }
    public DateTime UpdatedUtc { get; set; }
}
