namespace SecondShiftAutoCare.Api.Entities;

public sealed class Customer
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string? Email { get; set; }
    public DateTime CreatedUtc { get; set; }
    public DateTime UpdatedUtc { get; set; }
    public ICollection<Vehicle> Vehicles { get; set; } = [];
    public ICollection<ServiceRequest> ServiceRequests { get; set; } = [];
}
