using EducationSystem.Domain.Entities.Common;
using EducationSystem.Domain.Interfaces.Common;

namespace EducationSystem.Domain.Entities;

//public class ApplicationUser : BaseAuditableEntity
public class ApplicationUser : IBaseAuditableEntity
{
    // ---- Your own properties (unchanged) ----
    public string FirstName { get; set; } = string.Empty;

    public string LastName { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public DateTime? DateOfBirth { get; set; }

    /// --- Minimum Identity needs(mapped by OUR store, not IdentityDbContext) ---

    public string UserName { get; set; } = string.Empty;

    //public string NormalizedUserName { get; set; } = string.Empty; //
    public string Email { get; set; } = string.Empty;

    //public string NormalizedEmail { get; set; } = string.Empty; //

    public string PasswordHash { get; set; } = string.Empty;   // was hidden inside IdentityUser
    public bool EmailConfirmed { get; set; }
    public string SecurityStamp { get; set; } = string.Empty;  // invalidates old reset tokens
    public string ConcurrencyStamp { get; set; } = Guid.NewGuid().ToString();

    // Role (one per user)
    public Guid RoleId { get; set; }

    public virtual ApplicationRole Role { get; set; } = null!;
    // ---- Relations ----

    // [one user for one school ]
    public Guid? SchoolId { get; set; }

    public virtual School? School { get; set; }
    public virtual School? ManagedSchool { get; set; }// one admin managed one school

    // [one user for one grade : student ]
    public Guid? GradeId { get; set; }

    public virtual Grade? Grade { get; set; }

    // [many: many teacher-grade relationships]
    public virtual ICollection<TeacherGrade> TeacherGrades { get; set; } = new List<TeacherGrade>();

    // [one user for one organisation : admin ]
    public virtual Organisation? ManagedOrganisation { get; set; } // Navigation property to Organisation (if the user is an admin of an organisation)

    public virtual ICollection<UserSubject> UserSubjects { get; set; } = new List<UserSubject>();

    public virtual ICollection<EmailConfirmationToken> EmailConfirmationTokens { get; set; } = new List<EmailConfirmationToken>();

    // ---- IBaseAuditableEntity Implementation ----
    public Guid Id { get; set; }

    public DateTime CreatedAt { get; set; }
    public Guid? CreatedBy { get; set; }
    public DateTime? ModifiedAt { get; set; }
    public Guid? LastModifiedBy { get; set; }
}