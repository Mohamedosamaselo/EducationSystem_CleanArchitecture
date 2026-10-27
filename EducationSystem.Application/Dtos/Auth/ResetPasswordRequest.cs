using System.ComponentModel.DataAnnotations;

namespace EducationSystem.Application.Dtos.Auth;

public record ResetPasswordRequest(

    string Email,

    string Token,

    string NewPassword
);