using System.ComponentModel.DataAnnotations;

namespace SecondShiftAutoCare.Shared.Models;

public sealed class ServiceRequestDto
{
    public Guid? Id { get; set; }
    public Guid? CustomerId { get; set; }
    public Guid? VehicleId { get; set; }

    [Required, StringLength(100)]
    public string CustomerName { get; set; } = string.Empty;

    [Required, Phone, StringLength(30)]
    public string Phone { get; set; } = string.Empty;

    [EmailAddress, StringLength(150)]
    public string? Email { get; set; }

    [Required, VehicleYear]
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

    [StringLength(2000)]
    public string? Symptoms { get; set; }

    [StringLength(500)]
    public string? PreferredAvailability { get; set; }

    [StringLength(50)]
    public string UrgencyLevel { get; set; } = ServiceRequestUrgencyLevels.Routine;

    [StringLength(30)]
    public string? IsVehicleDrivable { get; set; }

    [StringLength(300)]
    public string? VehicleLocation { get; set; }

    public bool ConsentAccepted { get; set; }

    public bool WantsPhotoUploadLater { get; set; }

    public string Status { get; set; } = ServiceRequestStatuses.New;
    public bool IsArchived { get; set; }
    public string? PublicStatusToken { get; set; }
    public string? QuoteApprovalToken { get; set; }
    public DateTime? ScheduledStartUtc { get; set; }
    public DateTime? ScheduledEndUtc { get; set; }
    public int? EstimatedDurationMinutes { get; set; }
    public string? ServiceLocationType { get; set; }
    public string? ScheduleNotes { get; set; }

    [Range(typeof(decimal), "0", "999999.99")]
    public decimal? EstimateLow { get; set; }

    [Range(typeof(decimal), "0", "999999.99")]
    public decimal? EstimateHigh { get; set; }

    [StringLength(1000)]
    public string? PartsNeeded { get; set; }

    [StringLength(4000)]
    public string? InternalQuoteNotes { get; set; }

    [Range(typeof(decimal), "0", "999999.99")]
    public decimal? LaborAmount { get; set; }

    [Range(typeof(decimal), "0", "999999.99")]
    public decimal? PartsAmount { get; set; }

    [Range(typeof(decimal), "0", "999999.99")]
    public decimal? ShopSuppliesAmount { get; set; }

    [Range(typeof(decimal), "0", "999999.99")]
    public decimal? TotalEstimate { get; set; }

    [StringLength(100)]
    public string? QuoteTemplate { get; set; }

    [StringLength(2000)]
    public string? AssumptionDisclaimerText { get; set; }

    [StringLength(2000)]
    public string? GoodOption { get; set; }

    [StringLength(2000)]
    public string? BetterOption { get; set; }

    [StringLength(2000)]
    public string? BestOption { get; set; }

    public DateTime? QuoteExpirationDate { get; set; }

    [StringLength(50)]
    public string CustomerApprovalStatus { get; set; } = ServiceRequestApprovalStatuses.Pending;
    public DateTime? CustomerApprovalRespondedUtc { get; set; }
    public string? CustomerApprovalMessage { get; set; }
    public string? PaymentStatus { get; set; }
    public List<ServiceIntakeAnswerDto> IntakeAnswers { get; set; } = [];

    [StringLength(2000)]
    public string? InternalNotes { get; set; }

    public DateTime? CreatedAtUtc { get; set; }
    public DateTime? UpdatedAtUtc { get; set; }
}
