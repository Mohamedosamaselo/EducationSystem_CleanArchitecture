using System.ComponentModel.DataAnnotations;

namespace EducationSystem.Application.Dtos;

public record ForgotPasswordRequest(

    [Required]
    [EmailAddress]
    string Email
);