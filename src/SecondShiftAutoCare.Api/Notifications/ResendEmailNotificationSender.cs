using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace SecondShiftAutoCare.Api.Notifications;

public sealed class ResendEmailNotificationSender(HttpClient httpClient, IConfiguration configuration, ILogger<ResendEmailNotificationSender> logger) : IEmailNotificationSender
{
    private const string ProviderName = "Resend";

    public async Task<EmailNotificationResult> SendAsync(EmailNotificationMessage message, CancellationToken cancellationToken)
    {
        var apiKey = configuration["ResendApiKey"];
        if (string.IsNullOrWhiteSpace(apiKey))
        {
            logger.LogInformation("Resend notification skipped for {Recipient}: ResendApiKey is not configured.", message.To);
            return new EmailNotificationResult { Success = false, Provider = ProviderName, ErrorMessage = "Email provider not configured." };
        }

        var fromEmail = configuration["NotificationFromEmail"];
        if (string.IsNullOrWhiteSpace(fromEmail))
        {
            logger.LogWarning("Resend notification skipped for {Recipient}: NotificationFromEmail is not configured.", message.To);
            return new EmailNotificationResult { Success = false, Provider = ProviderName, ErrorMessage = "NotificationFromEmail is not configured." };
        }

        var fromName = configuration["NotificationFromName"];
        var from = string.IsNullOrWhiteSpace(fromName) ? fromEmail : $"{fromName} <{fromEmail}>";
        using var request = new HttpRequestMessage(HttpMethod.Post, "https://api.resend.com/emails");
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);
        request.Content = JsonContent.Create(new ResendEmailRequest(from, [message.To], message.Subject, message.HtmlBody, message.TextBody));

        try
        {
            using var response = await httpClient.SendAsync(request, cancellationToken);
            var resendResponse = await response.Content.ReadFromJsonAsync<ResendEmailResponse>(cancellationToken: cancellationToken);
            if (response.IsSuccessStatusCode)
            {
                logger.LogInformation("Resend notification sent to {Recipient} with provider message id {ProviderMessageId}.", message.To, resendResponse?.Id);
                return new EmailNotificationResult { Success = true, Provider = ProviderName, ProviderMessageId = resendResponse?.Id };
            }

            var safeError = $"Resend returned {(int)response.StatusCode} {response.ReasonPhrase}.";
            logger.LogWarning("Resend notification failed for {Recipient}: {Error}", message.To, safeError);
            return new EmailNotificationResult { Success = false, Provider = ProviderName, ErrorMessage = safeError, ProviderMessageId = resendResponse?.Id };
        }
        catch (Exception ex) when (ex is HttpRequestException or TaskCanceledException)
        {
            logger.LogWarning(ex, "Resend notification failed for {Recipient}.", message.To);
            return new EmailNotificationResult { Success = false, Provider = ProviderName, ErrorMessage = ex.Message };
        }
    }

    private sealed record ResendEmailRequest(
        [property: JsonPropertyName("from")] string From,
        [property: JsonPropertyName("to")] string[] To,
        [property: JsonPropertyName("subject")] string Subject,
        [property: JsonPropertyName("html")] string Html,
        [property: JsonPropertyName("text")] string Text);
    private sealed record ResendEmailResponse([property: JsonPropertyName("id")] string? Id);
}
