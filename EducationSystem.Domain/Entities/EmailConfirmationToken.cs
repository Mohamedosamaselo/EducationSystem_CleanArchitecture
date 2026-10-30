using EducationSystem.Domain.Interfaces.Common;

namespace EducationSystem.Domain.Entities;

public class EmailConfirmationToken : IBaseAuditableEntity
{
    //public Guid Id { get; set; } = Guid.NewGuid();
    public Guid UserId { get; set; }

    public string TokenHash { get; set; } = "";   // SHA-256 of the raw token (raw token only goes in the email)
    public DateTime ExpiresAt { get; set; }
    public DateTime? UsedAt { get; set; }

    public Guid Id { get; set; }
    public DateTime CreatedAt { get; set; }
    public Guid? CreatedBy { get; set; }
    public DateTime? ModifiedAt { get; set; }
    public Guid? LastModifiedBy { get; set; }

    //
    public virtual ApplicationUser User { get; set; } = null!;
}