using EducationSystem.Application.Abstarctions.Persistence.Repositories;
using EducationSystem.Domain.Common;
using EducationSystem.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace EducationSystem.Infrastructure.Repositories;

public class GenericRepository<TEntity> : IGenericRepository<TEntity> where TEntity : BaseAuditableEntity
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

    public void Delete(TEntity entity) => _dbset.Remove(entity);

    public void Update(TEntity entity) => _dbset.Update(entity);
}