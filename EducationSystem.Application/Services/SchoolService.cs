using EducationSystem.Application.Abstarctions.HandlingError;
using EducationSystem.Application.Abstarctions.HandlingError.Errors;
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
    public async Task<Result<IReadOnlyList<SchoolResponse>>> GetAllAsync(CancellationToken ct = default)
    {
        var schools = await _unitOfWork.SchoolRepository.GetAllAsync(ct);

        var response = schools.Select(MapToSchoolResponse).ToList();
        return Result.Success<IReadOnlyList<SchoolResponse>>(response);
    }

    // getById
    public async Task<Result<SchoolResponse?>> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        if (id == Guid.Empty)
            return Result.Failure<SchoolResponse?>(SchoolErrors.InvalidInput);

        var school = await _unitOfWork.SchoolRepository.GetByIdAsync(id, ct);
        if (school is null)
            return Result.Failure<SchoolResponse?>(SchoolErrors.NotFound);

        return Result.Success<SchoolResponse?>(MapToSchoolResponse(school));
    }

    // search school by name
    public async Task<Result<SchoolResponse?>> GetByNameAsync(string schoolName, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(schoolName))
            return Result.Failure<SchoolResponse?>(SchoolErrors.InvalidInput);

        var school = await _unitOfWork.SchoolRepository.GetByNameAsync(schoolName, ct);
        if (school is null)
            return Result.Failure<SchoolResponse?>(SchoolErrors.NotFound);

        return Result.Success<SchoolResponse?>(MapToSchoolResponse(school));
    }

    public async Task<Result<IReadOnlyList<SchoolResponse>>> GetAllByOrganisationIdAsync(
        Guid organisationId, CancellationToken ct = default)
    {
        if (organisationId == Guid.Empty)
            return Result.Failure<IReadOnlyList<SchoolResponse>>(SchoolErrors.InvalidInput);

        // Verify the organisation exists
        var organisation = await _unitOfWork.OrganisationRepository.GetByIdAsync(organisationId, ct);
        if (organisation is null)
            return Result.Failure<IReadOnlyList<SchoolResponse>>(SchoolErrors.OrganisationNotFound);

        var schools = await _unitOfWork.SchoolRepository
            .GetAllAsync(s => s.OrganisationId == organisationId, ct);

        var response = schools.Select(MapToSchoolResponse).ToList();
        return Result.Success<IReadOnlyList<SchoolResponse>>(response);
    }

    //add
    public async Task<Result<SchoolResponse?>> AddAsync(CreateSchoolRequest createDto, CancellationToken ct = default)
    {
        // 1. Validate input
        if (createDto is null)
            return Result.Failure<SchoolResponse?>(SchoolErrors.InvalidInput);

        if (string.IsNullOrWhiteSpace(createDto.Name))
            return Result.Failure<SchoolResponse?>(
                new Error("School.InvalidInput", "Name is required."));

        if (string.IsNullOrWhiteSpace(createDto.Email))
            return Result.Failure<SchoolResponse?>(
                new Error("School.InvalidInput", "Email is required."));

        // 2. Validate the referenced organisation exists
        var organisation = await _unitOfWork.OrganisationRepository
            .GetByIdAsync(createDto.OrganisationId, ct);
        if (organisation is null)
            return Result.Failure<SchoolResponse?>(SchoolErrors.OrganisationNotFound);

        // 3. Duplicate email check
        var existingByEmail = await _unitOfWork.SchoolRepository
            .FirstOrDefaultAsync(s => s.Email == createDto.Email, ct);
        if (existingByEmail is not null)
            return Result.Failure<SchoolResponse?>(SchoolErrors.DuplicateEmail);

        // 4. Duplicate name check (within the same organisation)
        var existingByName = await _unitOfWork.SchoolRepository
            .FirstOrDefaultAsync(s => s.Name == createDto.Name
                                   && s.OrganisationId == createDto.OrganisationId, ct);
        if (existingByName is not null)
            return Result.Failure<SchoolResponse?>(SchoolErrors.DuplicateName);

        // 5. Map DTO to entity
        var school = new School
        {
            Id = Guid.NewGuid(),
            Name = createDto.Name,
            Address = createDto.Address,
            Email = createDto.Email,
            LogoUrl = createDto.LogoUrl,
            PhoneNumber = createDto.PhoneNumber,
            Status = createDto.Status,
            OrganisationId = createDto.OrganisationId,
            Organisation = organisation
        };

        // 6. Add to repo
        await _unitOfWork.SchoolRepository.AddAsync(school, ct);

        // 7. Save (wrap so DB failures become Result.Failure, not exceptions)
        try
        {
            await _unitOfWork.SaveChangesAsync(ct);
        }
        catch (Exception)
        {
            // _logger.LogError(ex, "Failed to save school {Email}", createDto.Email);
            return Result.Failure<SchoolResponse?>(SchoolErrors.CreationFailed);
        }

        // 8. Return mapped response
        return Result.Success<SchoolResponse?>(MapToSchoolResponse(school));
    }

    // Update
    public async Task<Result<SchoolResponse?>> UpdateAsync(Guid id, UpdateSchoolRequest updateDto, CancellationToken ct = default)
    {
        // 1. Validate input
        if (updateDto is null)
            return Result.Failure<SchoolResponse?>(SchoolErrors.InvalidInput);

        if (id == Guid.Empty)
            return Result.Failure<SchoolResponse?>(SchoolErrors.InvalidInput);

        // 2. Load existing school
        var school = await _unitOfWork.SchoolRepository.GetByIdAsync(id, ct);
        if (school is null)
            return Result.Failure<SchoolResponse?>(SchoolErrors.NotFound);

        // 3. If OrganisationId changed, verify the new org exists
        if (updateDto.OrganisationId != school.OrganisationId)
        {
            var newOrg = await _unitOfWork.OrganisationRepository
                .GetByIdAsync(updateDto.OrganisationId, ct);
            if (newOrg is null)
                return Result.Failure<SchoolResponse?>(SchoolErrors.OrganisationNotFound);
        }

        // 4. Apply updates
        school.Name = updateDto.Name;
        school.Address = updateDto.Address;
        school.Email = updateDto.Email;
        school.LogoUrl = updateDto.LogoUrl;
        school.PhoneNumber = updateDto.PhoneNumber;
        school.Status = updateDto.Status;
        school.OrganisationId = updateDto.OrganisationId;
        school.ModifiedAt = DateTime.UtcNow;

        _unitOfWork.SchoolRepository.Update(school);

        // 5. Save
        try
        {
            await _unitOfWork.SaveChangesAsync(ct);
        }
        catch (Exception)
        {
            // _logger.LogError(ex, "Failed to update school {Id}", id);
            return Result.Failure<SchoolResponse?>(SchoolErrors.UpdateFailed);
        }

        // 6. Return mapped response
        // NOTE: school.Organisation may now be stale if OrganisationId changed.
        // If you need accurate OrganisationName after update, reload with include:
        // var fresh = await _unitOfWork.SchoolRepository.GetByIdWithIncludeAsync(id, s => s.Organisation, ct);
        return Result.Success<SchoolResponse?>(MapToSchoolResponse(school));
    }

    // Delete
    public async Task<Result> DeleteAsync(Guid id, CancellationToken ct = default)
    {
        if (id == Guid.Empty)
            return Result.Failure(SchoolErrors.InvalidInput);

        var school = await _unitOfWork.SchoolRepository.GetByIdAsync(id, ct);
        if (school is null)
            return Result.Failure(SchoolErrors.NotFound);

        _unitOfWork.SchoolRepository.Delete(school);

        try
        {
            await _unitOfWork.SaveChangesAsync(ct);
        }
        catch (Exception)
        {
            // _logger.LogError(ex, "Failed to delete school {Id}", id);
            return Result.Failure(SchoolErrors.DeletionFailed);
        }

        return Result.Success();
    }

    // Helper — single source of truth for mapping School -> SchoolResponse
    private static SchoolResponse MapToSchoolResponse(School school)
    {
        return new SchoolResponse
        {
            Name = school.Name,
            Address = school.Address,
            Email = school.Email,
            PhoneNumber = school.PhoneNumber,
            LogoUrl = school.LogoUrl,
            Status = school.Status,
            OrganisationId = school.OrganisationId,
            OrganisationName = school.Organisation?.Name ?? string.Empty
        };
    }
}