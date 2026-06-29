using Microsoft.EntityFrameworkCore;
using SecondShiftAutoCare.Api.Entities;
using SecondShiftAutoCare.Shared.Models;

namespace SecondShiftAutoCare.Api.Data;

public sealed class ServiceRequestDbContext(DbContextOptions<ServiceRequestDbContext> options) : DbContext(options)
{
    public DbSet<Customer> Customers => Set<Customer>();
    public DbSet<Vehicle> Vehicles => Set<Vehicle>();
    public DbSet<ServiceRequest> ServiceRequests => Set<ServiceRequest>();
    public DbSet<Quote> Quotes => Set<Quote>();
    public DbSet<ServiceCatalogItem> ServiceCatalogItems => Set<ServiceCatalogItem>();
    public DbSet<VehicleMake> VehicleMakes => Set<VehicleMake>();
    public DbSet<VehicleModel> VehicleModels => Set<VehicleModel>();
    public DbSet<ServiceIntakeQuestion> ServiceIntakeQuestions => Set<ServiceIntakeQuestion>();
    public DbSet<ServiceIntakeAnswer> ServiceIntakeAnswers => Set<ServiceIntakeAnswer>();
    public DbSet<ChecklistTemplate> ChecklistTemplates => Set<ChecklistTemplate>();
    public DbSet<ChecklistTemplateItem> ChecklistTemplateItems => Set<ChecklistTemplateItem>();
    public DbSet<JobChecklistItem> JobChecklistItems => Set<JobChecklistItem>();
    public DbSet<Payment> Payments => Set<Payment>();
    public DbSet<JobRiskAssessment> JobRiskAssessments => Set<JobRiskAssessment>();
    public DbSet<NotificationLog> NotificationLogs => Set<NotificationLog>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Customer>(e =>
        {
            e.ToTable("Customers", "dbo"); e.HasKey(x => x.Id); e.Property(x => x.Id).ValueGeneratedNever();
            e.Property(x => x.Name).HasMaxLength(100).IsRequired(); e.Property(x => x.Phone).HasMaxLength(30).IsRequired(); e.Property(x => x.Email).HasMaxLength(150);
            e.Property(x => x.CreatedUtc).HasDefaultValueSql("SYSUTCDATETIME()").IsRequired(); e.Property(x => x.UpdatedUtc).HasDefaultValueSql("SYSUTCDATETIME()").IsRequired();
            e.HasIndex(x => x.Phone); e.HasIndex(x => x.Email);
        });
        modelBuilder.Entity<Vehicle>(e =>
        {
            e.ToTable("Vehicles", "dbo"); e.HasKey(x => x.Id); e.Property(x => x.Id).ValueGeneratedNever();
            e.Property(x => x.Make).HasMaxLength(75).IsRequired(); e.Property(x => x.Model).HasMaxLength(75).IsRequired();
            e.Property(x => x.CreatedUtc).HasDefaultValueSql("SYSUTCDATETIME()").IsRequired(); e.Property(x => x.UpdatedUtc).HasDefaultValueSql("SYSUTCDATETIME()").IsRequired();
            e.HasOne(x => x.Customer).WithMany(x => x.Vehicles).HasForeignKey(x => x.CustomerId).OnDelete(DeleteBehavior.Restrict);
            e.HasIndex(x => x.CustomerId); e.HasIndex(x => x.Year); e.HasIndex(x => new { x.Make, x.Model });
        });
        modelBuilder.Entity<ServiceRequest>(e =>
        {
            e.ToTable("ServiceRequests", "dbo"); e.HasKey(x => x.Id); e.Property(x => x.Id).ValueGeneratedNever();
            e.Property(x => x.ServiceType).HasMaxLength(1000).IsRequired(); e.Property(x => x.ServiceSpecificAnswers).HasMaxLength(1000); e.Property(x => x.Symptoms).HasMaxLength(2000); e.Property(x => x.Notes).HasMaxLength(2000); e.Property(x => x.PreferredAvailability).HasMaxLength(500);
            e.Property(x => x.PublicStatusToken).HasMaxLength(80).IsRequired(); e.Property(x => x.QuoteApprovalToken).HasMaxLength(80); e.Property(x => x.ServiceLocationType).HasMaxLength(50); e.Property(x => x.ScheduleNotes).HasMaxLength(1000); e.HasIndex(x => x.PublicStatusToken).IsUnique(); e.HasIndex(x => x.QuoteApprovalToken).IsUnique().HasFilter("[QuoteApprovalToken] IS NOT NULL");
            e.Property(x => x.UrgencyLevel).HasMaxLength(50).HasDefaultValue(ServiceRequestUrgencyLevels.Routine).IsRequired(); e.Property(x => x.IsVehicleDrivable).HasMaxLength(30); e.Property(x => x.VehicleLocation).HasMaxLength(300);
            e.Property(x => x.ConsentAccepted).HasDefaultValue(false).IsRequired(); e.Property(x => x.WantsPhotoUploadLater).HasDefaultValue(false).IsRequired(); e.Property(x => x.IsArchived).HasDefaultValue(false).IsRequired(); e.Property(x => x.Status).HasMaxLength(50).HasDefaultValue(ServiceRequestStatuses.New).IsRequired(); e.Property(x => x.InternalNotes).HasMaxLength(2000);
            e.Property(x => x.CreatedUtc).HasDefaultValueSql("SYSUTCDATETIME()").IsRequired(); e.Property(x => x.UpdatedUtc).HasDefaultValueSql("SYSUTCDATETIME()").IsRequired();
            e.HasOne(x => x.Customer).WithMany(x => x.ServiceRequests).HasForeignKey(x => x.CustomerId).OnDelete(DeleteBehavior.Restrict); e.HasOne(x => x.Vehicle).WithMany(x => x.ServiceRequests).HasForeignKey(x => x.VehicleId).OnDelete(DeleteBehavior.Restrict); e.HasOne(x => x.Quote).WithOne(x => x.ServiceRequest).HasForeignKey<Quote>(x => x.ServiceRequestId).OnDelete(DeleteBehavior.Cascade);
            e.HasIndex(x => x.CreatedUtc); e.HasIndex(x => x.Status); e.HasIndex(x => x.IsArchived); e.HasIndex(x => x.CustomerId); e.HasIndex(x => x.VehicleId);
        });
        modelBuilder.Entity<Quote>(e =>
        {
            e.ToTable("Quotes", "dbo"); e.HasKey(x => x.Id); e.Property(x => x.Id).ValueGeneratedNever();
            e.Property(x => x.EstimateLow).HasPrecision(8,2); e.Property(x => x.EstimateHigh).HasPrecision(8,2); e.Property(x => x.LaborAmount).HasPrecision(8,2); e.Property(x => x.PartsAmount).HasPrecision(8,2); e.Property(x => x.ShopSuppliesAmount).HasPrecision(8,2); e.Property(x => x.TotalEstimate).HasPrecision(8,2);
            e.Property(x => x.PartsNeeded).HasMaxLength(1000); e.Property(x => x.InternalQuoteNotes).HasMaxLength(4000); e.Property(x => x.QuoteTemplate).HasMaxLength(100); e.Property(x => x.AssumptionDisclaimerText).HasMaxLength(2000); e.Property(x => x.GoodOption).HasMaxLength(2000); e.Property(x => x.BetterOption).HasMaxLength(2000); e.Property(x => x.BestOption).HasMaxLength(2000); e.Property(x => x.CustomerApprovalStatus).HasMaxLength(50).HasDefaultValue(ServiceRequestApprovalStatuses.Pending).IsRequired();
            e.Property(x => x.CreatedUtc).HasDefaultValueSql("SYSUTCDATETIME()").IsRequired(); e.Property(x => x.UpdatedUtc).HasDefaultValueSql("SYSUTCDATETIME()").IsRequired(); e.Property(x => x.CustomerApprovalMessage).HasMaxLength(2000); e.HasIndex(x => x.ServiceRequestId).IsUnique();
        });

