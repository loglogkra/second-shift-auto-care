namespace SecondShiftAutoCare.Api.Entities;

public sealed class NotificationLog
{
    public Guid Id { get; set; }
    public Guid? ServiceRequestId { get; set; }
    public ServiceRequest? ServiceRequest { get; set; }
    public string NotificationType { get; set; } = string.Empty;
    public string Channel { get; set; } = string.Empty;
    public string Recipient { get; set; } = string.Empty;
    public string? Subject { get; set; }
    public string? BodyPreview { get; set; }
    public string Provider { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public string? ErrorMessage { get; set; }
    public DateTime CreatedUtc { get; set; }
}

public static class NotificationTypes
{
    public const string AdminNewRequest = "AdminNewRequest";
}

public static class NotificationChannels
{
    public const string Email = "Email";
    public const string SmsGatewayEmail = "SmsGatewayEmail";
}

public static class NotificationStatuses
{
    public const string Sent = "Sent";
    public const string Skipped = "Skipped";
    public const string Failed = "Failed";
}
