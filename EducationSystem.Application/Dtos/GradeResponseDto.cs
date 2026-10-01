namespace EducationSystem.Application.Dtos;

public class GradeResponseDto
{
    public Guid Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string? Description { get; set; }

    public bool IsActive { get; set; }

    public Guid SchoolId { get; set; }

    public string SchoolName { get; set; } = string.Empty;
}