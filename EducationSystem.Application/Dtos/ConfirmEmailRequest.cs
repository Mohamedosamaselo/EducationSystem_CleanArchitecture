namespace EducationSystem.Application.Dtos;

public record ConfirmEmailRequest
(
    string UserId,
    string Token
);