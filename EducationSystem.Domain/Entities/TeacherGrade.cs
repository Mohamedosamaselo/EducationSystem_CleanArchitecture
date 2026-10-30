using EducationSystem.Domain.Entities.Common;
using EducationSystem.Domain.Interfaces.Common;

namespace EducationSystem.Domain.Entities;

//public class TeacherGrade : BaseAuditableEntity
public class TeacherGrade : IBaseAuditableEntity
{
    public Guid TeacherId { get; set; }
    public virtual ApplicationUser Teacher { get; set; } = null!;

    public Guid GradeId { get; set; }
    public virtual Grade Grade { get; set; } = null!;
    public Guid Id { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    public DateTime CreatedAt { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    public Guid? CreatedBy { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    public DateTime? ModifiedAt { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    public Guid? LastModifiedBy { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
}