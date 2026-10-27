using EducationSystem.Domain.Entities.Common;

namespace EducationSystem.Domain.Entities;

public class Grade : BaseAuditableEntity
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }

    // F K
    public Guid SchoolId { get; set; }

    // navigational property
    public virtual School School { get; set; } = null!;

    public virtual ICollection<Subject> Subjects { get; set; } = new List<Subject>(); // M : M [Grade , Subject](GradeSubject)
    public virtual ICollection<ApplicationUser> Students { get; set; } = new List<ApplicationUser>();// 1:M [Grade , StudentsApplicationUser]
    public virtual ICollection<TeacherGrade> TeacherGrades { get; set; } = new List<TeacherGrade>();// M : M [Grade , Teacher](TeacherGrade)
}