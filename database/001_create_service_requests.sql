CREATE TABLE dbo.ServiceRequests
(
    Id UNIQUEIDENTIFIER NOT NULL CONSTRAINT PK_ServiceRequests PRIMARY KEY,
    CustomerName NVARCHAR(100) NOT NULL,
    Phone NVARCHAR(25) NOT NULL,
    Email NVARCHAR(254) NULL,
    VehicleYear INT NULL,
    VehicleMake NVARCHAR(60) NOT NULL,
    VehicleModel NVARCHAR(60) NOT NULL,
    Mileage INT NULL,
    ServiceType NVARCHAR(80) NOT NULL,
    Symptoms NVARCHAR(2000) NOT NULL,
    PreferredAvailability NVARCHAR(500) NOT NULL,
    ConsentAccepted BIT NOT NULL,
    SubmittedAt DATETIMEOFFSET NOT NULL CONSTRAINT DF_ServiceRequests_SubmittedAt DEFAULT SYSUTCDATETIME(),
    Status NVARCHAR(40) NOT NULL CONSTRAINT DF_ServiceRequests_Status DEFAULT N'New',
    QuoteAmount DECIMAL(10, 2) NULL,
    QuoteNotes NVARCHAR(2000) NULL,
    InternalNotes NVARCHAR(4000) NULL,
    UpdatedAt DATETIMEOFFSET NULL
);
GO

CREATE INDEX IX_ServiceRequests_SubmittedAt ON dbo.ServiceRequests (SubmittedAt DESC);
GO

CREATE INDEX IX_ServiceRequests_Status ON dbo.ServiceRequests (Status);
GO
