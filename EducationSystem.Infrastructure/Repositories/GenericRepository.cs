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
    private readonly DbSet<TEntity> _dbset;

    public GenericRepository(ApplicationDbContext context)
    {
        _context = context;
        _dbset = _context.Set<TEntity>();
    }

    public async Task<IReadOnlyList<TEntity>> GetAllAsync() => await _dbset.AsNoTracking()
                                                                           .ToListAsync();

    public async Task<TEntity?> GetByIdAsync(Guid Id) => await _dbset.FindAsync(Id);

    public async Task AddAsync(TEntity entity) => await _dbset.AddAsync(entity);

    public async Task AddRangeAsync(IEnumerable<TEntity> entities) => await _dbset.AddRangeAsync(entities);

    public void Delete(TEntity entity) => _dbset.Remove(entity);

    public void DeleteRange(IEnumerable<TEntity> entities) => _dbset.RemoveRange(entities);

    public void Update(TEntity entity) => _dbset.Update(entity);

    public void UpdateRange(IEnumerable<TEntity> entities) => _dbset.UpdateRange(entities);

    public async Task<int> CountAsync() => await _dbset.CountAsync();

    public async Task<IReadOnlyList<TEntity>> GetAllAsync(Expression<Func<TEntity, bool>>? filter = null)
    {
        IQueryable<TEntity> query = _dbset;

        if (filter != null)
        {
            query = query.Where(filter);
        }

        return await query
            .AsNoTracking()
            .ToListAsync();
    }

    // get entity by id with related entities using include [ Eager Laoding ]
    public async Task<TEntity?> GetByIdWithIncludeAsync(Guid id, params Expression<Func<TEntity, object>>[] includes)
    {
        IQueryable<TEntity> query = _dbset;

        foreach (var include in includes)
        {
            query = query.Include(include);
        }

        return await query.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
    }

    // get all entities with related entities using include [ Eager Laoding ]
    public async Task<IReadOnlyList<TEntity>> GetAllWithIncludesAsync(
                                                Expression<Func<TEntity, bool>>? filter = null,
                                                params Expression<Func<TEntity, object>>[] includes)
    {
        IQueryable<TEntity> query = _dbset;

        if (filter != null)
        {
            query = query.Where(filter);
        }

        if (includes != null)
        {
            foreach (var include in includes)
            {
                query = query.Include(include);
            }
        }

        return await query
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<TEntity?> GetByNameAsync(string name)
    {
        IQueryable<TEntity> query = _dbset;

        return await query.AsNoTracking()
                    .FirstOrDefaultAsync(x => EF.Property<string>(x, "Name").ToLower() == name.ToLower());
    }
}