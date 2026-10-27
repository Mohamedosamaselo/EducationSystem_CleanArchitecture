using EducationSystem.Application.Abstarctions.HandlingError;
using EducationSystem.Application.Dtos.School;

namespace EducationSystem.Application.Abstarctions.Services;

public interface ISchoolService
{
    Task<Result<IReadOnlyList<SchoolResponse>>> GetAllAsync(CancellationToken ct = default);

    Task<Result<SchoolResponse?>> GetByIdAsync(Guid id, CancellationToken ct = default);

    //Task<Result<SchoolResponse?>> GetByNameAsync(string schoolName, CancellationToken ct = default);

    //Task<Result<IReadOnlyList<SchoolResponse>>> GetAllByOrganisationIdAsync(Guid organisationId, CancellationToken ct = default);

    Task<Result<SchoolResponse?>> AddAsync(CreateSchoolRequest createDto, CancellationToken ct = default);

    Task<Result<SchoolResponse?>> UpdateAsync(Guid id, UpdateSchoolRequest updateDto, CancellationToken ct = default);

    Task<Result> DeleteAsync(Guid id, CancellationToken ct = default);

    // SchoolAdmin management // active for OrganisationAdmin Only
    Task<Result<SchoolAdminResponse?>> GetSchoolAdminByIdAsync(Guid id, CancellationToken ct = default);

    Task<Result<SchoolAdminResponse?>> CreateSchoolAdminAsync(Guid schoolId, CreateSchoolAdminRequest createDto, CancellationToken ct = default);

    Task<Result<SchoolAdminResponse?>> UpdateSchoolAdminAsync(Guid schoolId, UpdateSchoolAdminRequest updateDto, CancellationToken ct = default);

    Task<Result> RemoveSchoolAdminAsync(Guid id, CancellationToken ct = default);
}