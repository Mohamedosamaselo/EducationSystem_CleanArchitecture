using EducationSystem.Domain.Entities.Common;

namespace EducationSystem.Domain.Entities;

public class TeacherGrade : BaseEntity
{
    public Guid TeacherId { get; set; }
    public virtual ApplicationUser Teacher { get; set; } = null!;

    public Guid GradeId { get; set; }
    public virtual Grade Grade { get; set; } = null!;
}