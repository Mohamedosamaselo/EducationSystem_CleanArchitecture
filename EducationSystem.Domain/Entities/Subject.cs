namespace EducationSystem.Domain.Entities;

public class Subject : BaseAuditableEntity
{
    public string Name { get; set; } = string.Empty;
    public int Code { get; set; }

    // Foreign key
    public Guid SchoolId { get; set; }

    // navigational property
    public School School { get; set; } = null!;

    public ICollection<Grade> Grades { get; set; } = new HashSet<Grade>();
}