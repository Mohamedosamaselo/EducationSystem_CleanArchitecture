using EducationSystem.Domain.Entities.Common;
using EducationSystem.Domain.Interfaces.Common;

namespace EducationSystem.Domain.Entities;

//public class Grade : BaseAuditableEntity
public class Grade : IBaseAuditableEntity
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
    public Guid Id { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    public DateTime CreatedAt { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    public Guid? CreatedBy { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    public DateTime? ModifiedAt { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    public Guid? LastModifiedBy { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
}