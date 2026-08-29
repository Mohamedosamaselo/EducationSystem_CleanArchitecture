namespace EducationSystem.Domain.Entities;

public class Organisation : BaseAuditableEntity
{
    public string Name { get; set; } = string.Empty;

    // navigational property
    public ICollection<School> Schools { get; set; }

    public DateTime CreatedAt { get; set; }
    public Guid? CreatedBy { get; set; }
    public DateTime? ModifiedAt { get; set; }
    public Guid? LastModifiedBy { get; set; }
}