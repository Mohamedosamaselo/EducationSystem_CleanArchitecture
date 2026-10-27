namespace EducationSystem.Application.Dtos.Grade;

public class GradeResponse
{
    public Guid Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string? Description { get; set; }

    //public bool IsActive { get; set; }

    public Guid SchoolId { get; set; }

    public string SchoolName { get; set; } = string.Empty;
}