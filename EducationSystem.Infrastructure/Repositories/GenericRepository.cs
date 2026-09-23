using EducationSystem.Application.Abstarctions.Persistence.Repositories;
using EducationSystem.Domain.Entities.Common;
using EducationSystem.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace EducationSystem.Infrastructure.Repositories;

public class GenericRepository<TEntity>
    : IGenericRepository<TEntity> where TEntity : BaseAuditableEntity
{
    private readonly ApplicationDbContext _context;
    private readonly DbSet<TEntity> _dbSet;

    public GenericRepository(ApplicationDbContext context)
    {
        _context = context;
        _dbSet = _context.Set<TEntity>();
    }

    //public async Task<IReadOnlyList<TEntity>> GetAllAsync() => await _dbSet.AsNoTracking()
    //                                                                       .ToListAsync();

    //public async Task<TEntity?> GetByIdAsync(Guid Id) => await _dbSet.FindAsync(Id);

    //public async Task AddAsync(TEntity entity) => await _dbSet.AddAsync(entity);

    //public async Task AddRangeAsync(IEnumerable<TEntity> entities) => await _dbSet.AddRangeAsync(entities);

    //public void Delete(TEntity entity) => _dbSet.Remove(entity);

    //public void DeleteRange(IEnumerable<TEntity> entities) => _dbSet.RemoveRange(entities);

    //public void Update(TEntity entity) => _dbSet.Update(entity);

    //public void UpdateRange(IEnumerable<TEntity> entities) => _dbSet.UpdateRange(entities);

    //public async Task<int> CountAsync() => await _dbSet.CountAsync();

    //public async Task<IReadOnlyList<TEntity>> GetAllAsync(Expression<Func<TEntity, bool>>? filter = null)
    //{
    //    IQueryable<TEntity> query = _dbSet;

    //    if (filter != null)
    //    {
    //        query = query.Where(filter);
    //    }

    //    return await query
    //        .AsNoTracking()
    //        .ToListAsync();
    //}

    //// get entity by id with related entities using include [ Eager Laoding ]
    //public async Task<TEntity?> GetByIdWithIncludeAsync(Guid id, params Expression<Func<TEntity, object>>[] includes)
    //{
    //    IQueryable<TEntity> query = _dbSet;

    //    foreach (var include in includes)
    //    {
    //        query = query.Include(include);
    //    }

    //    return await query.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
    //}

    //// get all entities with related entities using include [ Eager Laoding ]
    //public async Task<IReadOnlyList<TEntity>> GetAllWithIncludesAsync(
    //                                            Expression<Func<TEntity, bool>>? filter = null,
    //                                            params Expression<Func<TEntity, object>>[] includes)
    //{
    //    IQueryable<TEntity> query = _dbSet;

    //    if (filter != null)
    //    {
    //        query = query.Where(filter);
    //    }

    //    if (includes != null)
    //    {
    //        foreach (var include in includes)
    //        {
    //            query = query.Include(include);
    //        }
    //    }

    //    return await query
    //        .AsNoTracking()
    //        .ToListAsync();
    //}

    //public async Task<TEntity?> GetByNameAsync(string name)
    //{
    //    IQueryable<TEntity> query = _dbSet;

    //    return await query.AsNoTracking()
    //                .FirstOrDefaultAsync(x => EF.Property<string>(x, "Name").ToLower() == name.ToLower());
    //}

    //public async Task<TEntity?> FirstOrDefaultAsync(Expression<Func<TEntity, bool>> predicate)
    //    => await _dbSet.FirstOrDefaultAsync(predicate);

    // ─────────────────────────────────────────────────────────────
    // READ — single by Id
    // ─────────────────────────────────────────────────────────────
    public async Task<TEntity?> GetByIdAsync(Guid Id, CancellationToken ct = default)
        // FindAsync has a (object[], CancellationToken) overload — pass ct explicitly
        => await _dbSet.FindAsync(new object[] { Id }, ct);

    public async Task<TEntity?> GetByIdWithIncludeAsync(
        Guid id,
        params Expression<Func<TEntity, object>>[] includes)
    {
        IQueryable<TEntity> query = _dbSet;

        foreach (var include in includes)
            query = query.Include(include);

        return await query
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task<IReadOnlyList<TEntity>?> GetAllWithIncludesAsync(
        Expression<Func<TEntity, bool>>? filter = null,
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

    // ─────────────────────────────────────────────────────────────
    // READ — lists
    // ─────────────────────────────────────────────────────────────
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
}