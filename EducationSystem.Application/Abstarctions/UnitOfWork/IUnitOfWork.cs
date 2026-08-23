using EducationSystem.Application.Abstarctions.Persistence.Repositories;
using EducationSystem.Domain.Entities;

namespace EducationSystem.Application.Abstarctions.UnitOfWork;

public interface IUnitOfWork : IDisposable
{
    #region Fields

    IGenericRepository<Organisation> OrganisationRepo { get; }
    IGenericRepository<School> SchoolRepo { get; }
    IGenericRepository<Grade> GradeRepo { get; }
    IGenericRepository<Subject> SubjectRepo { get; }
    IGenericRepository<ApplicationUser> UserRepo { get; }
    IGenericRepository<Role> RoleRepo { get; }
    IGenericRepository<Permission> PermissionRepo { get; }

    #endregion Fields

    Task<int> CompleteAsync(); // save All changes in DB
}