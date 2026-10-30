namespace EducationSystem.Application.Dtos.Auth;

public record RegisterResponse
(
    bool IsSuccess,
    string? Message,
    bool IsConfirmed,
    string? Token,
    bool IsAuthenticated,
    DateTime ExpiresIn,
    string Role
);