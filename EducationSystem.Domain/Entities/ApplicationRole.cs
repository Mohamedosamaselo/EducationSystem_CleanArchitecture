using EducationSystem.Domain.Interfaces.Common;
using Microsoft.AspNetCore.Identity;

namespace EducationSystem.Domain.Entities;

public class ApplicationRole : IdentityRole<Guid>, IBaseAuditableEntity
{
    //public bool IsDefault { get; set; }
    //public bool IsDeleted { get; set; }
    public string Name { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public Guid? CreatedBy { get; set; }
    public DateTime? ModifiedAt { get; set; }
    public Guid? LastModifiedBy { get; set; }

    // roleClaims navigation property
    public virtual ICollection<IdentityRoleClaim<Guid>> Claims { get; set; }
       = new List<IdentityRoleClaim<Guid>>();
}