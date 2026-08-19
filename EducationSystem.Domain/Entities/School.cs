using EducationSystem.Domain.Interfaces;

namespace EducationSystem.Domain.Entities;

public class School : BaseAuditableEntity
{
    public string Name { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;

    // Foreign key
    public Guid OrganisationId { get; set; }

    // navigational property
    public Organisation Organisation { get; set; } = null!;

    public ICollection<Subject> Subjects { get; set; } = new List<Subject>();
    public ICollection<Grade> Grades { get; set; } = new List<Grade>();
    public ICollection<ApplicationUser> Users { get; set; } = new List<ApplicationUser>();
    public DateTime CreatedAt { get; set; }
    public Guid? CreatedBy { get; set; }
    public DateTime? ModifiedAt { get; set; }
    public Guid? LastModifiedBy { get; set; }
}