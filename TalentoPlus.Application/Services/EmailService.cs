using System.Net;
using System.Net.Mail;
using Microsoft.Extensions.Options;
using TalentoPlus.Domain.Entities;

namespace TalentoPlus.Application.Services;

public class EmailService
{
    private readonly EmailSettings _settings;

    public EmailService(IOptions<EmailSettings> settings)
    {
        _settings = settings.Value;
    }

    public async Task sendAsync(string toEmail, string subject, string body)
    {
        using var client = new SmtpClient(_settings.Host, _settings.Port)
        {
            EnableSsl = _settings.EnableSsl,
            Credentials = new NetworkCredential(_settings.UserName, _settings.Password)
        };

        using var message = new MailMessage
        {
            From = new MailAddress(_settings.FromEmail, _settings.FromName),
            Subject = subject,
            Body = body,
            IsBodyHtml = false
        };
        
        message.To.Add(new MailAddress(toEmail));

        await Task.Run(() => client.Send(message));
    }
}