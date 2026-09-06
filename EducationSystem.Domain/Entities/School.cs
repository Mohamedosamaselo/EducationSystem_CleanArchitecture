namespace EducationSystem.Domain.Entities;

using EducationSystem.Domain.Enums;

public class School : BaseAuditableEntity
{
    public string Name { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public SchoolStatus Status { get; set; } = SchoolStatus.Active;
    public string LogoUrl { get; set; } = string.Empty;

    // Foreign key
    public Guid OrganisationId { get; set; }

    // Navigational property
    public Organisation Organisation { get; set; } = null!;

    public ICollection<Subject> Subjects { get; set; } = new List<Subject>();
    public ICollection<Grade> Grades { get; set; } = new List<Grade>();
    public ICollection<ApplicationUser> Users { get; set; } = new List<ApplicationUser>();
}