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
    public IGenericRepository<ApplicationUser> UserRepository { get; }

    //public IGenericRepository<ApplicationRole> RoleRepository { get; }

    public IGenericRepository<TeacherGrade> TeacherGradeRepository { get; }

    public IGenericRepository<UserSubject> UserSubjectRepository { get; }

    public UnitOfWork(ApplicationDbContext dbContext)
    {
        _context = dbContext;

        OrganisationRepository = new GenericRepository<Organisation>(_context);
        SchoolRepository = new GenericRepository<School>(_context);
        GradeRepository = new GenericRepository<Grade>(_context);
        SubjectRepository = new GenericRepository<Subject>(_context);
        UserRepository = new GenericRepository<ApplicationUser>(_context);
        
        TeacherGradeRepository = new GenericRepository<TeacherGrade>(_context);
        UserSubjectRepository = new GenericRepository<UserSubject>(_context);
    }

    public async Task<int> SaveChangesAsync(CancellationToken ct = default) => await _context.SaveChangesAsync();

    public void Dispose() => _context.Dispose();
}