using EducationSystem.Domain.Enums;
using EducationSystem.Domain.Interfaces.Common;
using Microsoft.AspNetCore.Identity;

namespace EducationSystem.Domain.Entities;

public class ApplicationUser : IdentityUser<Guid>, IBaseAuditableEntity
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Address { get; set; } = string.Empty;
    public DateTime DateOfBirth { get; set; }
    public UserStatus Status { get; set; } = UserStatus.Active;

    // Auditing
    public DateTime CreatedAt { get; set; }

    public Guid? CreatedBy { get; set; }
    public DateTime? ModifiedAt { get; set; }
    public Guid? LastModifiedBy { get; set; }

    // Foreign keys (nullable to allow users without a school/grade)
    public Guid? SchoolId { get; set; }

    public Guid? GradeId { get; set; }

    // Navigational properties (nullable because FKs are optional)
    public School? School { get; set; }

    public Grade? Grade { get; set; }
}