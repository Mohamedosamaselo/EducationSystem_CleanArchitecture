using EducationSystem.Application.Dtos.Request;
using EducationSystem.Application.Dtos.Response;

namespace EducationSystem.Application.Abstarctions.Services;

public interface IOrganisationService
{
    Task<SchoolResponse?> GetByIdAsync(Guid Id);

    //Task<SchoolResponse?> GetByNameAsync(string schoolName);// Search organisation

    Task<SchoolResponse> AddAsync(CreateSchoolRequest createDto);

    Task<SchoolResponse> UpdateAsync(Guid Id, UpdateSchoolRequest updateDto);

    Task DeleteAsync(Guid Id);
}