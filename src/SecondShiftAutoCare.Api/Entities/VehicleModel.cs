namespace SecondShiftAutoCare.Api.Entities;

public sealed class VehicleModel
{
    public Guid Id { get; set; }
    public Guid VehicleMakeId { get; set; }
    public VehicleMake VehicleMake { get; set; } = null!;
    public string Name { get; set; } = string.Empty;
    public int StartYear { get; set; }
    public int? EndYear { get; set; }
    public bool IsActive { get; set; } = true;
}
