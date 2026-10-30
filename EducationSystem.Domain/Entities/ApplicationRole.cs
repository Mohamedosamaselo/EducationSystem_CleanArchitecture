using EducationSystem.Domain.Entities.Common;
using EducationSystem.Domain.Interfaces.Common;
using Microsoft.AspNetCore.Identity;

namespace EducationSystem.Domain.Entities;

public class ApplicationRole : IdentityRole<Guid>, IBaseAuditableEntity
{
    //public string Name { get; set; } = string.Empty!; //This is unnecessary because IdentityRole<Guid> already defines Name.

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public Guid? CreatedBy { get; set; }
    public DateTime? ModifiedAt { get; set; }
    public Guid? LastModifiedBy { get; set; }

    // roleClaims navigation property
    public virtual ICollection<IdentityRoleClaim<Guid>> Claims { get; set; } = new List<IdentityRoleClaim<Guid>>();
}