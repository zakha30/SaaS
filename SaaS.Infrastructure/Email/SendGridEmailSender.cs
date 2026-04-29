using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using SaaS.Modules.Notifications.Services;
using SendGrid;
using SendGrid.Helpers.Mail;
using System.Threading;
using System.Threading.Tasks;

namespace SaaS.Infrastructure.Email;

public sealed class SendGridEmailSender(
    IConfiguration config,
    ILogger<SendGridEmailSender> logger) : IEmailSender
{
    public async Task SendAsync(string to, string subject, string body, CancellationToken ct = default)
    {
        var apiKey = config["SendGrid:ApiKey"];
        var fromEmail = config["SendGrid:FromEmail"] ?? "noreply@yoursaas.com";

        if (string.IsNullOrWhiteSpace(apiKey))
        {
            logger.LogWarning("SendGrid API key not configured. Email to {To} skipped.", to);
            return;
        }

        var client = new SendGridClient(apiKey);
        var msg = MailHelper.CreateSingleEmail(
            new EmailAddress(fromEmail),
            new EmailAddress(to),
            subject,
            body,
            $"<p>{body}</p>");

        var response = await client.SendEmailAsync(msg, ct);

        if (!response.IsSuccessStatusCode)
            logger.LogError("SendGrid failed for {To}: {Status}", to, response.StatusCode);
    }
}
