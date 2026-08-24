using EducationSystem.Domain.Interfaces.Common;
using System.Linq.Expressions;

namespace EducationSystem.Application.Abstarctions.Persistence.Repositories;

public interface IGenericRepository<TEntity> where TEntity : class, IBaseAuditableEntity
{
    Task<TEntity?> GetByIdAsync(Guid Id);

    Task<TEntity?> GetByNameAsync(string Name); // Search

    Task<IReadOnlyList<TEntity>> GetAllAsync();

    Task<IReadOnlyList<TEntity>> GetAllAsync(Expression<Func<TEntity, bool>>? filter = null);

    Task AddAsync(TEntity entity);

    Task AddRangeAsync(IEnumerable<TEntity> entities);

    void Update(TEntity entity);

    void UpdateRange(IEnumerable<TEntity> entities);

    void Delete(TEntity entity);

    void DeleteRange(IEnumerable<TEntity> entities);

    Task<int> CountAsync();
}