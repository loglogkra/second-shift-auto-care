using Microsoft.EntityFrameworkCore;
using SecondShiftAutoCare.Api.Entities;
using SecondShiftAutoCare.Shared.Models;

namespace SecondShiftAutoCare.Api.Data;

public sealed class ServiceRequestDbContext(DbContextOptions<ServiceRequestDbContext> options) : DbContext(options)
{
    public DbSet<ServiceRequest> ServiceRequests => Set<ServiceRequest>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        var serviceRequest = modelBuilder.Entity<ServiceRequest>();

        serviceRequest.ToTable("ServiceRequests", "dbo");
        serviceRequest.HasKey(request => request.Id);

        serviceRequest.Property(request => request.Id)
            .ValueGeneratedNever();

        serviceRequest.Property(request => request.CustomerName)
            .HasMaxLength(100)
            .IsRequired();

        serviceRequest.Property(request => request.Phone)
            .HasMaxLength(30)
            .IsRequired();

        serviceRequest.Property(request => request.Email)
            .HasMaxLength(150);

        serviceRequest.Property(request => request.VehicleYear)
            .IsRequired();

        serviceRequest.Property(request => request.VehicleMake)
            .HasMaxLength(75)
            .IsRequired();

        serviceRequest.Property(request => request.VehicleModel)
            .HasMaxLength(75)
            .IsRequired();

        serviceRequest.Property(request => request.ServiceType)
            .HasMaxLength(75)
            .IsRequired();

        serviceRequest.Property(request => request.Symptoms)
            .HasMaxLength(2000)
            .IsRequired();

        serviceRequest.Property(request => request.PreferredAvailability)
            .HasMaxLength(500);

        serviceRequest.Property(request => request.Status)
            .HasMaxLength(50)
            .HasDefaultValue(ServiceRequestStatuses.New)
            .IsRequired();

        serviceRequest.Property(request => request.EstimateLow)
            .HasPrecision(8, 2);

        serviceRequest.Property(request => request.EstimateHigh)
            .HasPrecision(8, 2);

        serviceRequest.Property(request => request.PartsNeeded)
            .HasMaxLength(1000);

        serviceRequest.Property(request => request.InternalNotes)
            .HasMaxLength(2000);

        serviceRequest.Property(request => request.CreatedUtc)
            .HasDefaultValueSql("SYSUTCDATETIME()")
            .IsRequired();

        serviceRequest.Property(request => request.UpdatedUtc)
            .HasDefaultValueSql("SYSUTCDATETIME()")
            .IsRequired();

        serviceRequest.HasIndex(request => request.CreatedUtc);
        serviceRequest.HasIndex(request => request.Status);
    }
}
