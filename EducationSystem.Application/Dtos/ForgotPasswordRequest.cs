using System.ComponentModel.DataAnnotations;

namespace EducationSystem.Application.Dtos;

public record ForgotPasswordRequest(
    [property: Required, EmailAddress]
    string Email
);