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

    [Required, Range(1900, 2100)]
    [Display(Name = "Vehicle year")]
    public int? VehicleYear { get; set; }

    [Required, StringLength(75)]
    [Display(Name = "Vehicle make")]
    public string VehicleMake { get; set; } = string.Empty;

    [Required, StringLength(75)]
    [Display(Name = "Vehicle model")]
    public string VehicleModel { get; set; } = string.Empty;

    [Range(0, 999999)]
    public int? Mileage { get; set; }

    [Required]
    [Display(Name = "Service type")]
    public string ServiceType { get; set; } = string.Empty;

    [Required, StringLength(2000, MinimumLength = 10)]
    public string Symptoms { get; set; } = string.Empty;

    [StringLength(500)]
    [Display(Name = "Preferred availability")]
    public string? PreferredAvailability { get; set; }

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (!string.IsNullOrWhiteSpace(ServiceType) && !ServiceTypeOptions.All.Contains(ServiceType))
        {
            yield return new ValidationResult("Select a valid service type.", [nameof(ServiceType)]);
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
        PreferredAvailability = PreferredAvailability
    };
}
