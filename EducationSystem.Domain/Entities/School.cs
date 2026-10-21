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

    public virtual ICollection<ApplicationUser> Users { get; set; } = new List<ApplicationUser>();
    public virtual ICollection<Grade> Grades { get; set; } = new List<Grade>();
}