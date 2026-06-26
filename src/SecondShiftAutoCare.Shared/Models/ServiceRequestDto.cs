using System.ComponentModel.DataAnnotations;

namespace SecondShiftAutoCare.Shared.Models;

public sealed class ServiceRequestDto
{
    public Guid? Id { get; set; }

    [Required, StringLength(100)]
    public string CustomerName { get; set; } = string.Empty;

    [Required, Phone, StringLength(30)]
    public string Phone { get; set; } = string.Empty;

    [EmailAddress, StringLength(150)]
    public string? Email { get; set; }

    [Required, Range(1900, 2100)]
    public int? VehicleYear { get; set; }

    [Required, StringLength(75)]
    public string VehicleMake { get; set; } = string.Empty;

    [Required, StringLength(75)]
    public string VehicleModel { get; set; } = string.Empty;

    [Range(0, 999999)]
    public int? Mileage { get; set; }

    [Required, StringLength(1000)]
    public string ServiceType { get; set; } = string.Empty;

    [StringLength(1000)]
    public string? ServiceSpecificAnswers { get; set; }

    [Required, StringLength(2000)]
    public string Symptoms { get; set; } = string.Empty;

    [StringLength(500)]
    public string? PreferredAvailability { get; set; }

    [StringLength(50)]
    public string UrgencyLevel { get; set; } = ServiceRequestUrgencyLevels.Routine;

    [StringLength(30)]
    public string? IsVehicleDrivable { get; set; }

    [StringLength(300)]
    public string? VehicleLocation { get; set; }

    [StringLength(200)]
    public string? AlternateContactName { get; set; }

    [StringLength(30)]
    public string? AlternateContactPhone { get; set; }

    public bool ConsentAccepted { get; set; }

    public bool WantsPhotoUploadLater { get; set; }

    public string Status { get; set; } = ServiceRequestStatuses.New;

    [Range(typeof(decimal), "0", "999999.99")]
    public decimal? EstimateLow { get; set; }

    [Range(typeof(decimal), "0", "999999.99")]
    public decimal? EstimateHigh { get; set; }

    [StringLength(1000)]
    public string? PartsNeeded { get; set; }

    [StringLength(2000)]
    public string? InternalNotes { get; set; }

    public DateTime? CreatedAtUtc { get; set; }
    public DateTime? UpdatedAtUtc { get; set; }
}
