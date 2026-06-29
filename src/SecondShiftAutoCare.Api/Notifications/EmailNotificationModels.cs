namespace SecondShiftAutoCare.Api.Notifications;

public sealed class EmailNotificationMessage
{
    public required string To { get; init; }
    public required string Subject { get; init; }
    public required string HtmlBody { get; init; }
    public required string TextBody { get; init; }
}

public sealed class EmailNotificationResult
{
    public bool Success { get; init; }
    public required string Provider { get; init; }
    public string? ErrorMessage { get; init; }
    public string? ProviderMessageId { get; init; }
}

public interface IEmailNotificationSender
{
    Task<EmailNotificationResult> SendAsync(EmailNotificationMessage message, CancellationToken cancellationToken);
}
