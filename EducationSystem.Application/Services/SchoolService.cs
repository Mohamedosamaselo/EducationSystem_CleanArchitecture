using EducationSystem.Application.Abstarctions.Services;
using EducationSystem.Application.Abstarctions.UnitOfWork;
using EducationSystem.Application.Dtos.Request;
using EducationSystem.Application.Dtos.Response;
using Microsoft.AspNetCore.Http.HttpResults;

namespace EducationSystem.Application.Services;

public class SchoolService(IUnitOfWork unitOfWork) : ISchoolService
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public async Task<SchoolResponse?> GetByIdAsync(Guid Id)
    {
        var school = await _unitOfWork.SchoolRepo.GetByIdAsync(Id);

        return new SchoolResponse
        {
            Name = school!.Name,
            Address = school.Address,
            OrganisationId = school.OrganisationId
        };
    }

    public async Task<SchoolResponse?> GetByNameAsync(string schoolName)
    {
        var school = await _unitOfWork.SchoolRepo.GetByNameAsync(schoolName);

        if (school is not null)
            return new SchoolResponse
            {
                Name = school!.Name,
                Address = school.Address,
                OrganisationId = school.OrganisationId
            };

        return null;
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

    public async Task<IReadOnlyList<SchoolResponse>> GetAllByOrganisationIdAsync(Guid organizationId)
    {
        var schools = await _unitOfWork.SchoolRepo.GetAllAsync(s => s.OrganisationId == organizationId);

        return schools.Select(s => new SchoolResponse
        {
            Name = s.Name,
            Address = s.Address,
            OrganisationId = s.OrganisationId
        }).ToList();
    }

    public async Task<SchoolResponse> AddAsync(CreateSchoolRequest createDto)
    {
        var school = new Domain.Entities.School()
        {
            Name = createDto.Name,
            Address = createDto.Address,
            OrganisationId = createDto.OrganisationId,
        };

        await _unitOfWork.SchoolRepo.AddAsync(school);

        await _unitOfWork.CompleteAsync();

        return new Dtos.Response.SchoolResponse
        {
            Name = school.Name,
            Address = school.Address,
            OrganisationId = school.OrganisationId
        };
    }

    public async Task<SchoolResponse> UpdateAsync(Guid Id, UpdateSchoolRequest updateDto)
    {
        var UpdatedSchool = await _unitOfWork.SchoolRepo.GetByIdAsync(Id);

        if (UpdatedSchool is null)
        {
            throw new Exception($"School with ID {Id} was not found.");
        }

        UpdatedSchool.Name = updateDto.Name;
        UpdatedSchool.Address = updateDto.Address;
        UpdatedSchool.OrganisationId = updateDto.OrganisationId;

        _unitOfWork.SchoolRepo.Update(UpdatedSchool);

        await _unitOfWork.CompleteAsync();

        return new Dtos.Response.SchoolResponse
        {
            Name = UpdatedSchool.Name,
            Address = UpdatedSchool.Address,
            OrganisationId = UpdatedSchool.OrganisationId
        };
    }

    public async Task DeleteAsync(Guid Id)
    {
        var school = await _unitOfWork.SchoolRepo.GetByIdAsync(Id);

        if (school is null)
        {
            throw new Exception($"School with ID {Id} was not found.");
        }

        _unitOfWork.SchoolRepo.Delete(school!);

        await _unitOfWork.CompleteAsync();
    }
}