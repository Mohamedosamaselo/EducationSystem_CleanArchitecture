namespace EducationSystem.Domain.Entities;

public class Grade : BaseAuditableEntity
{
    public string Name { get; set; } = string.Empty;

    // Foreign key
    public Guid SchoolId { get; set; }

    // navigational property
    public School School { get; set; } = null!;

    public ICollection<Subject> Subjects { get; set; } = new HashSet<Subject>();
    public ICollection<User> Users { get; set; } = new HashSet<User>();
}