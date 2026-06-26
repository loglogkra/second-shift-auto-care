using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using SecondShiftAutoCare.Api.Data;

#nullable disable

namespace SecondShiftAutoCare.Api.Migrations;

[DbContext(typeof(ServiceRequestDbContext))]
partial class ServiceRequestDbContextModelSnapshot : ModelSnapshot
{
    protected override void BuildModel(ModelBuilder modelBuilder)
    {
#pragma warning disable 612, 618
        modelBuilder
            .HasAnnotation("ProductVersion", "8.0.18")
            .HasAnnotation("Relational:MaxIdentifierLength", 128);

        SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

        modelBuilder.Entity("SecondShiftAutoCare.Api.Entities.ServiceRequest", b =>
            {
                b.Property<Guid>("Id").HasColumnType("uniqueidentifier");
                b.Property<DateTime>("CreatedUtc").ValueGeneratedOnAdd().HasColumnType("datetime2").HasDefaultValueSql("SYSUTCDATETIME()");
                b.Property<string>("AssumptionDisclaimerText").HasMaxLength(2000).HasColumnType("nvarchar(2000)");
                b.Property<string>("BestOption").HasMaxLength(2000).HasColumnType("nvarchar(2000)");
                b.Property<string>("BetterOption").HasMaxLength(2000).HasColumnType("nvarchar(2000)");
                b.Property<string>("CustomerApprovalStatus").IsRequired().ValueGeneratedOnAdd().HasMaxLength(50).HasColumnType("nvarchar(50)").HasDefaultValue("Pending");
                b.Property<string>("CustomerName").IsRequired().HasMaxLength(100).HasColumnType("nvarchar(100)");
                b.Property<string>("Email").HasMaxLength(150).HasColumnType("nvarchar(150)");
                b.Property<decimal?>("EstimateHigh").HasPrecision(8, 2).HasColumnType("decimal(8,2)");
                b.Property<decimal?>("EstimateLow").HasPrecision(8, 2).HasColumnType("decimal(8,2)");
                b.Property<string>("GoodOption").HasMaxLength(2000).HasColumnType("nvarchar(2000)");
                b.Property<string>("InternalNotes").HasMaxLength(2000).HasColumnType("nvarchar(2000)");
                b.Property<string>("InternalQuoteNotes").HasMaxLength(4000).HasColumnType("nvarchar(4000)");
                b.Property<decimal?>("LaborAmount").HasPrecision(8, 2).HasColumnType("decimal(8,2)");
                b.Property<int?>("Mileage").HasColumnType("int");
                b.Property<string>("PartsNeeded").HasMaxLength(1000).HasColumnType("nvarchar(1000)");
                b.Property<decimal?>("PartsAmount").HasPrecision(8, 2).HasColumnType("decimal(8,2)");
                b.Property<string>("Phone").IsRequired().HasMaxLength(30).HasColumnType("nvarchar(30)");
                b.Property<string>("PreferredAvailability").HasMaxLength(500).HasColumnType("nvarchar(500)");
                b.Property<DateTime?>("QuoteExpirationDate").HasColumnType("datetime2");
                b.Property<string>("QuoteTemplate").HasMaxLength(100).HasColumnType("nvarchar(100)");
                b.Property<string>("ServiceType").IsRequired().HasMaxLength(1000).HasColumnType("nvarchar(75)");
                b.Property<string>("Status").IsRequired().ValueGeneratedOnAdd().HasMaxLength(50).HasColumnType("nvarchar(50)").HasDefaultValue("New");
                b.Property<decimal?>("ShopSuppliesAmount").HasPrecision(8, 2).HasColumnType("decimal(8,2)");
                b.Property<string>("Symptoms").IsRequired().HasMaxLength(2000).HasColumnType("nvarchar(2000)");
                b.Property<decimal?>("TotalEstimate").HasPrecision(8, 2).HasColumnType("decimal(8,2)");
                b.Property<DateTime>("UpdatedUtc").ValueGeneratedOnAdd().HasColumnType("datetime2").HasDefaultValueSql("SYSUTCDATETIME()");
                b.Property<string>("VehicleMake").IsRequired().HasMaxLength(75).HasColumnType("nvarchar(75)");
                b.Property<string>("VehicleModel").IsRequired().HasMaxLength(75).HasColumnType("nvarchar(75)");
                b.Property<int>("VehicleYear").HasColumnType("int");
                b.HasKey("Id");
                b.HasIndex("CreatedUtc");
                b.HasIndex("Status");
                b.ToTable("ServiceRequests", "dbo");
            });
#pragma warning restore 612, 618
    }
}
