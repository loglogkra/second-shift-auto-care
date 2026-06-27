using Microsoft.EntityFrameworkCore;
using SecondShiftAutoCare.Api.Data;
using SecondShiftAutoCare.Api.Entities;
using SecondShiftAutoCare.Shared.Models;

namespace SecondShiftAutoCare.Api;

public sealed class ServiceRequestRepository(ServiceRequestDbContext dbContext)
{
    public async Task<ServiceRequestDto> CreateAsync(ServiceRequestDto request)
    {
        var now = DateTime.UtcNow;
        var customer = new Customer { Id = Guid.NewGuid(), Name = request.CustomerName.Trim(), Phone = request.Phone.Trim(), Email = TrimOrNull(request.Email), CreatedUtc = now, UpdatedUtc = now };
        var vehicle = new Vehicle { Id = Guid.NewGuid(), CustomerId = customer.Id, Year = request.VehicleYear!.Value, Make = request.VehicleMake.Trim(), Model = request.VehicleModel.Trim(), Mileage = request.Mileage, CreatedUtc = now, UpdatedUtc = now };
        var serviceRequest = new ServiceRequest
        {
            Id = request.Id ?? Guid.NewGuid(), CustomerId = customer.Id, VehicleId = vehicle.Id, ServiceType = request.ServiceType.Trim(),
            ServiceSpecificAnswers = TrimOrNull(request.ServiceSpecificAnswers), Symptoms = TrimOrNull(request.Symptoms), Notes = null,
            PreferredAvailability = TrimOrNull(request.PreferredAvailability), UrgencyLevel = request.UrgencyLevel, IsVehicleDrivable = TrimOrNull(request.IsVehicleDrivable), VehicleLocation = TrimOrNull(request.VehicleLocation),
            ConsentAccepted = request.ConsentAccepted, WantsPhotoUploadLater = request.WantsPhotoUploadLater, Status = ServiceRequestStatuses.New, CreatedUtc = now, UpdatedUtc = now
        };
        var quote = new Quote { Id = Guid.NewGuid(), ServiceRequestId = serviceRequest.Id, CustomerApprovalStatus = ServiceRequestApprovalStatuses.Pending, CreatedUtc = now, UpdatedUtc = now };
        dbContext.Customers.Add(customer); dbContext.Vehicles.Add(vehicle); dbContext.ServiceRequests.Add(serviceRequest); dbContext.Quotes.Add(quote);
        await dbContext.SaveChangesAsync();
        serviceRequest.Customer = customer; serviceRequest.Vehicle = vehicle; serviceRequest.Quote = quote;
        return ToDto(serviceRequest);
    }

    public async Task<IReadOnlyList<ServiceRequestDto>> GetAllAsync() => await dbContext.ServiceRequests.AsNoTracking().Include(x => x.Customer).Include(x => x.Vehicle).Include(x => x.Quote).OrderByDescending(x => x.CreatedUtc).Select(x => ToDto(x)).ToListAsync();
    public async Task<ServiceRequestDto?> GetByIdAsync(Guid id) { var r = await dbContext.ServiceRequests.AsNoTracking().Include(x => x.Customer).Include(x => x.Vehicle).Include(x => x.Quote).SingleOrDefaultAsync(x => x.Id == id); return r is null ? null : ToDto(r); }
    public Task<ServiceRequestDto?> UpdateStatusAsync(Guid id, string status) => UpdateAsync(id, r => r.Status = status);
    public Task<ServiceRequestDto?> UpdateQuoteAsync(Guid id, ServiceRequestQuoteUpdateModel q) => UpdateAsync(id, r =>
    {
        r.Quote ??= new Quote { Id = Guid.NewGuid(), ServiceRequestId = r.Id, CreatedUtc = DateTime.UtcNow };
        r.Quote.EstimateLow = q.EstimateLow; r.Quote.EstimateHigh = q.EstimateHigh; r.Quote.PartsNeeded = TrimOrNull(q.PartsNeeded); r.Quote.InternalQuoteNotes = TrimOrNull(q.InternalQuoteNotes); r.Quote.LaborAmount = q.LaborAmount; r.Quote.PartsAmount = q.PartsAmount; r.Quote.ShopSuppliesAmount = q.ShopSuppliesAmount; r.Quote.TotalEstimate = q.TotalEstimate; r.Quote.QuoteTemplate = TrimOrNull(q.QuoteTemplate); r.Quote.AssumptionDisclaimerText = TrimOrNull(q.AssumptionDisclaimerText); r.Quote.GoodOption = TrimOrNull(q.GoodOption); r.Quote.BetterOption = TrimOrNull(q.BetterOption); r.Quote.BestOption = TrimOrNull(q.BestOption); r.Quote.QuoteExpirationDate = q.QuoteExpirationDate; r.Quote.CustomerApprovalStatus = q.CustomerApprovalStatus; r.Quote.UpdatedUtc = DateTime.UtcNow;
    });
    public Task<ServiceRequestDto?> UpdateNotesAsync(Guid id, string? internalNotes) => UpdateAsync(id, r => r.InternalNotes = TrimOrNull(internalNotes));

