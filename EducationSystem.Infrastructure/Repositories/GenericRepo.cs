using EducationSystem.Application.Abstarctions.Persistence.Repositories;
using EducationSystem.Domain.Entities;
using EducationSystem.Domain.Entities.Common;
using EducationSystem.Domain.Interfaces.Common;
using EducationSystem.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace EducationSystem.Infrastructure.Repositories;

public class GenericRepo<TEntity> : IGenericRepository<TEntity> where TEntity : class, IBaseAuditableEntity
{
    private readonly ApplicationDbContext _context;
    private readonly DbSet<TEntity> _dbSet;

    public GenericRepo(ApplicationDbContext context)
    {
        _context = context;
        _dbSet = _context.Set<TEntity>();
    }

    public async Task<TEntity?> GetByIdAsync(Guid Id, CancellationToken ct = default)
        // FindAsync has a (object[], CancellationToken) overload — pass ct explicitly
        => await _dbSet.FindAsync(new object[] { Id }, ct);

    public async Task<TEntity?> GetByIdWithIncludeAsync(
        Guid id,
         CancellationToken cancellationToken = default,
        params Expression<Func<TEntity, object>>[] includes)
    {
        IQueryable<TEntity> query = _dbSet;

        foreach (var include in includes)
            query = query.Include(include);

        return await query
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task<TEntity?> GetByIdWithIncludeTrackedAsync(Guid id, CancellationToken ct, params Expression<Func<TEntity, object>>[] includes)
    {
        IQueryable<TEntity> query = _dbSet;          // no AsNoTracking here

        foreach (var include in includes)
            query = query.Include(include);

        return await query.FirstOrDefaultAsync(e => e.Id == id, ct);
    }

    public async Task<IReadOnlyList<TEntity>?> GetAllWithIncludesAsync(
        Expression<Func<TEntity, bool>>? filter = null,
         CancellationToken cancellationToken = default,
        params Expression<Func<TEntity, object>>[] includes)
    {
        IQueryable<TEntity> query = _dbSet;

        if (filter is not null)
            query = query.Where(filter);

        if (includes is not null && includes.Length > 0)
        {
            foreach (var include in includes)
                query = query.Include(include);
        }

        return await query
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<TEntity?> GetByNameAsync(string Name, CancellationToken ct = default)
    {
        var normalisedName = Name.ToLower();

        return await _dbSet
            .AsNoTracking()
            .FirstOrDefaultAsync(x => EF.Property<string>(x, "Name").ToLower() == normalisedName, ct);
    }

    public async Task<IReadOnlyList<TEntity>> GetAllAsync(CancellationToken ct = default)
        => await _dbSet
            .AsNoTracking()
            .ToListAsync(ct);

    public async Task<IReadOnlyList<TEntity>> GetAllAsync(
        Expression<Func<TEntity, bool>>? filter = null,
        CancellationToken ct = default)
    {
        IQueryable<TEntity> query = _dbSet;

        if (filter is not null)
            query = query.Where(filter);

        return await query
            .AsNoTracking()
            .ToListAsync(ct);
    }

    public async Task<TEntity?> FirstOrDefaultAsync(
        Expression<Func<TEntity, bool>> predicate,
        CancellationToken ct = default)
        => await _dbSet.FirstOrDefaultAsync(predicate, ct);

    // ─────────────────────────────────────────────────────────────
    // WRITE — add
    // ─────────────────────────────────────────────────────────────
    public async Task AddAsync(TEntity entity, CancellationToken ct = default)
        => await _dbSet.AddAsync(entity, ct);

    public async Task AddRangeAsync(IEnumerable<TEntity> entities)
        // interface doesn't expose a CancellationToken overload — call the non-token version
        => await _dbSet.AddRangeAsync(entities);

    // ─────────────────────────────────────────────────────────────
    // WRITE — update / delete (sync in EF Core — change tracker only)
    // ─────────────────────────────────────────────────────────────
    public void Update(TEntity entity) => _dbSet.Update(entity);

    public void UpdateRange(IEnumerable<TEntity> entities) => _dbSet.UpdateRange(entities);

    public void Delete(TEntity entity) => _dbSet.Remove(entity);

    public void DeleteRange(IEnumerable<TEntity> entities) => _dbSet.RemoveRange(entities);

    // ─────────────────────────────────────────────────────────────
    // MISC
    // ─────────────────────────────────────────────────────────────
    public async Task<int> CountAsync() => await _dbSet.CountAsync();

    //public async Task<bool> SchoolHasSubjectAsync(
    //                                                Guid schoolId,
    //                                                Guid subjectId,
    //                                                CancellationToken ct = default)
    //{
    //    return await _context.Grades
    //        .AnyAsync(g =>
    //            g.SchoolId == schoolId &&
    //            g.Subjects.Any(s => s.Id == subjectId),
    //            ct);
    //}

    //public async Task<bool> GradeHasSubjectAsync(
    //                                            Guid gradeId,
    //                                            Guid schoolId,
    //                                            Guid subjectId,
    //                                            CancellationToken ct = default)
    //{
    //    return await _context.Grades
    //        .AnyAsync(g =>
    //            g.Id == gradeId &&
    //            g.SchoolId == schoolId &&
    //            g.Subjects.Any(s => s.Id == subjectId),
    //            ct);
    //}

    //public async Task<IReadOnlyList<Grade>> GetTeacherGradesAsync(
    //     Guid teacherId,
    //     Guid schoolId,
    //     CancellationToken ct = default)
    //{
    //    return await _context.TeacherGrades
    //        .Where(tg =>
    //            tg.TeacherId == teacherId &&
    //            tg.Grade.SchoolId == schoolId)
    //        .Select(tg => tg.Grade)
    //        .ToListAsync(ct);
    //}

    public Task<bool> AnyAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken ct = default)
        => _dbSet.AnyAsync(predicate, ct);

    public Task UpdateAsync(TEntity entity, CancellationToken ct = default)
     => Task.Run(() => _dbSet.Update(entity), ct);

    public Task DeleteAsync(TEntity entity, CancellationToken ct = default)
   => Task.Run(() => _dbSet.Remove(entity), ct);
}