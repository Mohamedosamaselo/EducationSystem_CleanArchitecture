using EducationSystem.Domain.Enums;
using EducationSystem.Domain.Interfaces.Common;
using Microsoft.AspNetCore.Identity;

namespace EducationSystem.Domain.Entities;

public class ApplicationUser : IdentityUser<Guid>, IBaseAuditableEntity
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public DateTime DateOfBirth { get; set; }
    public UserStatus Status { get; set; } = UserStatus.Active;

    public DateTime CreatedAt { get; set; }
    public Guid? CreatedBy { get; set; }
    public DateTime? ModifiedAt { get; set; }
    public Guid? LastModifiedBy { get; set; }

    public Guid? SchoolId { get; set; }
    public virtual School? School { get; set; }

    public Guid? GradeId { get; set; }
    public virtual Grade? Grade { get; set; }
}