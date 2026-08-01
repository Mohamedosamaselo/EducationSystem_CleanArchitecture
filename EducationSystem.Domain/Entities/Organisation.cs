namespace EducationSystem.Domain.Entities;

public class Organisation : BaseAuditableEntity
{
    public string Name { get; set; } = string.Empty;

    // navigational property
    public ICollection<School> Schools { get; set; } = new HashSet<School>();
}