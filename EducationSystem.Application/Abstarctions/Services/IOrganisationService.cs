using EducationSystem.Application.Abstarctions.HandlingError;
using EducationSystem.Application.Dtos;
using EducationSystem.Application.Dtos.Response.Organisation;

namespace EducationSystem.Application.Abstarctions.Services;

public interface IOrganisationService
{
    Task<Result<OrganisationResponse?>> GetByIdAsync(Guid Id, CancellationToken cancellationToken = default);

    Task<Result<OrganisationResponse?>> AddAsync(CreateOrganisationRequest createDto, CancellationToken cancellationToken = default);
}