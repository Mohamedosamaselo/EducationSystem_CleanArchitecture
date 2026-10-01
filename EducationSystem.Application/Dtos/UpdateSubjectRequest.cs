namespace EducationSystem.Application.Dtos;

public class UpdateSubjectRequest
{
    public string Name { get; set; } = string.Empty;

    public string? Description { get; set; }

    public Guid SchoolId { get; set; }
}