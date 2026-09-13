using EducationSystem.Application.Dtos.Request;
using EducationSystem.Application.Dtos.Response;

namespace EducationSystem.Application.Abstarctions.Services;

public interface IOrganisationService
{
    Task<OrganisationResponse?> GetByIdAsync(Guid Id);

    //Task<OrganisationResponse?> GetByNameAsync(string organisationName);// Search organisation

    //Task<OrganisationResponse> AddAsync(CreateSchoolRequest createDto);

    //Task<OrganisationResponse> UpdateAsync(Guid Id, UpdateSchoolRequest updateDto);

    //Task DeleteAsync(Guid Id);
}