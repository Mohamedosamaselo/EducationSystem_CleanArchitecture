using EducationSystem.Domain.Interfaces.Common;

namespace EducationSystem.Domain.Entities.Common;

public abstract class BaseAuditableEntity : BaseEntity, IBaseAuditableEntity
{
    public DateTime CreatedAt { get; set; }
    public Guid? CreatedBy { get; set; }
    public DateTime? ModifiedAt { get; set; }
    public Guid? LastModifiedBy { get; set; }
}