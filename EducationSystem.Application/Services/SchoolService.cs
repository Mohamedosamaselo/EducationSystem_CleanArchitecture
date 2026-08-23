using EducationSystem.Application.Abstarctions.Services;
using EducationSystem.Application.Abstarctions.UnitOfWork;
using EducationSystem.Application.Dtos.Request;
using EducationSystem.Application.Dtos.Response;
using EducationSystem.Domain.Entities;

namespace EducationSystem.Application.Services;

public class SchoolService : ISchoolService
{
    private readonly IUnitOfWork _unitOfWork;

    public SchoolService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<SchoolResponse> GetByIdAsync(Guid Id)
    {
        var school = await _unitOfWork.SchoolRepo.GetByIdAsync(Id);

        return new SchoolResponse
        {
            Name = school!.Name,
            Address = school.Address,
            OrganisationId = school.OrganisationId
        };
    }

    public async Task<IReadOnlyList<SchoolResponse>> GetAllAsync()
    {
        var schools = await _unitOfWork.SchoolRepo.GetAllAsync();

        return schools.Select(s => new SchoolResponse
        {
            Name = s.Name,
            Address = s.Address,
            OrganisationId = s.OrganisationId
        }).ToList();
    }

    public async Task<SchoolResponse> CreateAsync(CreateSchoolRequest createDto)
    {
        var school = new School()
        {
            Name = createDto.Name,
            Address = createDto.Address,
            OrganisationId = createDto.OrganisationId,
        };

        await _unitOfWork.SchoolRepo.AddAsync(school);

        await _unitOfWork.CompleteAsync();

        return new SchoolResponse
        {
            Name = school.Name,
            Address = school.Address,
            OrganisationId = school.OrganisationId
        };
    }

    public async Task<SchoolResponse> UpdateAsync(UpdateSchoolRequest updateDto)
    {
        var school = new School()
        {
            Name = updateDto.Name,
            Address = updateDto.Address,
            OrganisationId = updateDto.OrganisationId,
        };

        _unitOfWork.SchoolRepo.Update(school);

        await _unitOfWork.CompleteAsync();

        return new SchoolResponse
        {
            Name = school.Name,
            Address = school.Address,
            OrganisationId = school.OrganisationId
        };
    }

    public async Task DeleteAsync(Guid Id)
    {
        var school = await _unitOfWork.SchoolRepo.GetByIdAsync(Id);

        _unitOfWork.SchoolRepo.Delete(school!);

        await _unitOfWork.CompleteAsync();
    }
}