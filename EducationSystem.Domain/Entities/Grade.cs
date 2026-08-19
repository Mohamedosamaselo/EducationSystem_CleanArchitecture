using EducationSystem.Domain.Interfaces;

namespace EducationSystem.Domain.Entities;

public class Grade : BaseAuditableEntity
{
    public string Name { get; set; } = string.Empty;

    // Foreign key
    public Guid SchoolId { get; set; }

    // navigational property
    public School School { get; set; } = null!;

    public ICollection<Subject> Subjects { get; set; } = new List<Subject>();
    public ICollection<ApplicationUser> Users { get; set; } = new List<ApplicationUser>();
}