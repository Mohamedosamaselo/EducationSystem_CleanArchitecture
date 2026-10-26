using System.ComponentModel.DataAnnotations;

namespace EducationSystem.Application.Dtos.SchoolUsers;

public class ResetSchoolUserPasswordRequest
{
    [Required, MinLength(8)]
    public string NewPassword { get; set; } = string.Empty;
}