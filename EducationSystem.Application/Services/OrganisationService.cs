using EducationSystem.Application.Abstarctions.Services;
using EducationSystem.Application.Abstarctions.UnitOfWork;
using EducationSystem.Application.Dtos.Response;

namespace EducationSystem.Application.Services;

public class OrganisationService(IUnitOfWork unitOfWork) : IOrganisationService
{
    private readonly IUnitOfWork unitOfWork = unitOfWork;

    public async Task<OrganisationResponse?> GetByIdAsync(Guid Id)
    {
        var organisation = await unitOfWork.OrganisationRepository.GetByIdWithIncludeAsync(Id, organisation => organisation.Schools);

        if (organisation is null)
            return null;

        return new OrganisationResponse
        {
            Name = organisation.Name,
            Email = organisation.Email,
            Phone = organisation.Phone,
            Schools = organisation.Schools.Select(school => new SchoolResponse
            {
                Name = school.Name,
                Address = school.Address,
                Email = school.Email,
                PhoneNumber = school.PhoneNumber,
                Status = school.Status,
                LogoUrl = school.LogoUrl,
                OrganisationId = school.OrganisationId
            }).ToList()
        };
    }
}