using System.ComponentModel.DataAnnotations;

namespace EducationSystem.Application.Dtos.SchoolUsers;

public class CreateSchoolUserRequest
{
    [Required]
    public string FirstName { get; set; } = string.Empty;

    [Required]
    public string LastName { get; set; } = string.Empty;

    [Required, EmailAddress]
    public string Email { get; set; } = string.Empty;

    public string UserName { get; set; } = string.Empty;

    [Required, MinLength(8)]
    public string Password { get; set; } = string.Empty;

    public string? Address { get; set; }

    public DateTime? DateOfBirth { get; set; }
}