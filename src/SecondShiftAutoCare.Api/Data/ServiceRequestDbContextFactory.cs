using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace SecondShiftAutoCare.Api.Data;

public sealed class ServiceRequestDbContextFactory : IDesignTimeDbContextFactory<ServiceRequestDbContext>
{
    public ServiceRequestDbContext CreateDbContext(string[] args)
    {
        var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("local.settings.json", optional: true)
            .AddUserSecrets<ServiceRequestDbContextFactory>(optional: true)
            .AddEnvironmentVariables()
            .Build();

        // In local.settings.json the value is nested under "Values"; the Functions host
        // promotes it to a flat key at runtime, but at design time we must read it directly.
        var connectionString = configuration["Values:SqlConnectionString"]
            ?? configuration["SqlConnectionString"];
        if (string.IsNullOrWhiteSpace(connectionString))
        {
            throw new InvalidOperationException("SqlConnectionString is not configured. Add it to local.settings.json, user secrets, or the Azure Function App application settings.");
        }

        var options = new DbContextOptionsBuilder<ServiceRequestDbContext>()
            .UseSqlServer(connectionString)
            .Options;

        return new ServiceRequestDbContext(options);
    }
}
