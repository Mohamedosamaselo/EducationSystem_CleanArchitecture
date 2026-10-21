using EducationSystem.Domain.Entities.Common;

namespace EducationSystem.Domain.Entities;

public class UserSubject
{
    public Guid UserId { get; set; }
    public virtual ApplicationUser User { get; set; } = null!;

    public Guid SubjectId { get; set; }
    public virtual Subject Subject { get; set; } = null!;

    public string? FinalEvaluation { get; set; } = string.Empty;
}