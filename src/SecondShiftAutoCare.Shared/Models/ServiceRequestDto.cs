using System.ComponentModel.DataAnnotations;

namespace SecondShiftAutoCare.Shared.Models;

public sealed class ServiceRequestDto
{
    public Guid? Id { get; set; }

    [Required, StringLength(100)]
    public string CustomerName { get; set; } = string.Empty;

    [Required, Phone, StringLength(25)]
    public string Phone { get; set; } = string.Empty;

    [EmailAddress, StringLength(254)]
    public string? Email { get; set; }

    [Range(1900, 2100)]
    public int? VehicleYear { get; set; }

    [Required, StringLength(60)]
    public string VehicleMake { get; set; } = string.Empty;

    [Required, StringLength(60)]
    public string VehicleModel { get; set; } = string.Empty;

    [Range(0, 999999)]
    public int? Mileage { get; set; }

    [Required, StringLength(80)]
    public string ServiceType { get; set; } = string.Empty;

    [Required, StringLength(2000)]
    public string Symptoms { get; set; } = string.Empty;

    [Required, StringLength(500)]
    public string PreferredAvailability { get; set; } = string.Empty;

    [Range(typeof(bool), "true", "true", ErrorMessage = "Consent is required before submitting a request.")]
    public bool ConsentAccepted { get; set; }

    public DateTimeOffset? SubmittedAt { get; set; }
    public string Status { get; set; } = ServiceRequestStatuses.New;
    public decimal? QuoteAmount { get; set; }
    public string? QuoteNotes { get; set; }
    public string? InternalNotes { get; set; }
    public DateTimeOffset? UpdatedAt { get; set; }
}
