using EducationSystem.Domain.Entities.Common;

namespace EducationSystem.Domain.Entities;

public class Organisation : BaseAuditableEntity
{
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;

    public Guid? ApplicationUserId { get; set; }  // Foreign key to ApplicationUser

    // Navigational property
    public virtual ApplicationUser? OrganisationAdmin { get; set; }  // Navigation property to ApplicationUser

    public virtual ICollection<School> Schools { get; set; } = new List<School>();
}