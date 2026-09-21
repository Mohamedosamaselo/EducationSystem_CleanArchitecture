namespace EducationSystem.Application.Dtos.Request.Auth;

public record RegisterRequest(
    string FirstName,
    string LastName,
    string Email,
    string Password,
    string Address
);