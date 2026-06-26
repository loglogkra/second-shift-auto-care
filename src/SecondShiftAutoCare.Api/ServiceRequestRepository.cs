using Microsoft.EntityFrameworkCore;
using SecondShiftAutoCare.Api.Data;
using SecondShiftAutoCare.Api.Entities;
using SecondShiftAutoCare.Shared.Models;

namespace SecondShiftAutoCare.Api;

public sealed class ServiceRequestRepository(ServiceRequestDbContext dbContext)
{
    public async Task<ServiceRequestDto> CreateAsync(ServiceRequestDto request)
    {
        var serviceRequest = new ServiceRequest
        {
            Id = request.Id ?? Guid.NewGuid(),
            CustomerName = request.CustomerName.Trim(),
            Phone = request.Phone.Trim(),
            Email = string.IsNullOrWhiteSpace(request.Email) ? null : request.Email.Trim(),
            VehicleYear = request.VehicleYear!.Value,
            VehicleMake = request.VehicleMake.Trim(),
            VehicleModel = request.VehicleModel.Trim(),
            Mileage = request.Mileage,
            ServiceType = request.ServiceType.Trim(),
            ServiceSpecificAnswers = string.IsNullOrWhiteSpace(request.ServiceSpecificAnswers) ? null : request.ServiceSpecificAnswers.Trim(),
            Symptoms = request.Symptoms.Trim(),
            PreferredAvailability = string.IsNullOrWhiteSpace(request.PreferredAvailability) ? null : request.PreferredAvailability.Trim(),
            UrgencyLevel = request.UrgencyLevel,
            IsVehicleDrivable = string.IsNullOrWhiteSpace(request.IsVehicleDrivable) ? null : request.IsVehicleDrivable.Trim(),
            VehicleLocation = string.IsNullOrWhiteSpace(request.VehicleLocation) ? null : request.VehicleLocation.Trim(),
            AlternateContactName = string.IsNullOrWhiteSpace(request.AlternateContactName) ? null : request.AlternateContactName.Trim(),
            AlternateContactPhone = string.IsNullOrWhiteSpace(request.AlternateContactPhone) ? null : request.AlternateContactPhone.Trim(),
            ConsentAccepted = request.ConsentAccepted,
            WantsPhotoUploadLater = request.WantsPhotoUploadLater,
            Status = ServiceRequestStatuses.New,
            CreatedUtc = DateTime.UtcNow,
            UpdatedUtc = DateTime.UtcNow
        };

        dbContext.ServiceRequests.Add(serviceRequest);
        await dbContext.SaveChangesAsync();

        return ToDto(serviceRequest);
    }

    public async Task<IReadOnlyList<ServiceRequestDto>> GetAllAsync()
    {
        var serviceRequests = await dbContext.ServiceRequests
            .AsNoTracking()
            .OrderByDescending(request => request.CreatedUtc)
            .ToListAsync();

        return serviceRequests.Select(ToDto).ToList();
    }

    public async Task<ServiceRequestDto?> GetByIdAsync(Guid id)
    {
        var serviceRequest = await dbContext.ServiceRequests
            .AsNoTracking()
            .SingleOrDefaultAsync(request => request.Id == id);

        return serviceRequest is null ? null : ToDto(serviceRequest);
    }

    public Task<ServiceRequestDto?> UpdateStatusAsync(Guid id, string status) => UpdateAsync(id, request => request.Status = status);

    public Task<ServiceRequestDto?> UpdateQuoteAsync(Guid id, ServiceRequestQuoteUpdateModel quote) =>
        UpdateAsync(id, request =>
        {
            request.EstimateLow = quote.EstimateLow;
            request.EstimateHigh = quote.EstimateHigh;
            request.PartsNeeded = TrimOrNull(quote.PartsNeeded);
            request.InternalQuoteNotes = TrimOrNull(quote.InternalQuoteNotes);
            request.LaborAmount = quote.LaborAmount;
            request.PartsAmount = quote.PartsAmount;
            request.ShopSuppliesAmount = quote.ShopSuppliesAmount;
            request.TotalEstimate = quote.TotalEstimate;
            request.QuoteTemplate = TrimOrNull(quote.QuoteTemplate);
            request.AssumptionDisclaimerText = TrimOrNull(quote.AssumptionDisclaimerText);
            request.GoodOption = TrimOrNull(quote.GoodOption);
            request.BetterOption = TrimOrNull(quote.BetterOption);
            request.BestOption = TrimOrNull(quote.BestOption);
            request.QuoteExpirationDate = quote.QuoteExpirationDate;
            request.CustomerApprovalStatus = quote.CustomerApprovalStatus;
        });

    public Task<ServiceRequestDto?> UpdateNotesAsync(Guid id, string? internalNotes) =>
        UpdateAsync(id, request => request.InternalNotes = TrimOrNull(internalNotes));

    private async Task<ServiceRequestDto?> UpdateAsync(Guid id, Action<ServiceRequest> applyUpdate)
    {
        var serviceRequest = await dbContext.ServiceRequests.SingleOrDefaultAsync(request => request.Id == id);
        if (serviceRequest is null)
        {
            return null;
        }

        applyUpdate(serviceRequest);
        serviceRequest.UpdatedUtc = DateTime.UtcNow;
        await dbContext.SaveChangesAsync();

        return ToDto(serviceRequest);
    }

    private static string? TrimOrNull(string? value) => string.IsNullOrWhiteSpace(value) ? null : value.Trim();

    private static ServiceRequestDto ToDto(ServiceRequest request) => new()
    {
        Id = request.Id,
        CustomerName = request.CustomerName,
        Phone = request.Phone,
        Email = request.Email,
        VehicleYear = request.VehicleYear,
        VehicleMake = request.VehicleMake,
        VehicleModel = request.VehicleModel,
        Mileage = request.Mileage,
        ServiceType = request.ServiceType,
        Symptoms = request.Symptoms,
        PreferredAvailability = request.PreferredAvailability,
        ServiceSpecificAnswers = request.ServiceSpecificAnswers,
        UrgencyLevel = request.UrgencyLevel,
        IsVehicleDrivable = request.IsVehicleDrivable,
        VehicleLocation = request.VehicleLocation,
        AlternateContactName = request.AlternateContactName,
        AlternateContactPhone = request.AlternateContactPhone,
        ConsentAccepted = request.ConsentAccepted,
        WantsPhotoUploadLater = request.WantsPhotoUploadLater,
        Status = request.Status,
        EstimateLow = request.EstimateLow,
        EstimateHigh = request.EstimateHigh,
        PartsNeeded = request.PartsNeeded,
        InternalQuoteNotes = request.InternalQuoteNotes,
        LaborAmount = request.LaborAmount,
        PartsAmount = request.PartsAmount,
        ShopSuppliesAmount = request.ShopSuppliesAmount,
        TotalEstimate = request.TotalEstimate,
        QuoteTemplate = request.QuoteTemplate,
        AssumptionDisclaimerText = request.AssumptionDisclaimerText,
        GoodOption = request.GoodOption,
        BetterOption = request.BetterOption,
        BestOption = request.BestOption,
        QuoteExpirationDate = request.QuoteExpirationDate,
        CustomerApprovalStatus = request.CustomerApprovalStatus,
        InternalNotes = request.InternalNotes,
        CreatedAtUtc = request.CreatedUtc,
        UpdatedAtUtc = request.UpdatedUtc
    };
}
