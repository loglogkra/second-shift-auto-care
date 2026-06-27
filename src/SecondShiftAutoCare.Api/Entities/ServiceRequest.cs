using SecondShiftAutoCare.Shared.Models;

namespace SecondShiftAutoCare.Api.Entities;

public sealed class ServiceRequest
{
    public Guid Id { get; set; }
    public Guid CustomerId { get; set; }
    public Customer Customer { get; set; } = null!;
    public Guid VehicleId { get; set; }
    public Vehicle Vehicle { get; set; } = null!;
    public string ServiceType { get; set; } = string.Empty;
    public string? ServiceSpecificAnswers { get; set; }
    public string? Symptoms { get; set; }
    public string? Notes { get; set; }
    public string? PreferredAvailability { get; set; }
    public string UrgencyLevel { get; set; } = ServiceRequestUrgencyLevels.Routine;
    public string? IsVehicleDrivable { get; set; }
    public string? VehicleLocation { get; set; }
    public bool ConsentAccepted { get; set; }
    public bool WantsPhotoUploadLater { get; set; }
    public bool IsArchived { get; set; }
    public string Status { get; set; } = ServiceRequestStatuses.New;
    public string? InternalNotes { get; set; }
    public DateTime CreatedUtc { get; set; }
    public DateTime UpdatedUtc { get; set; }
    public Quote? Quote { get; set; }
}
