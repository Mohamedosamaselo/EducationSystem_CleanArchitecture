using EducationSystem.Domain.Common;

namespace EducationSystem.Application.Abstarctions.Persistence.Repositories;

public interface IGenericRepository<TEntity>
                                            where TEntity : BaseAuditableEntity
{
    Task<TEntity?> GetByIdAsync(Guid Id);

    Task<IReadOnlyList<TEntity>> GetAllAsync();

    Task AddAsync(TEntity entity);

    Task UpdateAsync(TEntity entity);

    Task DeleteAsync(TEntity entity);
}