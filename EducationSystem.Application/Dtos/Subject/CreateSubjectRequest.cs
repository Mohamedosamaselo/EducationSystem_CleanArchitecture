namespace EducationSystem.Application.Dtos.Subject;

public class CreateSubjectRequest
{
    public string Name { get; set; } = string.Empty;

    public string? Description { get; set; }

    public Guid GradeId { get; set; }
    public Guid SchoolId { get; set; }
}