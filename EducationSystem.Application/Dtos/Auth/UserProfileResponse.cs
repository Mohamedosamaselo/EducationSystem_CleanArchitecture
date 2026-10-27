namespace EducationSystem.Application.Dtos.Auth;

public record UserProfileResponse
(
    string Email,
    string UserName,
    string FirstName,
    string LastName
);