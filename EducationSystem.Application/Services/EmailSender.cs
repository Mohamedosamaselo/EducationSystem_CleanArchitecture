using EducationSystem.Application.Abstarctions.Identity;
using Microsoft.Extensions.Logging;

namespace EducationSystem.Application.Services;

public class EmailSender : IEmailSender
{
    private readonly ILogger<EmailSender> _logger;

    public EmailSender(ILogger<EmailSender> logger)
    {
        _logger = logger;
    }

    public Task SendAsync(string to, string subject, string body, CancellationToken cancellationToken = default)
    {
        // TODO: replace with MailKit / SendGrid / SMTP later.
        // For now we "send" emails by writing them to the console log.
        _logger.LogInformation(
            "---- EMAIL ----\nTo: {To}\nSubject: {Subject}\nBody:\n{Body}\n---------------",
            to, subject, body);

        return Task.CompletedTask;
    }
}