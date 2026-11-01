namespace EducationSystem.Application.Dtos.Auth;

public record LoginResponse(
    Guid Id,
    string Email,
    string Username,
    string FirstName,
    string LastName,
    string Token,
    DateTime ExpiresAt,
   string Role
);