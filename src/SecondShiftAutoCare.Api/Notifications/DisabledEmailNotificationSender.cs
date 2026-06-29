using Microsoft.Extensions.Logging;

namespace SecondShiftAutoCare.Api.Notifications;

public sealed class DisabledEmailNotificationSender(ILogger<DisabledEmailNotificationSender> logger) : IEmailNotificationSender
{
    public Task<EmailNotificationResult> SendAsync(EmailNotificationMessage message, CancellationToken cancellationToken)
    {
        logger.LogInformation("Email notification skipped for {Recipient}: email provider not configured.", message.To);
        return Task.FromResult(new EmailNotificationResult
        {
            Success = false,
            Provider = "Disabled",
            ErrorMessage = "Email provider not configured."
        });
    }
}
