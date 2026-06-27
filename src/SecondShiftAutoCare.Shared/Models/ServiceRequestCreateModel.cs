using System.ComponentModel.DataAnnotations;

namespace SecondShiftAutoCare.Shared.Models;

public sealed class ServiceRequestCreateModel : IValidatableObject
{
    [Required, StringLength(100)]
    [Display(Name = "Customer name")]
    public string CustomerName { get; set; } = string.Empty;

    [Required, Phone, StringLength(30)]
    public string Phone { get; set; } = string.Empty;

    [EmailAddress, StringLength(150)]
    public string? Email { get; set; }

    [Required, VehicleYear]
    [Display(Name = "Vehicle year")]
    public int? VehicleYear { get; set; } = DateTime.Now.Year;

    [Required, StringLength(75)]
    [Display(Name = "Vehicle make")]
    public string VehicleMake { get; set; } = string.Empty;

    [Required, StringLength(75)]
    [Display(Name = "Vehicle model")]
    public string VehicleModel { get; set; } = string.Empty;

    [Range(0, 999999)]
    public int? Mileage { get; set; }

    [Required]
    [Display(Name = "Services needed")]
    public string ServiceType { get; set; } = string.Empty;

    [StringLength(1000)]
    [Display(Name = "Service-specific answers")]
    public string? ServiceSpecificAnswers { get; set; }

    [StringLength(2000)]
    public string? Symptoms { get; set; }

    [StringLength(500)]
    [Display(Name = "Preferred availability")]
    public string? PreferredAvailability { get; set; }

    [Required]
    [Display(Name = "Urgency level")]
    public string UrgencyLevel { get; set; } = ServiceRequestUrgencyLevels.Routine;

    [Required]
    [Display(Name = "Is the vehicle drivable?")]
    public string? IsVehicleDrivable { get; set; }

    [StringLength(300)]
    [Display(Name = "Where is the vehicle located?")]
    public string? VehicleLocation { get; set; }


    public bool ConsentAccepted { get; set; }

    public bool WantsPhotoUploadLater { get; set; }

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        var selectedServices = SplitServices(ServiceType).ToArray();
        if (selectedServices.Length == 0 || selectedServices.Any(service => !ServiceTypeOptions.All.Contains(service)))
        {
            yield return new ValidationResult("Select one or more valid service types.", [nameof(ServiceType)]);
        }

        if (!ServiceRequestUrgencyLevels.All.Contains(UrgencyLevel))
        {
            yield return new ValidationResult("Select a valid urgency level.", [nameof(UrgencyLevel)]);
        }

        if (IsVehicleDrivable is not ("Yes" or "No" or "Unsure"))
        {
            yield return new ValidationResult("Select whether the vehicle is drivable.", [nameof(IsVehicleDrivable)]);
        }

        if (!ConsentAccepted)
        {
            yield return new ValidationResult("Please accept the consent/disclaimer before submitting.", [nameof(ConsentAccepted)]);
        }
    }

    public ServiceRequestDto ToDto() => new()
    {
        CustomerName = CustomerName,
        Phone = Phone,
        Email = Email,
        VehicleYear = VehicleYear,
        VehicleMake = VehicleMake,
        VehicleModel = VehicleModel,
        Mileage = Mileage,
        ServiceType = ServiceType,
        Symptoms = Symptoms,
        PreferredAvailability = PreferredAvailability,
        ServiceSpecificAnswers = ServiceSpecificAnswers,
        UrgencyLevel = UrgencyLevel,
        IsVehicleDrivable = IsVehicleDrivable,
        VehicleLocation = VehicleLocation,
        ConsentAccepted = ConsentAccepted,
        WantsPhotoUploadLater = WantsPhotoUploadLater
    };

    private static IEnumerable<string> SplitServices(string services) =>
        services.Split(",", StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
}
