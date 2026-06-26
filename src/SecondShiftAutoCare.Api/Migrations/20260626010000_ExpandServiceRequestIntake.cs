using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SecondShiftAutoCare.Api.Migrations;

public partial class ExpandServiceRequestIntake : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AlterColumn<string>(name: "ServiceType", schema: "dbo", table: "ServiceRequests", type: "nvarchar(1000)", maxLength: 1000, nullable: false, oldClrType: typeof(string), oldType: "nvarchar(75)", oldMaxLength: 75);
        migrationBuilder.AddColumn<string>(name: "AlternateContactName", schema: "dbo", table: "ServiceRequests", type: "nvarchar(200)", maxLength: 200, nullable: true);
        migrationBuilder.AddColumn<string>(name: "AlternateContactPhone", schema: "dbo", table: "ServiceRequests", type: "nvarchar(30)", maxLength: 30, nullable: true);
        migrationBuilder.AddColumn<bool>(name: "ConsentAccepted", schema: "dbo", table: "ServiceRequests", type: "bit", nullable: false, defaultValue: false);
        migrationBuilder.AddColumn<bool>(name: "IsArchived", schema: "dbo", table: "ServiceRequests", type: "bit", nullable: false, defaultValue: false);
        migrationBuilder.AddColumn<string>(name: "IsVehicleDrivable", schema: "dbo", table: "ServiceRequests", type: "nvarchar(30)", maxLength: 30, nullable: true);
        migrationBuilder.AddColumn<string>(name: "ServiceSpecificAnswers", schema: "dbo", table: "ServiceRequests", type: "nvarchar(1000)", maxLength: 1000, nullable: true);
        migrationBuilder.AddColumn<string>(name: "UrgencyLevel", schema: "dbo", table: "ServiceRequests", type: "nvarchar(50)", maxLength: 50, nullable: false, defaultValue: "Routine");
        migrationBuilder.AddColumn<string>(name: "VehicleLocation", schema: "dbo", table: "ServiceRequests", type: "nvarchar(300)", maxLength: 300, nullable: true);
        migrationBuilder.AddColumn<bool>(name: "WantsPhotoUploadLater", schema: "dbo", table: "ServiceRequests", type: "bit", nullable: false, defaultValue: false);
        migrationBuilder.CreateIndex(name: "IX_ServiceRequests_IsArchived", schema: "dbo", table: "ServiceRequests", column: "IsArchived");
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropIndex(name: "IX_ServiceRequests_IsArchived", schema: "dbo", table: "ServiceRequests");
        migrationBuilder.DropColumn(name: "AlternateContactName", schema: "dbo", table: "ServiceRequests");
        migrationBuilder.DropColumn(name: "AlternateContactPhone", schema: "dbo", table: "ServiceRequests");
        migrationBuilder.DropColumn(name: "ConsentAccepted", schema: "dbo", table: "ServiceRequests");
        migrationBuilder.DropColumn(name: "IsArchived", schema: "dbo", table: "ServiceRequests");
        migrationBuilder.DropColumn(name: "IsVehicleDrivable", schema: "dbo", table: "ServiceRequests");
        migrationBuilder.DropColumn(name: "ServiceSpecificAnswers", schema: "dbo", table: "ServiceRequests");
        migrationBuilder.DropColumn(name: "UrgencyLevel", schema: "dbo", table: "ServiceRequests");
        migrationBuilder.DropColumn(name: "VehicleLocation", schema: "dbo", table: "ServiceRequests");
        migrationBuilder.DropColumn(name: "WantsPhotoUploadLater", schema: "dbo", table: "ServiceRequests");
        migrationBuilder.AlterColumn<string>(name: "ServiceType", schema: "dbo", table: "ServiceRequests", type: "nvarchar(75)", maxLength: 75, nullable: false, oldClrType: typeof(string), oldType: "nvarchar(1000)", oldMaxLength: 1000);
    }
}
