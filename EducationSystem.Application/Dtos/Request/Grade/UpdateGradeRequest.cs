namespace EducationSystem.Application.Dtos.Request.Grade;

public class UpdateGradeRequest
{
    public string Name { get; set; } = string.Empty;

    public string? Description { get; set; }

    public bool IsActive { get; set; }
}