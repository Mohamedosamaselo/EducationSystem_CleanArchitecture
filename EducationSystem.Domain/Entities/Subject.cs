using EducationSystem.Domain.Interfaces;

namespace EducationSystem.Domain.Entities;

public class Subject : BaseAuditableEntity
{
    public string Name { get; set; }

    public DateTime CreatedAt { get; set; }
    public Guid? CreatedBy { get; set; }
    public DateTime? ModifiedAt { get; set; }
    public Guid? LastModifiedBy { get; set; }

    // Foreign key
    public Guid SchoolId { get; set; }

    // navigational property
    public School School { get; set; } = null!;

    public ICollection<Grade> Grades { get; set; } = new List<Grade>();
}