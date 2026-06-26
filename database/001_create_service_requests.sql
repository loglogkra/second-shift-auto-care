-- Legacy/manual equivalent of the EF Core InitialServiceRequestSchema migration.
-- Prefer applying src/SecondShiftAutoCare.Api/Migrations with dotnet ef.

CREATE TABLE dbo.ServiceRequests
(
    Id UNIQUEIDENTIFIER NOT NULL CONSTRAINT PK_ServiceRequests PRIMARY KEY,
    CustomerName NVARCHAR(100) NOT NULL,
    Phone NVARCHAR(30) NOT NULL,
    Email NVARCHAR(150) NULL,
    VehicleYear INT NOT NULL,
    VehicleMake NVARCHAR(75) NOT NULL,
    VehicleModel NVARCHAR(75) NOT NULL,
    Mileage INT NULL,
    ServiceType NVARCHAR(75) NOT NULL,
    Symptoms NVARCHAR(2000) NOT NULL,
    PreferredAvailability NVARCHAR(500) NULL,
    Status NVARCHAR(50) NOT NULL CONSTRAINT DF_ServiceRequests_Status DEFAULT N'New',
    EstimateLow DECIMAL(8, 2) NULL,
    EstimateHigh DECIMAL(8, 2) NULL,
    PartsNeeded NVARCHAR(1000) NULL,
    InternalNotes NVARCHAR(2000) NULL,
    CreatedUtc DATETIME2 NOT NULL CONSTRAINT DF_ServiceRequests_CreatedUtc DEFAULT SYSUTCDATETIME(),
    UpdatedUtc DATETIME2 NOT NULL CONSTRAINT DF_ServiceRequests_UpdatedUtc DEFAULT SYSUTCDATETIME()
);

CREATE INDEX IX_ServiceRequests_CreatedUtc ON dbo.ServiceRequests (CreatedUtc);
CREATE INDEX IX_ServiceRequests_Status ON dbo.ServiceRequests (Status);
