using System.Net.Mail;
using Microsoft.Extensions.Configuration;

namespace HybridShop.Services.Notification.Services;

public class EmailService : IEmailService
{
    private readonly IConfiguration _configuration;

    public EmailService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public async Task SendEmailAsync(string to, string subject, string body, CancellationToken cancellationToken = default)
    {
        var cleanTo = to?.Trim();
        if (string.IsNullOrWhiteSpace(cleanTo))
        {
            return;
        }

        var smtpHost = _configuration["Smtp:Host"] ?? "mailpit";
        var smtpPort = int.Parse(_configuration["Smtp:Port"] ?? "1025");
        var rawFrom = _configuration["Smtp:From"];
        
        var fromEmail = !string.IsNullOrWhiteSpace(rawFrom) 
            ? rawFrom.Trim().Trim('"').Trim('\'') 
            : "noreply@hybridshop.com";

        if (!MailAddress.TryCreate(fromEmail, out var fromMailAddress))
        {
            fromMailAddress = new MailAddress("noreply@hybridshop.com");
        }

        if (!MailAddress.TryCreate(cleanTo, out var toMailAddress))
        {
            return;
        }

        using var client = new SmtpClient(smtpHost, smtpPort);
        client.EnableSsl = false;

        var cleanSubject = subject?.Replace("\r", "").Replace("\n", "").Trim() ?? string.Empty;

        var mailMessage = new MailMessage
        {
            From = fromMailAddress,
            Subject = cleanSubject,
            Body = body,
            IsBodyHtml = true
        };

        mailMessage.To.Add(toMailAddress);

        await client.SendMailAsync(mailMessage, cancellationToken);
    }
}