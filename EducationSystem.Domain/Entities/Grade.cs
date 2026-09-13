using EducationSystem.Domain.Entities.Common;

namespace EducationSystem.Domain.Entities;

public class Grade : BaseAuditableEntity
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public bool IsActive { get; set; }

    // Foreign key
    public Guid SchoolId { get; set; }

    // navigational property
    public virtual School School { get; set; } = null!;

    public virtual ICollection<Subject> Subjects { get; set; } = new List<Subject>();
    public virtual ICollection<ApplicationUser> Users { get; set; } = new List<ApplicationUser>();
}