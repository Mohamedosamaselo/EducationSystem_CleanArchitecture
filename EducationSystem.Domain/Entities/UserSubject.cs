using EducationSystem.Domain.Entities.Common;
using EducationSystem.Domain.Interfaces.Common;

namespace EducationSystem.Domain.Entities;

//public class UserSubject : BaseAuditableEntity
public class UserSubject : IBaseAuditableEntity
{
    public Guid UserId { get; set; }
    public virtual ApplicationUser User { get; set; } = null!;

    public Guid SubjectId { get; set; }
    public virtual Subject Subject { get; set; } = null!;

    public string? FinalEvaluation { get; set; } = string.Empty;
    public Guid Id { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    public DateTime CreatedAt { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    public Guid? CreatedBy { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    public DateTime? ModifiedAt { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    public Guid? LastModifiedBy { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
}