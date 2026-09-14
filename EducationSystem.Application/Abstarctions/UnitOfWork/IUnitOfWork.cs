using EducationSystem.Application.Abstarctions.Persistence.Repositories;
using EducationSystem.Domain.Entities;

namespace EducationSystem.Application.Abstarctions.UnitOfWork;

public interface IUnitOfWork : IDisposable
{
    IGenericRepository<Organisation> OrganisationRepository { get; }
    IGenericRepository<School> SchoolRepository { get; }
    IGenericRepository<Grade> GradeRepository { get; }
    IGenericRepository<Subject> SubjectRepository { get; }
    IGenericRepository<Permission> PermissionRepository { get; }

    // I Commented it as we have UserManager  , RoleManager in Identity Package
    // IGenericRepository<ApplicationUser> UserRepo { get; }
    // IGenericRepository<ApplicationRole> RoleRepo { get; }

    Task<int> SaveChangesAsync(); // save All changes in DB
}