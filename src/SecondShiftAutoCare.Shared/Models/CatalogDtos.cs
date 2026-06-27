namespace SecondShiftAutoCare.Shared.Models;

public sealed record CustomerDto(Guid? Id, string Name, string Phone, string? Email);
public sealed record VehicleDto(Guid? Id, int Year, string Make, string Model, int? Mileage);
public sealed record QuoteDto(Guid? Id, decimal? EstimateLow, decimal? EstimateHigh, decimal? LaborAmount, decimal? PartsAmount, decimal? ShopSuppliesAmount, decimal? TotalEstimate, string? PartsNeeded, string? InternalQuoteNotes, string? QuoteTemplate, string? AssumptionDisclaimerText, string? GoodOption, string? BetterOption, string? BestOption, string CustomerApprovalStatus, DateTime? QuoteExpirationDate);
public sealed record VehicleMakeDto(Guid Id, string Name, bool IsActive);
public sealed record VehicleModelDto(Guid Id, Guid VehicleMakeId, string Name, int StartYear, int? EndYear, bool IsActive);
public sealed record ServiceCatalogItemDto(Guid Id, string Name, string? Description, bool IsActive, int SortOrder);
