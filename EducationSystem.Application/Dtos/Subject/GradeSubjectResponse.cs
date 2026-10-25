namespace EducationSystem.Application.Dtos.Subject;

public sealed class GradeSubjectResponse
{
    public Guid GradeId { get; init; }
    public string GradeName { get; init; } = string.Empty;
    public Guid SubjectId { get; init; }
    public string SubjectName { get; init; } = string.Empty;
}