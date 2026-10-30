using EducationSystem.Domain.Entities.Common;
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
    public Guid Id { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    public DateTime CreatedAt { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    public Guid? CreatedBy { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    public DateTime? ModifiedAt { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    public Guid? LastModifiedBy { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
}