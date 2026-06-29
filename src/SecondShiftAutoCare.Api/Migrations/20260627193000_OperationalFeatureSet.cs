using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SecondShiftAutoCare.Api.Migrations;

public partial class OperationalFeatureSet : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<int>("EstimatedDurationMinutes", "ServiceRequests", "dbo", nullable: true);
        migrationBuilder.AddColumn<string>("PublicStatusToken", "ServiceRequests", "dbo", maxLength: 80, nullable: false, defaultValueSql: "LOWER(CONVERT(varchar(36), NEWID()))");
        migrationBuilder.AddColumn<string>("QuoteApprovalToken", "ServiceRequests", "dbo", maxLength: 80, nullable: true);
        migrationBuilder.AddColumn<string>("ScheduleNotes", "ServiceRequests", "dbo", maxLength: 1000, nullable: true);
        migrationBuilder.AddColumn<DateTime>("ScheduledEndUtc", "ServiceRequests", "dbo", nullable: true);
        migrationBuilder.AddColumn<DateTime>("ScheduledStartUtc", "ServiceRequests", "dbo", nullable: true);
        migrationBuilder.AddColumn<string>("ServiceLocationType", "ServiceRequests", "dbo", maxLength: 50, nullable: true);
        migrationBuilder.AddColumn<string>("CustomerApprovalMessage", "Quotes", "dbo", maxLength: 2000, nullable: true);
        migrationBuilder.AddColumn<DateTime>("CustomerApprovalRespondedUtc", "Quotes", "dbo", nullable: true);

        migrationBuilder.CreateTable(
            name: "ServiceIntakeQuestions",
            columns: table => new
            {
                Id = table.Column<Guid>(nullable: false),
                ServiceType = table.Column<string>(maxLength: 150, nullable: false),
                QuestionText = table.Column<string>(maxLength: 500, nullable: false),
                HelpText = table.Column<string>(maxLength: 500, nullable: true),
                AnswerType = table.Column<string>(maxLength: 50, nullable: false),
                OptionsJson = table.Column<string>(maxLength: 1000, nullable: true),
                IsRequired = table.Column<bool>(nullable: false),
                SortOrder = table.Column<int>(nullable: false),
                IsActive = table.Column<bool>(nullable: false)
            },
            schema: "dbo",
            constraints: table => table.PrimaryKey("PK_ServiceIntakeQuestions", x => x.Id)
        );

        migrationBuilder.CreateTable(
            name: "ChecklistTemplates",
            columns: table => new
            {
                Id = table.Column<Guid>(nullable: false),
                ServiceType = table.Column<string>(maxLength: 150, nullable: false),
                Name = table.Column<string>(maxLength: 150, nullable: false),
                IsActive = table.Column<bool>(nullable: false),
                SortOrder = table.Column<int>(nullable: false)
            },
            schema: "dbo",
            constraints: table => table.PrimaryKey("PK_ChecklistTemplates", x => x.Id)
        );

        migrationBuilder.CreateTable(
            name: "Payments",
            columns: table => new
            {
                Id = table.Column<Guid>(nullable: false),
                ServiceRequestId = table.Column<Guid>(nullable: false),
                PaymentStatus = table.Column<string>(maxLength: 50, nullable: false),
                PaymentMethod = table.Column<string>(maxLength: 50, nullable: true),
                AmountDue = table.Column<decimal>(type: "decimal(8,2)", nullable: true),
                AmountPaid = table.Column<decimal>(type: "decimal(8,2)", nullable: true),
                PaidUtc = table.Column<DateTime>(nullable: true),
                Notes = table.Column<string>(maxLength: 1000, nullable: true),
                CreatedUtc = table.Column<DateTime>(nullable: false, defaultValueSql: "SYSUTCDATETIME()"),
                UpdatedUtc = table.Column<DateTime>(nullable: false, defaultValueSql: "SYSUTCDATETIME()")
            },
            schema: "dbo",
            constraints: table =>
            {
                table.PrimaryKey("PK_Payments", x => x.Id);
                table.ForeignKey(
                    name: "FK_Payments_ServiceRequests_ServiceRequestId",
                    column: x => x.ServiceRequestId,
                    principalTable: "ServiceRequests",
                    principalSchema: "dbo",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            }
        );

        migrationBuilder.CreateTable(
            name: "JobRiskAssessments",
            columns: table => new
            {
                Id = table.Column<Guid>(nullable: false),
                ServiceRequestId = table.Column<Guid>(nullable: false),
                RustConcern = table.Column<bool>(nullable: false),
                RequiresLift = table.Column<bool>(nullable: false),
                CustomerSuppliedParts = table.Column<bool>(nullable: false),
                KnownDifficultVehicle = table.Column<bool>(nullable: false),
                EstimatedHours = table.Column<decimal>(type: "decimal(6,2)", nullable: true),
                SafetyCritical = table.Column<bool>(nullable: false),
                NeedsAlignment = table.Column<bool>(nullable: false),
                NeedsTow = table.Column<bool>(nullable: false),
                MobileWorkConcern = table.Column<bool>(nullable: false),
                RiskScore = table.Column<int>(nullable: false),
                Recommendation = table.Column<string>(maxLength: 50, nullable: false),
                Notes = table.Column<string>(maxLength: 1000, nullable: true),
                CreatedUtc = table.Column<DateTime>(nullable: false, defaultValueSql: "SYSUTCDATETIME()"),
                UpdatedUtc = table.Column<DateTime>(nullable: false, defaultValueSql: "SYSUTCDATETIME()")
            },
            schema: "dbo",
            constraints: table =>
            {
                table.PrimaryKey("PK_JobRiskAssessments", x => x.Id);
                table.ForeignKey(
                    name: "FK_JobRiskAssessments_ServiceRequests_ServiceRequestId",
                    column: x => x.ServiceRequestId,
                    principalTable: "ServiceRequests",
                    principalSchema: "dbo",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            }
        );

        migrationBuilder.CreateTable(
            name: "JobChecklistItems",
            columns: table => new
            {
                Id = table.Column<Guid>(nullable: false),
                ServiceRequestId = table.Column<Guid>(nullable: false),
                Text = table.Column<string>(maxLength: 300, nullable: false),
                IsCompleted = table.Column<bool>(nullable: false),
                CompletedUtc = table.Column<DateTime>(nullable: true),
                SortOrder = table.Column<int>(nullable: false),
                IsCustom = table.Column<bool>(nullable: false),
                CreatedUtc = table.Column<DateTime>(nullable: false, defaultValueSql: "SYSUTCDATETIME()"),
                UpdatedUtc = table.Column<DateTime>(nullable: false, defaultValueSql: "SYSUTCDATETIME()")
            },
            schema: "dbo",
            constraints: table =>
            {
                table.PrimaryKey("PK_JobChecklistItems", x => x.Id);
                table.ForeignKey(
                    name: "FK_JobChecklistItems_ServiceRequests_ServiceRequestId",
                    column: x => x.ServiceRequestId,
                    principalTable: "ServiceRequests",
                    principalSchema: "dbo",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            }
        );

        migrationBuilder.CreateTable(
            name: "ServiceIntakeAnswers",
            columns: table => new
            {
                Id = table.Column<Guid>(nullable: false),
                ServiceRequestId = table.Column<Guid>(nullable: false),
                QuestionId = table.Column<Guid>(nullable: true),
                QuestionText = table.Column<string>(maxLength: 500, nullable: false),
                AnswerText = table.Column<string>(maxLength: 2000, nullable: true),
                CreatedUtc = table.Column<DateTime>(nullable: false, defaultValueSql: "SYSUTCDATETIME()")
            },
            schema: "dbo",
            constraints: table =>
            {
                table.PrimaryKey("PK_ServiceIntakeAnswers", x => x.Id);
                table.ForeignKey(
                    name: "FK_ServiceIntakeAnswers_ServiceRequests_ServiceRequestId",
                    column: x => x.ServiceRequestId,
                    principalTable: "ServiceRequests",
                    principalSchema: "dbo",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
                table.ForeignKey(
                    name: "FK_ServiceIntakeAnswers_ServiceIntakeQuestions_QuestionId",
                    column: x => x.QuestionId,
                    principalTable: "ServiceIntakeQuestions",
                    principalSchema: "dbo",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.SetNull);
            }
        );

        migrationBuilder.CreateTable(
            name: "ChecklistTemplateItems",
            columns: table => new
            {
                Id = table.Column<Guid>(nullable: false),
                ChecklistTemplateId = table.Column<Guid>(nullable: false),
                Text = table.Column<string>(maxLength: 300, nullable: false),
                SortOrder = table.Column<int>(nullable: false),
                IsRequired = table.Column<bool>(nullable: false)
            },
            schema: "dbo",
            constraints: table =>
            {
                table.PrimaryKey("PK_ChecklistTemplateItems", x => x.Id);
                table.ForeignKey(
                    name: "FK_ChecklistTemplateItems_ChecklistTemplates_ChecklistTemplateId",
                    column: x => x.ChecklistTemplateId,
                    principalTable: "ChecklistTemplates",
                    principalSchema: "dbo",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            }
        );

        migrationBuilder.CreateIndex(
            name: "IX_ServiceRequests_PublicStatusToken",
            table: "ServiceRequests",
            column: "PublicStatusToken",
            schema: "dbo",
            unique: true);

        migrationBuilder.CreateIndex(
            name: "IX_ServiceRequests_QuoteApprovalToken",
            table: "ServiceRequests",
            column: "QuoteApprovalToken",
            schema: "dbo",
            unique: true,
            filter: "[QuoteApprovalToken] IS NOT NULL");

        migrationBuilder.CreateIndex(
            name: "IX_Payments_ServiceRequestId",
            table: "Payments",
            column: "ServiceRequestId",
            schema: "dbo",
            unique: true);

        migrationBuilder.CreateIndex(
            name: "IX_JobRiskAssessments_ServiceRequestId",
            table: "JobRiskAssessments",
            column: "ServiceRequestId",
            schema: "dbo",
            unique: true);

        migrationBuilder.CreateIndex(
            name: "IX_JobChecklistItems_ServiceRequestId",
            table: "JobChecklistItems",
            column: "ServiceRequestId",
            schema: "dbo");

        migrationBuilder.CreateIndex(
            name: "IX_ServiceIntakeAnswers_ServiceRequestId",
            table: "ServiceIntakeAnswers",
            column: "ServiceRequestId",
            schema: "dbo");

        migrationBuilder.CreateIndex(
            name: "IX_ServiceIntakeAnswers_QuestionId",
            table: "ServiceIntakeAnswers",
            column: "QuestionId",
            schema: "dbo");

        migrationBuilder.CreateIndex(
            name: "IX_ChecklistTemplateItems_ChecklistTemplateId",
            table: "ChecklistTemplateItems",
            column: "ChecklistTemplateId",
            schema: "dbo");
    }
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(name: "ChecklistTemplateItems", schema: "dbo");
        migrationBuilder.DropTable(name: "JobChecklistItems", schema: "dbo");
        migrationBuilder.DropTable(name: "JobRiskAssessments", schema: "dbo");
        migrationBuilder.DropTable(name: "Payments", schema: "dbo");
        migrationBuilder.DropTable(name: "ServiceIntakeAnswers", schema: "dbo");
        migrationBuilder.DropTable(name: "ChecklistTemplates", schema: "dbo");
        migrationBuilder.DropTable(name: "ServiceIntakeQuestions", schema: "dbo");

        migrationBuilder.DropIndex(name: "IX_ServiceRequests_PublicStatusToken", table: "ServiceRequests", schema: "dbo");
        migrationBuilder.DropIndex(name: "IX_ServiceRequests_QuoteApprovalToken", table: "ServiceRequests", schema: "dbo");

        migrationBuilder.DropColumn("EstimatedDurationMinutes", "ServiceRequests", "dbo");
        migrationBuilder.DropColumn("PublicStatusToken", "ServiceRequests", "dbo");
        migrationBuilder.DropColumn("QuoteApprovalToken", "ServiceRequests", "dbo");
        migrationBuilder.DropColumn("ScheduleNotes", "ServiceRequests", "dbo");
        migrationBuilder.DropColumn("ScheduledEndUtc", "ServiceRequests", "dbo");
        migrationBuilder.DropColumn("ScheduledStartUtc", "ServiceRequests", "dbo");
        migrationBuilder.DropColumn("ServiceLocationType", "ServiceRequests", "dbo");
        migrationBuilder.DropColumn("CustomerApprovalMessage", "Quotes", "dbo");
        migrationBuilder.DropColumn("CustomerApprovalRespondedUtc", "Quotes", "dbo");
    }
}
