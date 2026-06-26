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
    public string Symptoms { get; set; } = string.Empty;
    public string? PreferredAvailability { get; set; }
    public string Status { get; set; } = ServiceRequestStatuses.New;
    public decimal? EstimateLow { get; set; }
    public decimal? EstimateHigh { get; set; }
    public string? PartsNeeded { get; set; }
    public string? InternalNotes { get; set; }
    public DateTime CreatedUtc { get; set; }
    public DateTime UpdatedUtc { get; set; }
}
