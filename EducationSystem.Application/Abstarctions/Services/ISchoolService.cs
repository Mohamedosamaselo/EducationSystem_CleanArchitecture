using EducationSystem.Application.Dtos.Request.School;
using EducationSystem.Application.Dtos.Response;

namespace EducationSystem.Application.Abstarctions.Services;

public interface ISchoolService
{
    Task<SchoolResponse?> GetByIdAsync(Guid Id);

    Task<SchoolResponse?> GetByNameAsync(string schoolName);// Search Schools

    Task<IReadOnlyList<SchoolResponse>> GetAllByOrganisationIdAsync(Guid organizationId);

    Task<IReadOnlyList<SchoolResponse>> GetAllAsync();

    Task<SchoolResponse> AddAsync(CreateSchoolRequest createDto);

    Task<SchoolResponse> UpdateAsync(Guid Id, UpdateSchoolRequest updateDto);

    Task<bool> DeleteAsync(Guid Id);
}