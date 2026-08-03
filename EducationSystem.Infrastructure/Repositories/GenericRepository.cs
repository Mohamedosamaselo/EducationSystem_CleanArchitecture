using EducationSystem.Application.Abstarctions.Persistence.Repositories;
using EducationSystem.Domain.Common;
using EducationSystem.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Metadata.Ecma335;

namespace EducationSystem.Infrastructure.Repositories;

public class GenericRepository<TEntity> : IGenericRepository<TEntity> where TEntity : BaseAuditableEntity
{
    private readonly ApplicationDbContext _context;
    private readonly DbSet<TEntity> _dbset;

    public GenericRepository(ApplicationDbContext context, DbSet<TEntity> dbset)
    {
        _context = context;
        _dbset = dbset;
    }

    public async Task<IReadOnlyList<TEntity>> GetAllAsync() => await _dbset.AsNoTracking()
                                                                           .ToListAsync();

    public async Task<TEntity?> GetByIdAsync(Guid Id) => await _dbset.FindAsync(Id);

    public async Task AddAsync(TEntity entity) => await _dbset.AddAsync(entity);

    public void DeleteAsync(TEntity entity) => _dbset.Remove(entity);

    public void UpdateAsync(TEntity entity) => _dbset.Update(entity);
}