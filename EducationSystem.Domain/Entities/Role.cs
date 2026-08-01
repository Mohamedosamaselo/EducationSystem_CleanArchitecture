namespace EducationSystem.Domain.Entities;

public class Role : BaseAuditableEntity
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;

    // naviigational property
    public ICollection<User> Users { get; set; } = new HashSet<User>();

    public ICollection<Permission> Permissions { get; set; } = new HashSet<Permission>();
}