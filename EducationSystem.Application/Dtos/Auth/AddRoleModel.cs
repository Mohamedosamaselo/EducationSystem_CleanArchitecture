using System.ComponentModel.DataAnnotations;

namespace EducationSystem.Application.Dtos.Auth;

public class AddRoleModel
{
    [Required]
    public string UserId { get; set; } = string.Empty;

    [Required]
    public string RoleName { get; set; } = string.Empty;
}