        modelBuilder.Entity<ServiceIntakeQuestion>(e => { e.ToTable("ServiceIntakeQuestions", "dbo"); e.HasKey(x => x.Id); e.Property(x => x.Id).ValueGeneratedNever(); e.Property(x => x.ServiceType).HasMaxLength(150).IsRequired(); e.Property(x => x.QuestionText).HasMaxLength(500).IsRequired(); e.Property(x => x.HelpText).HasMaxLength(500); e.Property(x => x.AnswerType).HasMaxLength(50).IsRequired(); e.Property(x => x.OptionsJson).HasMaxLength(1000); e.HasIndex(x => new { x.ServiceType, x.IsActive, x.SortOrder }); });
        modelBuilder.Entity<ServiceIntakeAnswer>(e => { e.ToTable("ServiceIntakeAnswers", "dbo"); e.HasKey(x => x.Id); e.Property(x => x.Id).ValueGeneratedNever(); e.Property(x => x.QuestionText).HasMaxLength(500).IsRequired(); e.Property(x => x.AnswerText).HasMaxLength(2000); e.Property(x => x.CreatedUtc).HasDefaultValueSql("SYSUTCDATETIME()").IsRequired(); e.HasOne(x => x.ServiceRequest).WithMany(x => x.IntakeAnswers).HasForeignKey(x => x.ServiceRequestId).OnDelete(DeleteBehavior.Cascade); e.HasOne(x => x.Question).WithMany(x => x.Answers).HasForeignKey(x => x.QuestionId).OnDelete(DeleteBehavior.SetNull); e.HasIndex(x => x.ServiceRequestId); });
        modelBuilder.Entity<ChecklistTemplate>(e => { e.ToTable("ChecklistTemplates", "dbo"); e.HasKey(x => x.Id); e.Property(x => x.Id).ValueGeneratedNever(); e.Property(x => x.ServiceType).HasMaxLength(150).IsRequired(); e.Property(x => x.Name).HasMaxLength(150).IsRequired(); e.HasIndex(x => new { x.ServiceType, x.IsActive, x.SortOrder }); });
        modelBuilder.Entity<ChecklistTemplateItem>(e => { e.ToTable("ChecklistTemplateItems", "dbo"); e.HasKey(x => x.Id); e.Property(x => x.Id).ValueGeneratedNever(); e.Property(x => x.Text).HasMaxLength(300).IsRequired(); e.HasOne(x => x.ChecklistTemplate).WithMany(x => x.Items).HasForeignKey(x => x.ChecklistTemplateId).OnDelete(DeleteBehavior.Cascade); });
        modelBuilder.Entity<JobChecklistItem>(e => { e.ToTable("JobChecklistItems", "dbo"); e.HasKey(x => x.Id); e.Property(x => x.Id).ValueGeneratedNever(); e.Property(x => x.Text).HasMaxLength(300).IsRequired(); e.Property(x => x.CreatedUtc).HasDefaultValueSql("SYSUTCDATETIME()").IsRequired(); e.Property(x => x.UpdatedUtc).HasDefaultValueSql("SYSUTCDATETIME()").IsRequired(); e.HasOne(x => x.ServiceRequest).WithMany(x => x.ChecklistItems).HasForeignKey(x => x.ServiceRequestId).OnDelete(DeleteBehavior.Cascade); e.HasIndex(x => x.ServiceRequestId); });
        modelBuilder.Entity<Payment>(e => { e.ToTable("Payments", "dbo"); e.HasKey(x => x.Id); e.Property(x => x.Id).ValueGeneratedNever(); e.Property(x => x.PaymentStatus).HasMaxLength(50).IsRequired(); e.Property(x => x.PaymentMethod).HasMaxLength(50); e.Property(x => x.AmountDue).HasPrecision(8,2); e.Property(x => x.AmountPaid).HasPrecision(8,2); e.Property(x => x.Notes).HasMaxLength(1000); e.Property(x => x.CreatedUtc).HasDefaultValueSql("SYSUTCDATETIME()").IsRequired(); e.Property(x => x.UpdatedUtc).HasDefaultValueSql("SYSUTCDATETIME()").IsRequired(); e.HasOne(x => x.ServiceRequest).WithOne(x => x.Payment).HasForeignKey<Payment>(x => x.ServiceRequestId).OnDelete(DeleteBehavior.Cascade); e.HasIndex(x => x.ServiceRequestId).IsUnique(); });
        modelBuilder.Entity<JobRiskAssessment>(e => { e.ToTable("JobRiskAssessments", "dbo"); e.HasKey(x => x.Id); e.Property(x => x.Id).ValueGeneratedNever(); e.Property(x => x.EstimatedHours).HasPrecision(6,2); e.Property(x => x.Recommendation).HasMaxLength(50).IsRequired(); e.Property(x => x.Notes).HasMaxLength(1000); e.Property(x => x.CreatedUtc).HasDefaultValueSql("SYSUTCDATETIME()").IsRequired(); e.Property(x => x.UpdatedUtc).HasDefaultValueSql("SYSUTCDATETIME()").IsRequired(); e.HasOne(x => x.ServiceRequest).WithOne(x => x.RiskAssessment).HasForeignKey<JobRiskAssessment>(x => x.ServiceRequestId).OnDelete(DeleteBehavior.Cascade); e.HasIndex(x => x.ServiceRequestId).IsUnique(); });

