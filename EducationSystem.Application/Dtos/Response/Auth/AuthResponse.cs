namespace EducationSystem.Application.Dtos.Response.Auth;

public record AuthResponse(
    Guid Id,
    string Email,
    string Username,
    string FirstName,
    string LastName,
    string Token,
    bool IsAuthenticated,
    DateTime ExpiresIn,
    //int ExpiresIn,
    List<string> Roles
);