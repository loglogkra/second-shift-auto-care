using System.ComponentModel.DataAnnotations;

namespace SecondShiftAutoCare.Shared.Models;

[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter)]
public sealed class VehicleYearAttribute : ValidationAttribute
{
    public const int MinimumYear = 1970;

    public VehicleYearAttribute() => ErrorMessage = $"Vehicle year must be between {MinimumYear} and next year.";

    public override bool IsValid(object? value)
    {
        if (value is null) return true;
        if (value is not int year) return false;
        return year >= MinimumYear && year <= DateTime.UtcNow.Year + 1;
    }

    public override string FormatErrorMessage(string name) => $"{name} must be between {MinimumYear} and {DateTime.UtcNow.Year + 1}.";
}
