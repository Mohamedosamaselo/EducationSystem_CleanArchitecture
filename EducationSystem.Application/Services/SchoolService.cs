using EducationSystem.Application.Abstarctions.Services;
using EducationSystem.Application.Abstarctions.UnitOfWork;
using EducationSystem.Application.Dtos.Request.School;
using EducationSystem.Application.Dtos.Response;
using EducationSystem.Domain.Entities;

namespace EducationSystem.Application.Services;

public class SchoolService(IUnitOfWork unitOfWork) : ISchoolService
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    // getAll
    public async Task<IReadOnlyList<SchoolResponse>> GetAllAsync()
    {
        var schools = await _unitOfWork.SchoolRepository.GetAllAsync();

        return schools.Select(s => new SchoolResponse
        {
            Name = s.Name,
            Address = s.Address,
            Email = s.Email,
            PhoneNumber = s.PhoneNumber,
            LogoUrl = s.LogoUrl,
            Status = s.Status,
            OrganisationId = s.OrganisationId,
            OrganisationName = s.Organisation?.Name ?? string.Empty
        }).ToList();
    }

    //getById
    public async Task<SchoolResponse?> GetByIdAsync(Guid Id)
    {
        var school = await _unitOfWork.SchoolRepository.GetByIdAsync(Id);

        return new SchoolResponse
        {
            Name = school!.Name,
            Address = school.Address,
            Email = school.Email,
            PhoneNumber = school.PhoneNumber,
            Status = school.Status,
            OrganisationId = school.OrganisationId,
            OrganisationName = school.Organisation?.Name ?? string.Empty
        };
    }

    // search school by name
    public async Task<SchoolResponse?> GetByNameAsync(string schoolName)
    {
        var school = await _unitOfWork.SchoolRepository.GetByNameAsync(schoolName);

        if (school is not null)
            return new SchoolResponse
            {
                Name = school.Name,
                Address = school.Address,
                Email = school.Email,
                PhoneNumber = school.PhoneNumber,
                Status = school.Status,
                LogoUrl = school.LogoUrl,
                OrganisationId = school.OrganisationId,
                OrganisationName = school.Organisation?.Name ?? string.Empty,
            };

        return null;
    }

    public async Task<IReadOnlyList<SchoolResponse>> GetAllByOrganisationIdAsync(Guid organizationId)
    {
        var schools = await _unitOfWork.SchoolRepository.GetAllAsync(s => s.OrganisationId == organizationId);

        return schools.Select(school => new SchoolResponse
        {
            Name = school.Name,
            Address = school.Address,
            Email = school.Email,
            PhoneNumber = school.PhoneNumber,
            Status = school.Status,
            LogoUrl = school.LogoUrl,
            OrganisationId = school.OrganisationId,
            OrganisationName = school.Organisation?.Name ?? string.Empty,
        }).ToList();
    }

    public async Task<SchoolResponse> AddAsync(CreateSchoolRequest createSchoolDto)
    {
        // load the organisation by id
        var organisation = await _unitOfWork.OrganisationRepository.GetByIdAsync(createSchoolDto.OrganisationId);
        if (organisation == null)
            throw new Exception($"Organisation with ID {createSchoolDto.OrganisationId} was not found.");

        // map from createDto to School
        var school = new School
        {
            Name = createSchoolDto.Name,
            Address = createSchoolDto.Address,
            Email = createSchoolDto.Email,
            LogoUrl = createSchoolDto.LogoUrl,
            PhoneNumber = createSchoolDto.PhoneNumber,
            Status = createSchoolDto.Status,
            OrganisationId = createSchoolDto.OrganisationId,
            Organisation = organisation // attach Navigational property
        };

        await _unitOfWork.SchoolRepository.AddAsync(school);
        await _unitOfWork.SaveChangesAsync();

        // Map school to ResponseDTO
        var schoolResponse = new SchoolResponse
        {
            Name = school.Name,
            Address = school.Address,
            Email = school.Email,
            LogoUrl = school.LogoUrl,
            PhoneNumber = school.PhoneNumber,
            Status = school.Status,
            OrganisationId = school.OrganisationId,
            OrganisationName = school.Organisation?.Name!
        };

        return schoolResponse;
    }

    public async Task<SchoolResponse> UpdateAsync(Guid Id, UpdateSchoolRequest updateDto)
    {
        var UpdatedSchool = await _unitOfWork.SchoolRepository.GetByIdAsync(Id);

        if (UpdatedSchool is null)
            throw new Exception($"School with ID {Id} was not found.");

        UpdatedSchool.Name = updateDto.Name;
        UpdatedSchool.Address = updateDto.Address;
        UpdatedSchool.Email = updateDto.Email;
        UpdatedSchool.LogoUrl = updateDto.LogoUrl;
        UpdatedSchool.PhoneNumber = updateDto.PhoneNumber;
        UpdatedSchool.Status = updateDto.Status;
        UpdatedSchool.OrganisationId = updateDto.OrganisationId;

        _unitOfWork.SchoolRepository.Update(UpdatedSchool);

        await _unitOfWork.SaveChangesAsync();

        return new SchoolResponse
        {
            Name = UpdatedSchool.Name,
            Address = UpdatedSchool.Address,
            Email = UpdatedSchool.Email,
            PhoneNumber = UpdatedSchool.PhoneNumber,
            Status = UpdatedSchool.Status,
            OrganisationId = UpdatedSchool.OrganisationId
        };
    }

    public async Task<bool> DeleteAsync(Guid Id)
    {
        var school = await _unitOfWork.SchoolRepository.GetByIdAsync(Id);

        if (school is null)
            return false;

        _unitOfWork.SchoolRepository.Delete(school!);

        await _unitOfWork.SaveChangesAsync();
        return true;
    }
}