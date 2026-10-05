namespace EducationSystem.Application.Abstarctions.Services;

public interface IEmailService
{
    Task SendAsync(
        string to,
        string subject,
        string body);
}