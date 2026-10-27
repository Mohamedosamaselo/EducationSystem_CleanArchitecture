namespace EducationSystem.Application.Dtos.Auth;

public record ChangePasswordRequest
(
    string CurrentPassword,
    string NewPassword
    );