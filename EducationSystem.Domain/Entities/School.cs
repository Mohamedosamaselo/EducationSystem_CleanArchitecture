namespace EducationSystem.Domain.Entities;

public class School : BaseAuditableEntity
{
    public string Name { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;

    // Foreign key
    public Guid OrganisationId { get; set; }

    // navigational property
    public Organisation Organisation { get; set; } = null!;

    public ICollection<Subject> Subjects { get; set; } = new HashSet<Subject>();
    public ICollection<Grade> Grades { get; set; } = new HashSet<Grade>();
    public ICollection<User> Users { get; set; } = new HashSet<User>();
}