namespace EducationSystem.Domain.Entities;

public class Organisation : BaseAuditableEntity
{
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;

    // Navigational property
    public ICollection<School> Schools { get; set; } = new List<School>();
}