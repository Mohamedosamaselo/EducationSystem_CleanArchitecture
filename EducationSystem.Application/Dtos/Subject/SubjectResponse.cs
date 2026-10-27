using EducationSystem.Application.Dtos.Grade;

namespace EducationSystem.Application.Dtos.Subject;

public class SubjectResponse
{
    public Guid Id { get; set; }

    public string Name { get; set; }

    //public string? Description { get; set; }

    //public Guid SchoolId { get; set; }

    //public string SchoolName { get; set; }

    public List<GradeResponse> Grades { get; set; } = new();
}