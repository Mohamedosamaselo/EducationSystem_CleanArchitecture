using System.ComponentModel.DataAnnotations;

namespace EducationSystem.Application.Dtos;

public record ResetPasswordRequest(
    [property: Required, EmailAddress]
    string Email,
    [property: Required]
    string Token,
    [property: Required, MinLength(8)]
    string NewPassword
);