using System.ComponentModel.DataAnnotations;

namespace EducationSystem.Application.Dtos.Auth;

public class RegisterRequestDto
{
    [Required, MaxLength(50)]
    public string Name { get; set; } = string.Empty;

    [Required, MaxLength(50)]
    public string Username { get; set; } = string.Empty;

    public string Address { get; set; } = string.Empty;

    [Required, EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required, MaxLength(100)]
    public string Password { get; set; } = string.Empty;

    public Guid SchoolId { get; set; }
    public Guid GradeId { get; set; }
}