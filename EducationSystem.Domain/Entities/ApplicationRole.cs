using EducationSystem.Domain.Enums;
using EducationSystem.Domain.Interfaces.Common;
using Microsoft.AspNetCore.Identity;

namespace EducationSystem.Domain.Entities;

public class ApplicationRole : IdentityRole<Guid>, IBaseAuditableEntity
{
    public bool IsDefault { get; set; }
    public bool IsDeleted { get; set; }
    public string Description { get; set; } = string.Empty;

    //public string Name { get; set; } = string.Empty;
    //public RoleStatus Status { get; set; } = RoleStatus.Active;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public Guid? CreatedBy { get; set; }
    public DateTime? ModifiedAt { get; set; }
    public Guid? LastModifiedBy { get; set; }

    //// Navigational property
    //public virtual ICollection<RolePermission> RolePermissions { get; set; }
    //    = new List<RolePermission>();
}