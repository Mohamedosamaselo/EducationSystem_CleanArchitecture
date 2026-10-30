using EducationSystem.Domain.Entities.Common;
using EducationSystem.Domain.Interfaces.Common;

namespace EducationSystem.Domain.Entities;

//public class Organisation : BaseAuditableEntity
public class Organisation : IBaseAuditableEntity
{
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;

    public Guid? ApplicationUserId { get; set; }  // Foreign key to ApplicationUser

    // Navigational property
    public virtual ApplicationUser? OrganisationAdmin { get; set; }  // Navigation property to ApplicationUser

    public virtual ICollection<School> Schools { get; set; } = new List<School>();
    public Guid Id { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    public DateTime CreatedAt { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    public Guid? CreatedBy { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    public DateTime? ModifiedAt { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    public Guid? LastModifiedBy { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
}