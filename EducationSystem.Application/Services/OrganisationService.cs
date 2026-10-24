using EducationSystem.Application.Abstarctions.HandlingError;
using EducationSystem.Application.Abstarctions.Services;
using EducationSystem.Application.Abstarctions.UnitOfWork;
using EducationSystem.Application.Abstractions.HandlingError.Errors;
using EducationSystem.Application.Dtos;
using EducationSystem.Application.Dtos.Response.Organisation;
using EducationSystem.Domain.Entities;

namespace EducationSystem.Application.Services;

public class OrganisationService(IUnitOfWork unitOfWork) : IOrganisationService
{
    private readonly IUnitOfWork unitOfWork = unitOfWork;

    public async Task<Result<OrganisationResponse?>> GetByIdAsync(Guid id, CancellationToken CT = default)
    {
        // 1. Validate the id (defensive)
        if (id == Guid.Empty)
            return Result.Failure<OrganisationResponse?>(OrganisationErrors.InvalidInput);

        // 2. Fetch the organisation (with Schools included)
        var organisation = await unitOfWork.OrganisationRepository
            .GetByIdWithIncludeAsync(id, CT, o => o.Schools);

        // 3. Handle not found
        if (organisation is null)
            return Result.Failure<OrganisationResponse?>(OrganisationErrors.NotFound);
        // 4. Map and return
        return Result.Success<OrganisationResponse?>(MapToOrganisationResponse(organisation));
    }

    //public async Task<Result<OrganisationResponse?>> AddAsync(CreateOrganisationRequest createDto, CancellationToken CT = default)
    //{
    //    // 1. Validate input
    //    if (createDto is null)
    //        return Result.Failure<OrganisationResponse?>(OrganisationErrors.InvalidInput);

    //    if (string.IsNullOrWhiteSpace(createDto.Name))
    //        return Result.Failure<OrganisationResponse?>(
    //            new Error("Organisation.InvalidInput", "Name is required."));

    //    if (string.IsNullOrWhiteSpace(createDto.Email))
    //        return Result.Failure<OrganisationResponse?>(
    //            new Error("Organisation.InvalidInput", "Email is required."));

    //    // 2. Check for duplicate email (business rule)
    //    var existingByEmail = await unitOfWork.OrganisationRepository
    //        .FirstOrDefaultAsync(o => o.Email == createDto.Email, CT);

    //    if (existingByEmail is not null)
    //        return Result.Failure<OrganisationResponse?>(OrganisationErrors.DuplicateEmail);

    //    // 3. Check for duplicate name (business rule)
    //    var existingByName = await unitOfWork.OrganisationRepository
    //        .FirstOrDefaultAsync(o => o.Name == createDto.Name);
    //    if (existingByName is not null)
    //        return Result.Failure<OrganisationResponse?>(OrganisationErrors.DuplicateName);

    //    // 4. Map the DTO to the Organisation entity
    //    var newOrganisation = new Organisation
    //    {
    //        Id = Guid.NewGuid(),
    //        Name = createDto.Name,
    //        Email = createDto.Email,
    //        Phone = createDto.PhoneNumber,
    //        CreatedAt = DateTime.UtcNow
    //    };

    //    // 5. Add the entity to the repository
    //    await unitOfWork.OrganisationRepository.AddAsync(newOrganisation, CT);

    //    // 6. Save changes — wrap in try/catch so DB-level failures become Result.Failure
    //    try
    //    {
    //        await unitOfWork.SaveChangesAsync(CT);
    //    }
    //    catch (Exception)
    //    {
    //        // Optional: log the exception here (inject ILogger)
    //        return Result.Failure<OrganisationResponse?>(OrganisationErrors.CreationFailed);
    //    }

    //    // 7. Return the created organisation response
    //    return Result.Success<OrganisationResponse?>(MapToOrganisationResponse(newOrganisation));
    //}

    // Helper method
    private static OrganisationResponse MapToOrganisationResponse(Organisation organisation)
    {
        return new OrganisationResponse
        {
            Name = organisation.Name,
            Email = organisation.Email,
            Phone = organisation.Phone,
            ApplicationUserId = organisation.ApplicationUserId,
            Schools = organisation.Schools.Select(school => new SchoolResponse
            {
                Name = school.Name,
                Address = school.Address,
                Email = school.Email,
                PhoneNumber = school.PhoneNumber,
                //OrganisationId = school.OrganisationId,
            }).ToList()
        };
    }

    public async Task<Result<OrganisationResponse?>> UpdateAsync(Guid id, UpdateOrganisationRequest updateDto, CancellationToken ct = default)
    {
        // 1. Validate inputs
        if (updateDto is null)
            return Result.Failure<OrganisationResponse?>(OrganisationErrors.InvalidInput);

        if (id == Guid.Empty)
            return Result.Failure<OrganisationResponse?>(OrganisationErrors.InvalidInput);

        // 2. Fetch the existing organisation
        var organisation = await unitOfWork.OrganisationRepository.GetByIdAsync(id, ct);
        if (organisation is null)
            return Result.Failure<OrganisationResponse?>(OrganisationErrors.NotFound);

        // 3. Check for duplicate email (business rule)
        if (!string.IsNullOrWhiteSpace(updateDto.Email) && updateDto.Email != organisation.Email)
        {
            var existingByEmail = await unitOfWork.OrganisationRepository
                 .FirstOrDefaultAsync(o => o.Email == updateDto.Email, ct);

            if (existingByEmail is not null)
                return Result.Failure<OrganisationResponse?>(OrganisationErrors.DuplicateEmail);
        }
        // 4. Check for duplicate name (business rule)
        if (!string.IsNullOrWhiteSpace(updateDto.Name) && updateDto.Name != organisation.Name)
        {
            var existingByName = await unitOfWork.OrganisationRepository
                .FirstOrDefaultAsync(o => o.Name == updateDto.Name, ct);
            if (existingByName is not null)
                return Result.Failure<OrganisationResponse?>(OrganisationErrors.DuplicateName);
        }
        // 5. Update the organisation entity
        organisation.Name = updateDto.Name ?? organisation.Name;
        organisation.Email = updateDto.Email ?? organisation.Email;
        organisation.Phone = updateDto.Phone ?? organisation.Phone;
        organisation.ApplicationUserId = updateDto.ApplicationUserId ?? organisation.ApplicationUserId;

        organisation.ModifiedAt = DateTime.UtcNow;

        // 6. Save changes — wrap in try/catch so DB-level failures become Result.Failure
        try
        {
            await unitOfWork.SaveChangesAsync(ct);
        }
        catch (Exception)
        {
            // Optional: log the exception here (inject ILogger)
            return Result.Failure<OrganisationResponse?>(OrganisationErrors.UpdateFailed);
        }
        // 7. Return the updated organisation response
        return Result.Success<OrganisationResponse?>(MapToOrganisationResponse(organisation));
    }
}