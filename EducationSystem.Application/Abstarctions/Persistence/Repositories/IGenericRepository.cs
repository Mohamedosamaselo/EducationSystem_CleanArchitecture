using EducationSystem.Domain.Common;

namespace EducationSystem.Application.Abstarctions.Persistence.Repositories;

public interface IGenericRepository<TEntity> where TEntity : BaseAuditableEntity
{
    Task<TEntity?> GetByIdAsync(Guid Id);

    Task<IReadOnlyList<TEntity>> GetAllAsync();

    Task AddAsync(TEntity entity);

    void UpdateAsync(TEntity entity);

    void DeleteAsync(TEntity entity);
}