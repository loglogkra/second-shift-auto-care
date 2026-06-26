using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SecondShiftAutoCare.Api.Migrations;

public partial class AddQuoteBuilderFields : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<string>(name: "AssumptionDisclaimerText", schema: "dbo", table: "ServiceRequests", type: "nvarchar(2000)", maxLength: 2000, nullable: true);
        migrationBuilder.AddColumn<string>(name: "BestOption", schema: "dbo", table: "ServiceRequests", type: "nvarchar(2000)", maxLength: 2000, nullable: true);
        migrationBuilder.AddColumn<string>(name: "BetterOption", schema: "dbo", table: "ServiceRequests", type: "nvarchar(2000)", maxLength: 2000, nullable: true);
        migrationBuilder.AddColumn<string>(name: "CustomerApprovalStatus", schema: "dbo", table: "ServiceRequests", type: "nvarchar(50)", maxLength: 50, nullable: false, defaultValue: "Pending");
        migrationBuilder.AddColumn<string>(name: "GoodOption", schema: "dbo", table: "ServiceRequests", type: "nvarchar(2000)", maxLength: 2000, nullable: true);
        migrationBuilder.AddColumn<string>(name: "InternalQuoteNotes", schema: "dbo", table: "ServiceRequests", type: "nvarchar(4000)", maxLength: 4000, nullable: true);
        migrationBuilder.AddColumn<decimal>(name: "LaborAmount", schema: "dbo", table: "ServiceRequests", type: "decimal(8,2)", precision: 8, scale: 2, nullable: true);
        migrationBuilder.AddColumn<decimal>(name: "PartsAmount", schema: "dbo", table: "ServiceRequests", type: "decimal(8,2)", precision: 8, scale: 2, nullable: true);
        migrationBuilder.AddColumn<DateTime>(name: "QuoteExpirationDate", schema: "dbo", table: "ServiceRequests", type: "datetime2", nullable: true);
        migrationBuilder.AddColumn<string>(name: "QuoteTemplate", schema: "dbo", table: "ServiceRequests", type: "nvarchar(100)", maxLength: 100, nullable: true);
        migrationBuilder.AddColumn<decimal>(name: "ShopSuppliesAmount", schema: "dbo", table: "ServiceRequests", type: "decimal(8,2)", precision: 8, scale: 2, nullable: true);
        migrationBuilder.AddColumn<decimal>(name: "TotalEstimate", schema: "dbo", table: "ServiceRequests", type: "decimal(8,2)", precision: 8, scale: 2, nullable: true);
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropColumn(name: "AssumptionDisclaimerText", schema: "dbo", table: "ServiceRequests");
        migrationBuilder.DropColumn(name: "BestOption", schema: "dbo", table: "ServiceRequests");
        migrationBuilder.DropColumn(name: "BetterOption", schema: "dbo", table: "ServiceRequests");
        migrationBuilder.DropColumn(name: "CustomerApprovalStatus", schema: "dbo", table: "ServiceRequests");
        migrationBuilder.DropColumn(name: "GoodOption", schema: "dbo", table: "ServiceRequests");
        migrationBuilder.DropColumn(name: "InternalQuoteNotes", schema: "dbo", table: "ServiceRequests");
        migrationBuilder.DropColumn(name: "LaborAmount", schema: "dbo", table: "ServiceRequests");
        migrationBuilder.DropColumn(name: "PartsAmount", schema: "dbo", table: "ServiceRequests");
        migrationBuilder.DropColumn(name: "QuoteExpirationDate", schema: "dbo", table: "ServiceRequests");
        migrationBuilder.DropColumn(name: "QuoteTemplate", schema: "dbo", table: "ServiceRequests");
        migrationBuilder.DropColumn(name: "ShopSuppliesAmount", schema: "dbo", table: "ServiceRequests");
        migrationBuilder.DropColumn(name: "TotalEstimate", schema: "dbo", table: "ServiceRequests");
    }
}
