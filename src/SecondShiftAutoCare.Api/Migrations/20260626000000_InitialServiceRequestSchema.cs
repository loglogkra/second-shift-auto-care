using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SecondShiftAutoCare.Api.Migrations;

public partial class InitialServiceRequestSchema : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "ServiceRequests",
            schema: "dbo",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                CustomerName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                Phone = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                Email = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                VehicleYear = table.Column<int>(type: "int", nullable: false),
                VehicleMake = table.Column<string>(type: "nvarchar(75)", maxLength: 75, nullable: false),
                VehicleModel = table.Column<string>(type: "nvarchar(75)", maxLength: 75, nullable: false),
                Mileage = table.Column<int>(type: "int", nullable: true),
                ServiceType = table.Column<string>(type: "nvarchar(75)", maxLength: 75, nullable: false),
                Symptoms = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: false),
                PreferredAvailability = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                Status = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false, defaultValue: "New"),
                EstimateLow = table.Column<decimal>(type: "decimal(8,2)", precision: 8, scale: 2, nullable: true),
                EstimateHigh = table.Column<decimal>(type: "decimal(8,2)", precision: 8, scale: 2, nullable: true),
                PartsNeeded = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                InternalNotes = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                CreatedUtc = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "SYSUTCDATETIME()"),
                UpdatedUtc = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "SYSUTCDATETIME()")
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_ServiceRequests", x => x.Id);
            });

        migrationBuilder.CreateIndex(
            name: "IX_ServiceRequests_CreatedUtc",
            schema: "dbo",
            table: "ServiceRequests",
            column: "CreatedUtc");

        migrationBuilder.CreateIndex(
            name: "IX_ServiceRequests_Status",
            schema: "dbo",
            table: "ServiceRequests",
            column: "Status");
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "ServiceRequests",
            schema: "dbo");
    }
}
