using System.ComponentModel.DataAnnotations;

namespace EducationSystem.Application.Dtos.Auth;

public record ForgotPasswordRequest(

    [Required]
    [EmailAddress]
    string Email
);