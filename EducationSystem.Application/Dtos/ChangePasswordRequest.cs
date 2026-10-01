namespace EducationSystem.Application.Dtos;

public record ChangePasswordRequest
(
    string CurrentPassword,
    string NewPassword
    );