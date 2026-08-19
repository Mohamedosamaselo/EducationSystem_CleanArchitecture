using EducationSystem.Domain.Interfaces.Common;
using Microsoft.AspNetCore.Identity;

namespace EducationSystem.Domain.Entities;

public class Role : IdentityRole<Guid>, IBaseAuditableEntity
{
    //public string Name { get; set; } = string.Empty; // IdentityRole Has Already Name
    public string Description { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; }
    public Guid? CreatedBy { get; set; }
    public DateTime? ModifiedAt { get; set; }
    public Guid? LastModifiedBy { get; set; }

    // naviigational property
    public ICollection<ApplicationUser> Users { get; set; } = new List<ApplicationUser>();

    public ICollection<Permission> Permissions { get; set; } = new List<Permission>();
}