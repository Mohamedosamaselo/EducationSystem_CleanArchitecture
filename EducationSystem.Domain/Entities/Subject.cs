using EducationSystem.Domain.Interfaces.Common;

namespace EducationSystem.Domain.Entities;

//public class Subject : BaseAuditableEntity
public class Subject : IBaseAuditableEntity
{
    public string Name { get; set; } = string.Empty;

    // M : M [Subject , Grade](GradeSubject)
    public virtual ICollection<Grade> Grades { get; set; } = new List<Grade>();

    //public virtual ICollection<ApplicationUser> Students { get; set; } = new List<ApplicationUser>();// M:M[subject , student/teacher]

    public virtual ICollection<UserSubject> UserSubjects { get; set; } = new List<UserSubject>();
    public Guid Id { get; set; }
    public DateTime CreatedAt { get; set; }
    public Guid? CreatedBy { get; set; }
    public DateTime? ModifiedAt { get; set; }
    public Guid? LastModifiedBy { get; set; }
}