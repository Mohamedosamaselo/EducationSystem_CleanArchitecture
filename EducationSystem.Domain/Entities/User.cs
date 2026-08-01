namespace EducationSystem.Domain.Entities;

public class User : BaseAuditableEntity
{
    public string Name { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public string UserName { get; set; } = string.Empty;

    public string PasswordHash { get; set; } = string.Empty;

    public string PhoneNumber { get; set; } = string.Empty;

    public string Address { get; set; } = string.Empty;

    public DateTime DateOfBirth { get; set; }

    // Foreign key
    public Guid SchoolId { get; set; }

    public Guid GradeId { get; set; }

    // navigatiobal property
    public School School { get; set; } = null!;

    public Grade Grade { get; set; } = null!;

    public ICollection<Role> Roles { get; set; } = new HashSet<Role>();
}