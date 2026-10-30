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

    public IGenericRepository<ApplicationRole> RoleRepository { get; }

    public IGenericRepository<TeacherGrade> TeacherGradeRepository { get; }

    public IGenericRepository<UserSubject> UserSubjectRepository { get; }

    public IGenericRepository<EmailConfirmationToken> EmailConfirmationTokenRepository { get; }

    public UnitOfWork(ApplicationDbContext dbContext)
    {
        _context = dbContext;

        OrganisationRepository = new GenericRepo<Organisation>(_context);
        SchoolRepository = new GenericRepo<School>(_context);
        GradeRepository = new GenericRepo<Grade>(_context);
        SubjectRepository = new GenericRepo<Subject>(_context);
        UserRepository = new GenericRepo<ApplicationUser>(_context);
        RoleRepository = new GenericRepo<ApplicationRole>(_context);
        TeacherGradeRepository = new GenericRepo<TeacherGrade>(_context);
        UserSubjectRepository = new GenericRepo<UserSubject>(_context);
        EmailConfirmationTokenRepository = new GenericRepo<EmailConfirmationToken>(_context);
    }

    public async Task<int> SaveChangesAsync(CancellationToken ct = default) => await _context.SaveChangesAsync();

    public void Dispose() => _context.Dispose();
}