        modelBuilder.Entity<NotificationLog>(e =>
        {
            e.ToTable("NotificationLogs", "dbo"); e.HasKey(x => x.Id); e.Property(x => x.Id).ValueGeneratedNever();
            e.Property(x => x.NotificationType).HasMaxLength(80).IsRequired(); e.Property(x => x.Channel).HasMaxLength(50).IsRequired(); e.Property(x => x.Recipient).HasMaxLength(320).IsRequired();
            e.Property(x => x.Subject).HasMaxLength(500); e.Property(x => x.BodyPreview).HasMaxLength(1000); e.Property(x => x.Provider).HasMaxLength(80).IsRequired(); e.Property(x => x.Status).HasMaxLength(50).IsRequired(); e.Property(x => x.ErrorMessage).HasMaxLength(1000);
            e.Property(x => x.CreatedUtc).HasDefaultValueSql("SYSUTCDATETIME()").IsRequired();
            e.HasOne(x => x.ServiceRequest).WithMany(x => x.NotificationLogs).HasForeignKey(x => x.ServiceRequestId).OnDelete(DeleteBehavior.SetNull);
            e.HasIndex(x => x.ServiceRequestId); e.HasIndex(x => x.NotificationType); e.HasIndex(x => x.Status); e.HasIndex(x => x.CreatedUtc);
        });
        modelBuilder.Entity<ServiceCatalogItem>(e => { e.ToTable("ServiceCatalogItems", "dbo"); e.HasKey(x => x.Id); e.Property(x => x.Name).HasMaxLength(150).IsRequired(); e.Property(x => x.Description).HasMaxLength(1000); e.HasIndex(x => x.Name).IsUnique(); });
        modelBuilder.Entity<VehicleMake>(e => { e.ToTable("VehicleMakes", "dbo"); e.HasKey(x => x.Id); e.Property(x => x.Name).HasMaxLength(75).IsRequired(); e.HasIndex(x => x.Name).IsUnique(); });
        modelBuilder.Entity<VehicleModel>(e => { e.ToTable("VehicleModels", "dbo"); e.HasKey(x => x.Id); e.Property(x => x.Name).HasMaxLength(75).IsRequired(); e.HasOne(x => x.VehicleMake).WithMany(x => x.Models).HasForeignKey(x => x.VehicleMakeId).OnDelete(DeleteBehavior.Cascade); e.HasIndex(x => x.VehicleMakeId); e.HasIndex(x => x.Name); });
    }
}
