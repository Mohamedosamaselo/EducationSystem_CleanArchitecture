namespace EducationSystem.Application.Dtos.Auth;

public record ConfirmEmailRequest
(
    string UserId,
    string Token
);