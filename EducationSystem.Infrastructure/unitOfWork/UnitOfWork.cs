using EducationSystem.Application.Abstarctions.Persistence.Repositories;
using EducationSystem.Application.Abstarctions.UnitOfWork;
using EducationSystem.Domain.Entities;
using EducationSystem.Infrastructure.Persistence;
using EducationSystem.Infrastructure.Repositories;

namespace EducationSystem.Infrastructure.unitOfWork;

public class UnitOfWork : IUnitOfWork
{
    private readonly ApplicationDbContext _context;

    public IGenericRepository<Organisation> OrganisationRepository { get; }
    public IGenericRepository<School> SchoolRepository { get; }
    public IGenericRepository<Grade> GradeRepository { get; }
    public IGenericRepository<Subject> SubjectRepository { get; }
    public IGenericRepository<Permission> PermissionRepository { get; }

    public UnitOfWork(ApplicationDbContext dbContext)
    {
        _context = dbContext;

        OrganisationRepository = new GenericRepository<Organisation>(_context);
        SchoolRepository = new GenericRepository<School>(_context);
        GradeRepository = new GenericRepository<Grade>(_context);
        SubjectRepository = new GenericRepository<Subject>(_context);
        PermissionRepository = new GenericRepository<Permission>(_context);

        //UserRepo = new GenericRepository<ApplicationUser>(_context);
        //RoleRepo = new GenericRepository<ApplicationRole>(_context);
    }

    public async Task<int> CompleteAsync() => await _context.SaveChangesAsync();

    public void Dispose() => _context.Dispose();
}