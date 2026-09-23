using EducationSystem.Application.Abstarctions.HandlingError;
using EducationSystem.Application.Dtos.Request.Organisation;
using EducationSystem.Application.Dtos.Response.Organisation;

namespace EducationSystem.Application.Abstarctions.Services;

public interface IOrganisationService
{
    Task<Result<OrganisationResponse?>> GetByIdAsync(Guid Id);

    Task<Result<OrganisationResponse?>> AddAsync(CreateOrganisationRequest createDto);
}