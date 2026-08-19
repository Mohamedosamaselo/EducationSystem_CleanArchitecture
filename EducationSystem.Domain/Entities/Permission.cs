using EducationSystem.Domain.Interfaces;

namespace EducationSystem.Domain.Entities;

public class Permission : BaseAuditableEntity
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;

    // Audit
    public DateTime CreatedAt { get; set; }

    public Guid? CreatedBy { get; set; }
    public DateTime? ModifiedAt { get; set; }
    public Guid? LastModifiedBy { get; set; }

    // Foreign key
    public Guid RoleId { get; set; }

    // navigational property
    public Role Role { get; set; } = null!;
}