    private async Task<ServiceRequestDto?> UpdateAsync(Guid id, Action<ServiceRequest> applyUpdate)
    {
        var r = await dbContext.ServiceRequests.Include(x => x.Customer).Include(x => x.Vehicle).Include(x => x.Quote).SingleOrDefaultAsync(x => x.Id == id);
        if (r is null) return null;
        applyUpdate(r); r.UpdatedUtc = DateTime.UtcNow; await dbContext.SaveChangesAsync(); return ToDto(r);
    }
    private static string? TrimOrNull(string? value) => string.IsNullOrWhiteSpace(value) ? null : value.Trim();
    private static ServiceRequestDto ToDto(ServiceRequest r) => new()
    {
        Id = r.Id, CustomerName = r.Customer.Name, Phone = r.Customer.Phone, Email = r.Customer.Email, VehicleYear = r.Vehicle.Year, VehicleMake = r.Vehicle.Make, VehicleModel = r.Vehicle.Model, Mileage = r.Vehicle.Mileage,
        ServiceType = r.ServiceType, Symptoms = r.Symptoms, PreferredAvailability = r.PreferredAvailability, ServiceSpecificAnswers = r.ServiceSpecificAnswers, UrgencyLevel = r.UrgencyLevel, IsVehicleDrivable = r.IsVehicleDrivable, VehicleLocation = r.VehicleLocation, ConsentAccepted = r.ConsentAccepted, WantsPhotoUploadLater = r.WantsPhotoUploadLater, Status = r.Status,
        EstimateLow = r.Quote?.EstimateLow, EstimateHigh = r.Quote?.EstimateHigh, PartsNeeded = r.Quote?.PartsNeeded, InternalQuoteNotes = r.Quote?.InternalQuoteNotes, LaborAmount = r.Quote?.LaborAmount, PartsAmount = r.Quote?.PartsAmount, ShopSuppliesAmount = r.Quote?.ShopSuppliesAmount, TotalEstimate = r.Quote?.TotalEstimate, QuoteTemplate = r.Quote?.QuoteTemplate, AssumptionDisclaimerText = r.Quote?.AssumptionDisclaimerText, GoodOption = r.Quote?.GoodOption, BetterOption = r.Quote?.BetterOption, BestOption = r.Quote?.BestOption, QuoteExpirationDate = r.Quote?.QuoteExpirationDate, CustomerApprovalStatus = r.Quote?.CustomerApprovalStatus ?? ServiceRequestApprovalStatuses.Pending,
        InternalNotes = r.InternalNotes, CreatedAtUtc = r.CreatedUtc, UpdatedAtUtc = r.UpdatedUtc
    };
}
