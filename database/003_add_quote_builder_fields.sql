ALTER TABLE [dbo].[ServiceRequests] ADD
    [InternalQuoteNotes] NVARCHAR(4000) NULL,
    [LaborAmount] DECIMAL(8, 2) NULL,
    [PartsAmount] DECIMAL(8, 2) NULL,
    [ShopSuppliesAmount] DECIMAL(8, 2) NULL,
    [TotalEstimate] DECIMAL(8, 2) NULL,
    [QuoteTemplate] NVARCHAR(100) NULL,
    [AssumptionDisclaimerText] NVARCHAR(2000) NULL,
    [GoodOption] NVARCHAR(2000) NULL,
    [BetterOption] NVARCHAR(2000) NULL,
    [BestOption] NVARCHAR(2000) NULL,
    [QuoteExpirationDate] DATETIME2 NULL,
    [CustomerApprovalStatus] NVARCHAR(50) NOT NULL CONSTRAINT [DF_ServiceRequests_CustomerApprovalStatus] DEFAULT N'Pending';
