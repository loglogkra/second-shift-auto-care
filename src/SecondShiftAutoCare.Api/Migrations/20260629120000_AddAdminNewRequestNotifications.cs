using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SecondShiftAutoCare.Api.Migrations;

public partial class AddAdminNewRequestNotifications : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "NotificationLogs",
            schema: "dbo",
            columns: table => new
            {
                Id = table.Column<Guid>(nullable: false),
                ServiceRequestId = table.Column<Guid>(nullable: true),
                NotificationType = table.Column<string>(maxLength: 80, nullable: false),
                Channel = table.Column<string>(maxLength: 50, nullable: false),
                Recipient = table.Column<string>(maxLength: 320, nullable: false),
                Subject = table.Column<string>(maxLength: 500, nullable: true),
                BodyPreview = table.Column<string>(maxLength: 1000, nullable: true),
                Provider = table.Column<string>(maxLength: 80, nullable: false),
                Status = table.Column<string>(maxLength: 50, nullable: false),
                ErrorMessage = table.Column<string>(maxLength: 1000, nullable: true),
                CreatedUtc = table.Column<DateTime>(nullable: false, defaultValueSql: "SYSUTCDATETIME()")
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_NotificationLogs", x => x.Id);
                table.ForeignKey(
                    name: "FK_NotificationLogs_ServiceRequests_ServiceRequestId",
                    column: x => x.ServiceRequestId,
                    principalSchema: "dbo",
                    principalTable: "ServiceRequests",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.SetNull);
            });

        migrationBuilder.CreateIndex("IX_NotificationLogs_ServiceRequestId", "NotificationLogs", "ServiceRequestId", schema: "dbo");
        migrationBuilder.CreateIndex("IX_NotificationLogs_NotificationType", "NotificationLogs", "NotificationType", schema: "dbo");
        migrationBuilder.CreateIndex("IX_NotificationLogs_Status", "NotificationLogs", "Status", schema: "dbo");
        migrationBuilder.CreateIndex("IX_NotificationLogs_CreatedUtc", "NotificationLogs", "CreatedUtc", schema: "dbo");
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(name: "NotificationLogs", schema: "dbo");
    }
}
