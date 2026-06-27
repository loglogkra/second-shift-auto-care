using SecondShiftAutoCare.Shared.Models;

namespace SecondShiftAutoCare.Client.Services;

public sealed class VehicleCatalogService
{
    public IReadOnlyList<string> GetMakes(int? year) => year is null ? [] : VehicleCatalogOptions.Makes;

    public IReadOnlyList<string> GetModels(int? year, string? make)
    {
        if (year is null || string.IsNullOrWhiteSpace(make)) return [];
        var models = VehicleCatalogOptions.Models
            .Where(model => string.Equals(model.Make, make, StringComparison.OrdinalIgnoreCase) && model.IsAvailableForYear(year.Value))
            .Select(model => model.Name)
            .OrderBy(name => name)
            .ToList();
        models.Add(VehicleCatalogOptions.OtherModel);
        return models;
    }
}
