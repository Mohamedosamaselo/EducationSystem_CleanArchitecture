using EducationSystem.Domain.Interfaces.Common;
using Microsoft.AspNetCore.Identity;

namespace EducationSystem.Domain.Entities;

public class ApplicationUser : IdentityUser<Guid>, IBaseAuditableEntity
{
    public string Name { get; set; }
    public string Address { get; set; }
    public DateTime DateOfBirth { get; set; }

    // Auditing
    public DateTime CreatedAt { get; set; }

    public Guid? CreatedBy { get; set; }
    public DateTime? ModifiedAt { get; set; }
    public Guid? LastModifiedBy { get; set; }

    // Foreign keys
    public Guid SchoolId { get; set; }

    public Guid GradeId { get; set; }

    // navigational property
    public School School { get; set; } = null!;

    public Grade Grade { get; set; } = null!;
    public ICollection<Role> Roles { get; set; } = new List<Role>();
}