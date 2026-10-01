namespace EducationSystem.Application.Abstarctions.Services;

public interface ICurrentUserService
{
    Guid? UserId { get; }
}