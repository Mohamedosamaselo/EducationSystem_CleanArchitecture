using EducationSystem.Application.Abstarctions.Services;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Configuration;
using MimeKit;

namespace EducationSystem.Application.Services;

public class MailKitEmailService : IEmailService
{
    private readonly IConfiguration _configuration;

    public MailKitEmailService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public async Task SendAsync(
        string to,
        string subject,
        string body)
    {
        var message = new MimeMessage();

        message.From.Add(
            new MailboxAddress(
                _configuration["Email:SenderName"],
                _configuration["Email:SenderEmail"]!));

        message.To.Add(
            MailboxAddress.Parse(to));

        message.Subject = subject;

        message.Body = new TextPart("html")
        {
            Text = body
        };

        using var client = new MailKit.Net.Smtp.SmtpClient();

        client.CheckCertificateRevocation = false;   // add this line

        await client.ConnectAsync(
                _configuration["Email:Server"]!,
                int.Parse(_configuration["Email:Port"]!),
                SecureSocketOptions.StartTls);

        await client.AuthenticateAsync(
                _configuration["Email:Account"]!,
                _configuration["Email:Password"]!);

        await client.SendAsync(message);

        await client.DisconnectAsync(true);
    }
}