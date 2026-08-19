using EducationSystem.Application.Abstarctions.Persistence.Repositories;
using EducationSystem.Application.Abstarctions.UnitOfWork;
using EducationSystem.Domain.Entities;
using EducationSystem.Infrastructure.Persistence;
using EducationSystem.Infrastructure.Repositories;

namespace EducationSystem.Infrastructure.unitOfWork;

public class UnitOfWork : IUnitOfWork
{
    private readonly ApplicationDbContext _context;

    public IGenericRepository<Organisation> OrganisationRepo { get; }
    public IGenericRepository<School> SchoolRepo { get; }
    public IGenericRepository<Grade> GradeRepo { get; }
    public IGenericRepository<Subject> SubjectRepo { get; }
    public IGenericRepository<ApplicationUser> UserRepo { get; }
    public IGenericRepository<Role> RoleRepo { get; }
    public IGenericRepository<Permission> PermissionRepo { get; }

    public UnitOfWork(ApplicationDbContext dbContext)
    {
        _context = dbContext;

        OrganisationRepo = new GenericRepository<Organisation>(_context);
        SchoolRepo = new GenericRepository<School>(_context);
        GradeRepo = new GenericRepository<Grade>(_context);
        SubjectRepo = new GenericRepository<Subject>(_context);
        UserRepo = new GenericRepository<ApplicationUser>(_context);
        RoleRepo = new GenericRepository<Role>(_context);
        PermissionRepo = new GenericRepository<Permission>(_context);
    }

    public async Task<int> CompleteAsync(CancellationToken cancellationToken = default)
        => await _context.SaveChangesAsync(cancellationToken);

    public void Dispose() => _context.Dispose();
}