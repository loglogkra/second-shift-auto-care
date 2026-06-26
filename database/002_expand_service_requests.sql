ALTER TABLE [dbo].[ServiceRequests] ALTER COLUMN [ServiceType] NVARCHAR(1000) NOT NULL;
GO

ALTER TABLE [dbo].[ServiceRequests] ADD
    [ServiceSpecificAnswers] NVARCHAR(1000) NULL,
    [UrgencyLevel] NVARCHAR(50) NOT NULL CONSTRAINT [DF_ServiceRequests_UrgencyLevel] DEFAULT N'Routine',
    [IsVehicleDrivable] NVARCHAR(30) NULL,
    [VehicleLocation] NVARCHAR(300) NULL,
    [AlternateContactName] NVARCHAR(200) NULL,
    [AlternateContactPhone] NVARCHAR(30) NULL,
    [ConsentAccepted] BIT NOT NULL CONSTRAINT [DF_ServiceRequests_ConsentAccepted] DEFAULT 0,
    [WantsPhotoUploadLater] BIT NOT NULL CONSTRAINT [DF_ServiceRequests_WantsPhotoUploadLater] DEFAULT 0,
    [IsArchived] BIT NOT NULL CONSTRAINT [DF_ServiceRequests_IsArchived] DEFAULT 0;
GO

CREATE INDEX [IX_ServiceRequests_IsArchived] ON [dbo].[ServiceRequests] ([IsArchived]);
GO
