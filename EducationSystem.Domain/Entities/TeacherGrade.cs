using EducationSystem.Domain.Interfaces.Common;

namespace EducationSystem.Domain.Entities;

//public class TeacherGrade : BaseAuditableEntity
public class TeacherGrade : IBaseAuditableEntity
{
    public Guid TeacherId { get; set; }
    public virtual ApplicationUser Teacher { get; set; } = null!;

    public Guid GradeId { get; set; }
    public virtual Grade Grade { get; set; } = null!;
    public Guid Id { get; set; }
    public DateTime CreatedAt { get; set; }
    public Guid? CreatedBy { get; set; }
    public DateTime? ModifiedAt { get; set; }
    public Guid? LastModifiedBy { get; set; }
}