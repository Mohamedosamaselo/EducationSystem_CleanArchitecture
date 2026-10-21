using EducationSystem.Domain.Entities.Common;

namespace EducationSystem.Domain.Entities;

public class ApplicationUser : BaseAuditableEntity
{
    // ---- Your own properties (unchanged) ----
    public string FirstName { get; set; } = string.Empty;

    public string LastName { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public DateTime? DateOfBirth { get; set; }

    /// --- Minimum Identity needs(mapped by OUR store, not IdentityDbContext) ---

    public string UserName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;   // was hidden inside IdentityUser
    public bool EmailConfirmed { get; set; }
    public string SecurityStamp { get; set; } = string.Empty;  // invalidates old reset tokens
    public string ConcurrencyStamp { get; set; } = Guid.NewGuid().ToString();

    // Role (one per user)
    public Guid RoleId { get; set; }

    public virtual ApplicationRole Role { get; set; } = null!;
    // ---- Relations ----

    // [one user for one school ]
    public Guid? SchoolId { get; set; }// Foreign key to School

    public virtual School? School { get; set; }// user in one school

    // [one user for one grade : student ]
    public Guid? GradeId { get; set; }

    public virtual Grade? Grade { get; set; }

    // [many: many teacher-grade relationships]
    public virtual ICollection<TeacherGrade> TeacherGrades { get; set; } = new List<TeacherGrade>();

    // [one user for one organisation : admin ]
    public virtual Organisation? ManagedOrganisation { get; set; } // Navigation property to Organisation (if the user is an admin of an organisation)

    public virtual ICollection<UserSubject> UserSubjects { get; set; } = new List<UserSubject>();
}