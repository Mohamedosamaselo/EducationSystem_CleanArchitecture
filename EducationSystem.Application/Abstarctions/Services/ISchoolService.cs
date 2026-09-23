using EducationSystem.Application.Abstarctions.HandlingError;
using EducationSystem.Application.Dtos.Request.School;
using EducationSystem.Application.Dtos.Response;

namespace EducationSystem.Application.Abstarctions.Services;

public interface ISchoolService
{
    Task<Result<IReadOnlyList<SchoolResponse>>> GetAllAsync(CancellationToken ct = default);

    Task<Result<SchoolResponse?>> GetByIdAsync(Guid id, CancellationToken ct = default);

    Task<Result<SchoolResponse?>> GetByNameAsync(string schoolName, CancellationToken ct = default);

    Task<Result<IReadOnlyList<SchoolResponse>>> GetAllByOrganisationIdAsync(Guid organisationId, CancellationToken ct = default);

    Task<Result<SchoolResponse?>> AddAsync(CreateSchoolRequest createDto, CancellationToken ct = default);

    Task<Result<SchoolResponse?>> UpdateAsync(Guid id, UpdateSchoolRequest updateDto, CancellationToken ct = default);

    Task<Result> DeleteAsync(Guid id, CancellationToken ct = default);
}