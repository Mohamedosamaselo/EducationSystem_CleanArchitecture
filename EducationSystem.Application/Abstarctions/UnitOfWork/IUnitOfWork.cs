using EducationSystem.Application.Abstarctions.Persistence.Repositories;
using EducationSystem.Domain.Entities;

namespace EducationSystem.Application.Abstarctions.UnitOfWork;

public interface IUnitOfWork : IDisposable
{
    IGenericRepository<Organisation> OrganisationRepository { get; }
    IGenericRepository<School> SchoolRepository { get; }

    IGenericRepository<Grade> GradeRepository { get; }

    IGenericRepository<Subject> SubjectRepository { get; }

    IGenericRepository<ApplicationUser> UserRepository { get; }
    //IGenericRepository<ApplicationRole> RoleRepository { get; }

    IGenericRepository<TeacherGrade> TeacherGradeRepository { get; }

    IGenericRepository<UserSubject> UserSubjectRepository { get; }

    Task<int> SaveChangesAsync(CancellationToken ct = default); // save All changes in DB
}