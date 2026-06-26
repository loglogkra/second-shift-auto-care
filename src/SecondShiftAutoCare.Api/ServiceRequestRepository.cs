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
            Symptoms = request.Symptoms.Trim(),
            PreferredAvailability = string.IsNullOrWhiteSpace(request.PreferredAvailability) ? null : request.PreferredAvailability.Trim(),
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

    public Task<ServiceRequestDto?> UpdateQuoteAsync(Guid id, decimal? estimateLow, decimal? estimateHigh, string? partsNeeded) =>
        UpdateAsync(id, request =>
        {
            request.EstimateLow = estimateLow;
            request.EstimateHigh = estimateHigh;
            request.PartsNeeded = string.IsNullOrWhiteSpace(partsNeeded) ? null : partsNeeded.Trim();
        });

    public Task<ServiceRequestDto?> UpdateNotesAsync(Guid id, string? internalNotes) =>
        UpdateAsync(id, request => request.InternalNotes = string.IsNullOrWhiteSpace(internalNotes) ? null : internalNotes.Trim());

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
        Status = request.Status,
        EstimateLow = request.EstimateLow,
        EstimateHigh = request.EstimateHigh,
        PartsNeeded = request.PartsNeeded,
        InternalNotes = request.InternalNotes,
        CreatedAtUtc = request.CreatedUtc,
        UpdatedAtUtc = request.UpdatedUtc
    };
}
