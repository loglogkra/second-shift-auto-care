namespace SecondShiftAutoCare.Api.Entities;

public sealed class VehicleMake
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;
    public ICollection<VehicleModel> Models { get; set; } = [];
}
