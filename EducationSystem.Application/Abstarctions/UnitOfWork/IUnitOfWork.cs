using EducationSystem.Application.Abstarctions.Persistence.Repositories;
using EducationSystem.Domain.Entities;

namespace EducationSystem.Application.Abstarctions.UnitOfWork;

public interface IUnitOfWork : IDisposable
{
    IGenericRepository<Organisation> OrganisationRepository { get; }
    IGenericRepository<School> SchoolRepository { get; }
    IGenericRepository<Grade> GradeRepository { get; }
    IGenericRepository<Subject> SubjectRepository { get; }

    Task<int> SaveChangesAsync(CancellationToken ct = default); // save All changes in DB
}