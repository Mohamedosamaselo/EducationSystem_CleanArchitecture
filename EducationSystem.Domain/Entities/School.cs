namespace EducationSystem.Domain.Entities;

using EducationSystem.Domain.Entities.Common;

public class School : BaseAuditableEntity
{
    public string Name { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;

    public Guid OrganisationId { get; set; }// Foreign key to Organisatio

    public virtual Organisation Organisation { get; set; } = null!;// Navigation property to Organisation

    // Foreign key to the School Admin
    public Guid? SchoolAdminId { get; set; }

    public virtual ApplicationUser SchoolAdmin { get; set; } = null!; // Navigation property to School Admin

    public virtual ICollection<ApplicationUser> Users { get; set; } = new List<ApplicationUser>(); // Navigation property to Users in the School

    public virtual ICollection<Grade> Grades { get; set; } = new List<Grade>();
}