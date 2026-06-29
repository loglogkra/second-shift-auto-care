using Microsoft.Azure.Functions.Worker;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SecondShiftAutoCare.Api;
using SecondShiftAutoCare.Api.Data;
using SecondShiftAutoCare.Api.Notifications;

var host = new HostBuilder()
    .ConfigureFunctionsWorkerDefaults()
    .ConfigureServices((context, services) =>
    {
        services.AddApplicationInsightsTelemetryWorkerService();
        services.ConfigureFunctionsApplicationInsights();
        services.AddDbContext<ServiceRequestDbContext>(options =>
        {
            var connectionString = context.Configuration["SqlConnectionString"];
            if (string.IsNullOrWhiteSpace(connectionString))
            {
                throw new InvalidOperationException("SqlConnectionString is not configured. Add it to the Azure Function App application settings.");
            }

            options.UseSqlServer(connectionString, sqlOptions =>
                sqlOptions.EnableRetryOnFailure(
                    maxRetryCount: 5,
                    maxRetryDelay: TimeSpan.FromSeconds(3),
                    errorNumbersToAdd: null));
        });
        services.AddScoped<ServiceRequestRepository>();
        services.AddScoped<IAdminNotificationService, AdminNotificationService>();
        if (string.IsNullOrWhiteSpace(context.Configuration["ResendApiKey"]))
        {
            services.AddScoped<IEmailNotificationSender, DisabledEmailNotificationSender>();
        }
        else
        {
            services.AddSingleton<HttpClient>();
            services.AddScoped<IEmailNotificationSender, ResendEmailNotificationSender>();
        }
    })
    .Build();

host.Run();
