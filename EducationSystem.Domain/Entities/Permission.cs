namespace EducationSystem.Domain.Entities;

public class Permission : BaseAuditableEntity
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;

    // Foreign key
    public Guid RoleId { get; set; }

    // navigational property
    public Role Role { get; set; } = null!;
}