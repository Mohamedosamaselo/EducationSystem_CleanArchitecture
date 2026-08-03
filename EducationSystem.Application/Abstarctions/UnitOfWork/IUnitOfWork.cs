using EducationSystem.Application.Abstarctions.Persistence.Repositories;
using EducationSystem.Domain.Common;

namespace EducationSystem.Application.Abstarctions.UnitOfWork;

public interface IUnitOfWork : IDisposable
{
    IGenericRepository<TEntity> GetRepository<TEntity>() where TEntity : BaseAuditableEntity;

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}