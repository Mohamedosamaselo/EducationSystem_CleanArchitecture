using EducationSystem.Domain.Entities;
using EducationSystem.Domain.Entities.Common;
using EducationSystem.Domain.Interfaces.Common;
using System.Linq.Expressions;

namespace EducationSystem.Application.Abstarctions.Persistence.Repositories;

public interface IGenericRepository<TEntity> where TEntity : BaseAuditableEntity
{
    Task<TEntity?> GetByIdAsync(Guid Id, CancellationToken ct = default);

    Task<TEntity?> GetByIdWithIncludeAsync(Guid id, CancellationToken cancellationToken = default, params Expression<Func<TEntity, object>>[] includes);

    Task<TEntity?> GetByIdWithIncludeTrackedAsync(Guid id, CancellationToken ct, params Expression<Func<TEntity, object>>[] includes);

    Task<IReadOnlyList<TEntity>?> GetAllWithIncludesAsync(Expression<Func<TEntity, bool>>? filter = null,
                                                          CancellationToken cancellationToken = default,
                                                          params Expression<Func<TEntity, object>>[] includes);

    Task<TEntity?> GetByNameAsync(string Name, CancellationToken ct = default); // Search

    Task<IReadOnlyList<TEntity>> GetAllAsync(CancellationToken ct = default);

    Task<IReadOnlyList<TEntity>> GetAllAsync(Expression<Func<TEntity, bool>>? filter = null, CancellationToken ct = default);

    Task<TEntity?> FirstOrDefaultAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken ct = default); //

    Task AddAsync(TEntity entity, CancellationToken ct = default);

    Task AddRangeAsync(IEnumerable<TEntity> entities);

    void Update(TEntity entity);

    void UpdateRange(IEnumerable<TEntity> entities);

    void Delete(TEntity entity);

    void DeleteRange(IEnumerable<TEntity> entities);

    Task<bool> SchoolHasSubjectAsync(
    Guid schoolId,
    Guid subjectId,
    CancellationToken ct = default);

    Task<bool> GradeHasSubjectAsync(
        Guid gradeId,
        Guid schoolId,
        Guid subjectId,
        CancellationToken ct = default);

    Task<IReadOnlyList<Grade>> GetTeacherGradesAsync(
    Guid teacherId,
    Guid schoolId,
    CancellationToken ct = default);

    Task<int> CountAsync();
}