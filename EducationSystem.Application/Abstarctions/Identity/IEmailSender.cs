namespace EducationSystem.Application.Abstarctions.Identity;

public interface IEmailSender
{
    Task SendAsync(string to,
                    string subject,
                    string body,
                    CancellationToken cancellationToken = default);
}