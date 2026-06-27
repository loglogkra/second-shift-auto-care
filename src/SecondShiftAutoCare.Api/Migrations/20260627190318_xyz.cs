using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SecondShiftAutoCare.Api.Migrations
{
    /// <inheritdoc />
    public partial class xyz : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "EstimatedDurationMinutes",
                schema: "dbo",
                table: "ServiceRequests",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PublicStatusToken",
                schema: "dbo",
                table: "ServiceRequests",
                type: "nvarchar(80)",
                maxLength: 80,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "QuoteApprovalToken",
                schema: "dbo",
                table: "ServiceRequests",
                type: "nvarchar(80)",
                maxLength: 80,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ScheduleNotes",
                schema: "dbo",
                table: "ServiceRequests",
                type: "nvarchar(1000)",
                maxLength: 1000,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ScheduledEndUtc",
                schema: "dbo",
                table: "ServiceRequests",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ScheduledStartUtc",
                schema: "dbo",
                table: "ServiceRequests",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ServiceLocationType",
                schema: "dbo",
                table: "ServiceRequests",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CustomerApprovalMessage",
                schema: "dbo",
                table: "Quotes",
                type: "nvarchar(2000)",
                maxLength: 2000,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CustomerApprovalRespondedUtc",
                schema: "dbo",
                table: "Quotes",
                type: "datetime2",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "ChecklistTemplates",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ServiceType = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    SortOrder = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChecklistTemplates", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "JobChecklistItems",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ServiceRequestId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Text = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: false),
                    IsCompleted = table.Column<bool>(type: "bit", nullable: false),
                    CompletedUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    SortOrder = table.Column<int>(type: "int", nullable: false),
                    IsCustom = table.Column<bool>(type: "bit", nullable: false),
                    CreatedUtc = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "SYSUTCDATETIME()"),
                    UpdatedUtc = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "SYSUTCDATETIME()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_JobChecklistItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_JobChecklistItems_ServiceRequests_ServiceRequestId",
                        column: x => x.ServiceRequestId,
                        principalSchema: "dbo",
                        principalTable: "ServiceRequests",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "JobRiskAssessments",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ServiceRequestId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RustConcern = table.Column<bool>(type: "bit", nullable: false),
                    RequiresLift = table.Column<bool>(type: "bit", nullable: false),
                    CustomerSuppliedParts = table.Column<bool>(type: "bit", nullable: false),
                    KnownDifficultVehicle = table.Column<bool>(type: "bit", nullable: false),
                    EstimatedHours = table.Column<decimal>(type: "decimal(6,2)", precision: 6, scale: 2, nullable: true),
                    SafetyCritical = table.Column<bool>(type: "bit", nullable: false),
                    NeedsAlignment = table.Column<bool>(type: "bit", nullable: false),
                    NeedsTow = table.Column<bool>(type: "bit", nullable: false),
                    MobileWorkConcern = table.Column<bool>(type: "bit", nullable: false),
                    RiskScore = table.Column<int>(type: "int", nullable: false),
                    Recommendation = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    CreatedUtc = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "SYSUTCDATETIME()"),
                    UpdatedUtc = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "SYSUTCDATETIME()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_JobRiskAssessments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_JobRiskAssessments_ServiceRequests_ServiceRequestId",
                        column: x => x.ServiceRequestId,
                        principalSchema: "dbo",
                        principalTable: "ServiceRequests",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Payments",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ServiceRequestId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PaymentStatus = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    PaymentMethod = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    AmountDue = table.Column<decimal>(type: "decimal(8,2)", precision: 8, scale: 2, nullable: true),
                    AmountPaid = table.Column<decimal>(type: "decimal(8,2)", precision: 8, scale: 2, nullable: true),
                    PaidUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    CreatedUtc = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "SYSUTCDATETIME()"),
                    UpdatedUtc = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "SYSUTCDATETIME()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Payments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Payments_ServiceRequests_ServiceRequestId",
                        column: x => x.ServiceRequestId,
                        principalSchema: "dbo",
                        principalTable: "ServiceRequests",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ServiceIntakeQuestions",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ServiceType = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    QuestionText = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    HelpText = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    AnswerType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    OptionsJson = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    IsRequired = table.Column<bool>(type: "bit", nullable: false),
                    SortOrder = table.Column<int>(type: "int", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ServiceIntakeQuestions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ChecklistTemplateItems",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ChecklistTemplateId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Text = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: false),
                    SortOrder = table.Column<int>(type: "int", nullable: false),
                    IsRequired = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChecklistTemplateItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ChecklistTemplateItems_ChecklistTemplates_ChecklistTemplateId",
                        column: x => x.ChecklistTemplateId,
                        principalSchema: "dbo",
                        principalTable: "ChecklistTemplates",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ServiceIntakeAnswers",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ServiceRequestId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    QuestionId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    QuestionText = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    AnswerText = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    CreatedUtc = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "SYSUTCDATETIME()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ServiceIntakeAnswers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ServiceIntakeAnswers_ServiceIntakeQuestions_QuestionId",
                        column: x => x.QuestionId,
                        principalSchema: "dbo",
                        principalTable: "ServiceIntakeQuestions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_ServiceIntakeAnswers_ServiceRequests_ServiceRequestId",
                        column: x => x.ServiceRequestId,
                        principalSchema: "dbo",
                        principalTable: "ServiceRequests",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ServiceRequests_PublicStatusToken",
                schema: "dbo",
                table: "ServiceRequests",
                column: "PublicStatusToken",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ServiceRequests_QuoteApprovalToken",
                schema: "dbo",
                table: "ServiceRequests",
                column: "QuoteApprovalToken",
                unique: true,
                filter: "[QuoteApprovalToken] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_ChecklistTemplateItems_ChecklistTemplateId",
                schema: "dbo",
                table: "ChecklistTemplateItems",
                column: "ChecklistTemplateId");

            migrationBuilder.CreateIndex(
                name: "IX_ChecklistTemplates_ServiceType_IsActive_SortOrder",
                schema: "dbo",
                table: "ChecklistTemplates",
                columns: new[] { "ServiceType", "IsActive", "SortOrder" });

            migrationBuilder.CreateIndex(
                name: "IX_JobChecklistItems_ServiceRequestId",
                schema: "dbo",
                table: "JobChecklistItems",
                column: "ServiceRequestId");

            migrationBuilder.CreateIndex(
                name: "IX_JobRiskAssessments_ServiceRequestId",
                schema: "dbo",
                table: "JobRiskAssessments",
                column: "ServiceRequestId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Payments_ServiceRequestId",
                schema: "dbo",
                table: "Payments",
                column: "ServiceRequestId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ServiceIntakeAnswers_QuestionId",
                schema: "dbo",
                table: "ServiceIntakeAnswers",
                column: "QuestionId");

            migrationBuilder.CreateIndex(
                name: "IX_ServiceIntakeAnswers_ServiceRequestId",
                schema: "dbo",
                table: "ServiceIntakeAnswers",
                column: "ServiceRequestId");

            migrationBuilder.CreateIndex(
                name: "IX_ServiceIntakeQuestions_ServiceType_IsActive_SortOrder",
                schema: "dbo",
                table: "ServiceIntakeQuestions",
                columns: new[] { "ServiceType", "IsActive", "SortOrder" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ChecklistTemplateItems",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "JobChecklistItems",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "JobRiskAssessments",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "Payments",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "ServiceIntakeAnswers",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "ChecklistTemplates",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "ServiceIntakeQuestions",
                schema: "dbo");

            migrationBuilder.DropIndex(
                name: "IX_ServiceRequests_PublicStatusToken",
                schema: "dbo",
                table: "ServiceRequests");

            migrationBuilder.DropIndex(
                name: "IX_ServiceRequests_QuoteApprovalToken",
                schema: "dbo",
                table: "ServiceRequests");

            migrationBuilder.DropColumn(
                name: "EstimatedDurationMinutes",
                schema: "dbo",
                table: "ServiceRequests");

            migrationBuilder.DropColumn(
                name: "PublicStatusToken",
                schema: "dbo",
                table: "ServiceRequests");

            migrationBuilder.DropColumn(
                name: "QuoteApprovalToken",
                schema: "dbo",
                table: "ServiceRequests");

            migrationBuilder.DropColumn(
                name: "ScheduleNotes",
                schema: "dbo",
                table: "ServiceRequests");

            migrationBuilder.DropColumn(
                name: "ScheduledEndUtc",
                schema: "dbo",
                table: "ServiceRequests");

            migrationBuilder.DropColumn(
                name: "ScheduledStartUtc",
                schema: "dbo",
                table: "ServiceRequests");

            migrationBuilder.DropColumn(
                name: "ServiceLocationType",
                schema: "dbo",
                table: "ServiceRequests");

            migrationBuilder.DropColumn(
                name: "CustomerApprovalMessage",
                schema: "dbo",
                table: "Quotes");

            migrationBuilder.DropColumn(
                name: "CustomerApprovalRespondedUtc",
                schema: "dbo",
                table: "Quotes");
        }
    }
}
