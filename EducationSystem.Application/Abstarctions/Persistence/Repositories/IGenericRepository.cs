using EducationSystem.Domain.Entities.Common;
using System.Linq.Expressions;

namespace EducationSystem.Application.Abstarctions.Persistence.Repositories;

public interface IGenericRepository<TEntity> where TEntity : BaseAuditableEntity
{
    Task<TEntity?> GetByIdAsync(Guid Id);

    Task<TEntity?> GetByIdWithIncludeAsync(Guid id, params Expression<Func<TEntity, object>>[] includes);

    Task<IReadOnlyList<TEntity>?> GetAllWithIncludesAsync(Expression<Func<TEntity, bool>>? filter = null,
                                                          params Expression<Func<TEntity, object>>[] includes);

    Task<TEntity?> GetByNameAsync(string Name); // Search

    Task<IReadOnlyList<TEntity>> GetAllAsync();

    Task<IReadOnlyList<TEntity>> GetAllAsync(Expression<Func<TEntity, bool>>? filter = null);

    Task<TEntity?> FirstOrDefaultAsync(Expression<Func<TEntity, bool>> predicate); //

    Task AddAsync(TEntity entity);

    Task AddRangeAsync(IEnumerable<TEntity> entities);

    void Update(TEntity entity);

    void UpdateRange(IEnumerable<TEntity> entities);

    void Delete(TEntity entity);

    void DeleteRange(IEnumerable<TEntity> entities);

    Task<int> CountAsync();
}