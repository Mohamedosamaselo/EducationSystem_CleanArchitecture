using System.ComponentModel.DataAnnotations;

namespace EducationSystem.Application.Dtos;

public record ResetPasswordRequest(

    string Email,

    string Token,

    string NewPassword
);