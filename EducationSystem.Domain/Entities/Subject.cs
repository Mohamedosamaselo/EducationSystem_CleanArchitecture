using EducationSystem.Domain.Interfaces;

namespace EducationSystem.Domain.Entities;

public class Subject : BaseAuditableEntity
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;

    // Foreign key
    public Guid SchoolId { get; set; }

    // Navigational property
    public School School { get; set; } = null!;

    public ICollection<Grade> Grades { get; set; } = new List<Grade>();
}