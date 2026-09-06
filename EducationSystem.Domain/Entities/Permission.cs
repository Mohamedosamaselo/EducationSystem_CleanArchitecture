using EducationSystem.Domain.Enums;

namespace EducationSystem.Domain.Entities;

public class Permission : BaseAuditableEntity
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public PermissionStatus Status { get; set; } = PermissionStatus.Active;

    // Navigational property
    public ICollection<RolePermission> RolePermissions { get; set; } = new List<RolePermission>();
}