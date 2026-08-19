namespace EducationSystem.Domain.Interfaces.Common;

public interface IBaseAuditableEntity : IBaseEntity

{
    DateTime CreatedAt { get; set; }
    Guid? CreatedBy { get; set; }
    DateTime? ModifiedAt { get; set; }
    Guid? LastModifiedBy { get; set; }
}