namespace EducationSystem.Domain.Interfaces.Common;

public interface IBaseAuditableEntity

{
    Guid Id { get; set; }
    DateTime CreatedAt { get; set; }
    Guid? CreatedBy { get; set; }
    DateTime? ModifiedAt { get; set; }
    Guid? LastModifiedBy { get; set; }
}