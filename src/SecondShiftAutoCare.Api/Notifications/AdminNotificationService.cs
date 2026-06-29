using System.Net;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using SecondShiftAutoCare.Api.Data;
using SecondShiftAutoCare.Api.Entities;

namespace SecondShiftAutoCare.Api.Notifications;

public interface IAdminNotificationService
{
    Task NotifyNewServiceRequestAsync(Guid serviceRequestId, CancellationToken cancellationToken);
}

public sealed class AdminNotificationService(ServiceRequestDbContext dbContext, IEmailNotificationSender emailSender, IConfiguration configuration, ILogger<AdminNotificationService> logger) : IAdminNotificationService
{
    public async Task NotifyNewServiceRequestAsync(Guid serviceRequestId, CancellationToken cancellationToken)
    {
        if (!bool.TryParse(configuration["EnableAdminNewRequestNotifications"], out var enabled) || !enabled)
        {
            logger.LogInformation("Admin new request notification skipped for {ServiceRequestId}: EnableAdminNewRequestNotifications is not enabled.", serviceRequestId);
            return;
        }

        var adminEmail = configuration["AdminNotificationEmail"];
        if (string.IsNullOrWhiteSpace(adminEmail))
        {
            logger.LogWarning("Admin new request notification skipped for {ServiceRequestId}: AdminNotificationEmail is not configured.", serviceRequestId);
            return;
        }

        var serviceRequest = await dbContext.ServiceRequests.AsNoTracking()
            .Include(x => x.Customer).Include(x => x.Vehicle).Include(x => x.Quote)
            .SingleOrDefaultAsync(x => x.Id == serviceRequestId, cancellationToken);
        if (serviceRequest is null)
        {
            logger.LogWarning("Admin new request notification skipped: service request {ServiceRequestId} was not found.", serviceRequestId);
            return;
        }

        var subject = $"New service request: {serviceRequest.ServiceType} - {serviceRequest.Vehicle.Year} {serviceRequest.Vehicle.Make} {serviceRequest.Vehicle.Model}";
        var textBody = BuildTextBody(serviceRequest);
        var htmlBody = BuildHtmlBody(textBody);
        await SendAndLogAsync(serviceRequest.Id, adminEmail, subject, htmlBody, textBody, NotificationChannels.Email, cancellationToken);

        var smsEmail = configuration["AdminNotificationSmsEmail"];
        if (!string.IsNullOrWhiteSpace(smsEmail))
        {
            await SendAndLogAsync(serviceRequest.Id, smsEmail, subject, htmlBody, ShortTextBody(serviceRequest), NotificationChannels.SmsGatewayEmail, cancellationToken);
        }
    }

    private async Task SendAndLogAsync(Guid serviceRequestId, string recipient, string subject, string htmlBody, string textBody, string channel, CancellationToken cancellationToken)
    {
        var result = await emailSender.SendAsync(new EmailNotificationMessage { To = recipient, Subject = subject, HtmlBody = htmlBody, TextBody = textBody }, cancellationToken);
        dbContext.NotificationLogs.Add(new NotificationLog
        {
            Id = Guid.NewGuid(), ServiceRequestId = serviceRequestId, NotificationType = NotificationTypes.AdminNewRequest, Channel = channel,
            Recipient = recipient, Subject = subject, BodyPreview = textBody.Length > 1000 ? textBody[..1000] : textBody,
            Provider = result.Provider, Status = result.Success ? NotificationStatuses.Sent : result.Provider == "Disabled" ? NotificationStatuses.Skipped : NotificationStatuses.Failed, ErrorMessage = result.ErrorMessage, CreatedUtc = DateTime.UtcNow
        });
        await dbContext.SaveChangesAsync(cancellationToken);
        if (result.Success) logger.LogInformation("Admin new request notification sent to {Recipient} for {ServiceRequestId} via {Provider}.", recipient, serviceRequestId, result.Provider);
        else logger.LogWarning("Admin new request notification failed for {Recipient} and {ServiceRequestId}: {Error}", recipient, serviceRequestId, result.ErrorMessage);
    }

    private string BuildTextBody(ServiceRequest r)
    {
        var baseUrl = GetBaseUrl();
        var adminLink = string.IsNullOrWhiteSpace(baseUrl) ? $"/admin/requests/{r.Id}" : $"{baseUrl}/admin/requests/{r.Id}";
        var statusLink = string.IsNullOrWhiteSpace(r.PublicStatusToken) ? null : (string.IsNullOrWhiteSpace(baseUrl) ? $"/request-status/{r.PublicStatusToken}" : $"{baseUrl}/request-status/{r.PublicStatusToken}");
        var sb = new StringBuilder();
        sb.AppendLine("A new service request was submitted.");
        sb.AppendLine();
        sb.AppendLine($"Customer: {r.Customer.Name}");
        sb.AppendLine($"Phone: {r.Customer.Phone}");
        if (!string.IsNullOrWhiteSpace(r.Customer.Email)) sb.AppendLine($"Email: {r.Customer.Email}");
        sb.AppendLine($"Vehicle: {r.Vehicle.Year} {r.Vehicle.Make} {r.Vehicle.Model}");
        if (r.Vehicle.Mileage is not null) sb.AppendLine($"Mileage: {r.Vehicle.Mileage}");
        sb.AppendLine($"Service type: {r.ServiceType}");
        if (!string.IsNullOrWhiteSpace(r.Symptoms)) sb.AppendLine($"Symptoms/notes: {r.Symptoms}");
        if (!string.IsNullOrWhiteSpace(r.PreferredAvailability)) sb.AppendLine($"Preferred availability: {r.PreferredAvailability}");
        sb.AppendLine($"Created UTC: {r.CreatedUtc:O}");
        sb.AppendLine($"Admin detail link: {adminLink}");
        if (statusLink is not null) sb.AppendLine($"Customer status link: {statusLink}");
        return sb.ToString();
    }

    private static string ShortTextBody(ServiceRequest r) => $"New request: {r.ServiceType} - {r.Vehicle.Year} {r.Vehicle.Make} {r.Vehicle.Model}. Customer: {r.Customer.Name}, {r.Customer.Phone}.";
    private static string BuildHtmlBody(string textBody) => $"<pre style=\"font-family:Arial,sans-serif;white-space:pre-wrap\">{WebUtility.HtmlEncode(textBody)}</pre>";
    private string? GetBaseUrl()
    {
        var baseUrl = configuration["PublicAppBaseUrl"]?.Trim().TrimEnd('/');
        if (string.IsNullOrWhiteSpace(baseUrl)) logger.LogWarning("PublicAppBaseUrl is not configured; notification links will be relative paths.");
        return baseUrl;
    }
}
