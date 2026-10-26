namespace EducationSystem.Application.Dtos.SchoolUsers;

public class SchoolUserResponseDto
{
    public Guid Id { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? Address { get; set; }
    public DateTime? DateOfBirth { get; set; }
    public string Role { get; set; } = string.Empty;
    public Guid SchoolId { get; set; }
}