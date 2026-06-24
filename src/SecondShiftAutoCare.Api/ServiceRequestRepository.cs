using System.Data;
using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using SecondShiftAutoCare.Shared.Models;

namespace SecondShiftAutoCare.Api;

public sealed class ServiceRequestRepository
{
    private readonly string _connectionString;

    public ServiceRequestRepository(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("SqlConnection")
            ?? configuration["SqlConnectionString"]
            ?? throw new InvalidOperationException("Configure an Azure SQL connection string named 'SqlConnection' or 'SqlConnectionString'.");
    }

    public async Task<ServiceRequestDto> CreateAsync(ServiceRequestDto request)
    {
        const string sql = """
            INSERT INTO dbo.ServiceRequests
            (
                Id, CustomerName, Phone, Email, VehicleYear, VehicleMake, VehicleModel, Mileage,
                ServiceType, Symptoms, PreferredAvailability, Status
            )
            OUTPUT
                inserted.Id,
                inserted.CustomerName,
                inserted.Phone,
                inserted.Email,
                inserted.VehicleYear,
                inserted.VehicleMake,
                inserted.VehicleModel,
                inserted.Mileage,
                inserted.ServiceType,
                inserted.Symptoms,
                inserted.PreferredAvailability,
                inserted.Status,
                inserted.EstimateLow,
                inserted.EstimateHigh,
                inserted.PartsNeeded,
                inserted.InternalNotes,
                inserted.CreatedAtUtc,
                inserted.UpdatedAtUtc
            VALUES
            (
                @Id, @CustomerName, @Phone, @Email, @VehicleYear, @VehicleMake, @VehicleModel, @Mileage,
                @ServiceType, @Symptoms, @PreferredAvailability, @Status
            );
            """;

        var parameters = new
        {
            Id = request.Id ?? Guid.NewGuid(),
            request.CustomerName,
            request.Phone,
            request.Email,
            request.VehicleYear,
            request.VehicleMake,
            request.VehicleModel,
            request.Mileage,
            request.ServiceType,
            request.Symptoms,
            request.PreferredAvailability,
            Status = ServiceRequestStatuses.New
        };

        using var connection = CreateConnection();
        return await connection.QuerySingleAsync<ServiceRequestDto>(sql, parameters);
    }

    public async Task<IReadOnlyList<ServiceRequestDto>> GetAllAsync()
    {
        const string sql = """
            SELECT Id, CustomerName, Phone, Email, VehicleYear, VehicleMake, VehicleModel, Mileage,
                   ServiceType, Symptoms, PreferredAvailability, Status,
                   EstimateLow, EstimateHigh, PartsNeeded, InternalNotes, CreatedAtUtc, UpdatedAtUtc
            FROM dbo.ServiceRequests
            ORDER BY CreatedAtUtc DESC;
            """;

        using var connection = CreateConnection();
        var requests = await connection.QueryAsync<ServiceRequestDto>(sql);
        return requests.AsList();
    }

    public async Task<ServiceRequestDto?> GetByIdAsync(Guid id)
    {
        const string sql = """
            SELECT Id, CustomerName, Phone, Email, VehicleYear, VehicleMake, VehicleModel, Mileage,
                   ServiceType, Symptoms, PreferredAvailability, Status,
                   EstimateLow, EstimateHigh, PartsNeeded, InternalNotes, CreatedAtUtc, UpdatedAtUtc
            FROM dbo.ServiceRequests
            WHERE Id = @Id;
            """;

        using var connection = CreateConnection();
        return await connection.QuerySingleOrDefaultAsync<ServiceRequestDto>(sql, new { Id = id });
    }

    public Task<ServiceRequestDto?> UpdateStatusAsync(Guid id, string status) => UpdateAsync(
        id,
        """
        UPDATE dbo.ServiceRequests
        SET Status = @Status,
            UpdatedAtUtc = SYSUTCDATETIME()
        WHERE Id = @Id;
        """,
        new { Id = id, Status = status });

    public Task<ServiceRequestDto?> UpdateQuoteAsync(Guid id, decimal? estimateLow, decimal? estimateHigh, string? partsNeeded) => UpdateAsync(
        id,
        """
        UPDATE dbo.ServiceRequests
        SET EstimateLow = @EstimateLow,
            EstimateHigh = @EstimateHigh,
            PartsNeeded = @PartsNeeded,
            UpdatedAtUtc = SYSUTCDATETIME()
        WHERE Id = @Id;
        """,
        new { Id = id, EstimateLow = estimateLow, EstimateHigh = estimateHigh, PartsNeeded = partsNeeded });

    public Task<ServiceRequestDto?> UpdateNotesAsync(Guid id, string? internalNotes) => UpdateAsync(
        id,
        """
        UPDATE dbo.ServiceRequests
        SET InternalNotes = @InternalNotes,
            UpdatedAtUtc = SYSUTCDATETIME()
        WHERE Id = @Id;
        """,
        new { Id = id, InternalNotes = internalNotes });

    private async Task<ServiceRequestDto?> UpdateAsync(Guid id, string updateSql, object parameters)
    {
        using var connection = CreateConnection();
        var rowsAffected = await connection.ExecuteAsync(updateSql, parameters);
        if (rowsAffected == 0)
        {
            return null;
        }

        const string selectSql = """
            SELECT Id, CustomerName, Phone, Email, VehicleYear, VehicleMake, VehicleModel, Mileage,
                   ServiceType, Symptoms, PreferredAvailability, Status,
                   EstimateLow, EstimateHigh, PartsNeeded, InternalNotes, CreatedAtUtc, UpdatedAtUtc
            FROM dbo.ServiceRequests
            WHERE Id = @Id;
            """;

        return await connection.QuerySingleAsync<ServiceRequestDto>(selectSql, new { Id = id });
    }

    private IDbConnection CreateConnection() => new SqlConnection(_connectionString);
}
