using EducationSystem.Domain.Interfaces;

namespace EducationSystem.Domain.Common;

public abstract class BaseEntity : IBaseEntity
{
    public Guid Id { get; set; } = Guid.NewGuid();
}