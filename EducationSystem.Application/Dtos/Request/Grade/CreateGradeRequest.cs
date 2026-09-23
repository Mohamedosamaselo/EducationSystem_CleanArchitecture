namespace EducationSystem.Application.Dtos.Request.Grade;

public class CreateGradeRequest
{
    public string Name { get; set; } = string.Empty;

    public string? Description { get; set; }

    public Guid SchoolId { get; set; }

    public bool IsActive { get; set; }
}