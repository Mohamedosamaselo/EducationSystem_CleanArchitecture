namespace EducationSystem.Application.Dtos.Request.Auth;

public record RegisterRequestDto(
    string FirstName,
    string LastName,
    string Email,
    string Password
);