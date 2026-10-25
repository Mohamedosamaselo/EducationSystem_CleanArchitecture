using EducationSystem.Application.Abstarctions.HandlingError;
using EducationSystem.Application.Abstarctions.HandlingError.Errors;
using EducationSystem.Application.Abstarctions.Services;
using EducationSystem.Application.Abstarctions.UnitOfWork;
using EducationSystem.Application.Dtos;
using EducationSystem.Application.Dtos.School;
using EducationSystem.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using System.Text;

namespace EducationSystem.Application.Services;

public class SchoolService(IUnitOfWork unitOfWork, UserManager<ApplicationUser> userManager, RoleManager<ApplicationRole> roleManager) : ISchoolService
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly UserManager<ApplicationUser> _userManager = userManager;
    private readonly RoleManager<ApplicationRole> _roleManager = roleManager;

    // get all schools
    public async Task<Result<IReadOnlyList<SchoolResponse>>> GetAllAsync(CancellationToken ct = default)
    {
        var schools = await _unitOfWork.SchoolRepository.GetAllAsync(ct);

        var response = schools.Select(MapToSchoolResponse).ToList();
        return Result.Success<IReadOnlyList<SchoolResponse>>(response);
    }

    // getById for School
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
    //public async Task<Result<SchoolResponse?>> GetByNameAsync(string schoolName, CancellationToken ct = default)
    //{
    //    if (string.IsNullOrWhiteSpace(schoolName))
    //        return Result.Failure<SchoolResponse?>(SchoolErrors.InvalidInput);

    //    var school = await _unitOfWork.SchoolRepository.GetByNameAsync(schoolName, ct);
    //    if (school is null)
    //        return Result.Failure<SchoolResponse?>(SchoolErrors.NotFound);

    //    return Result.Success<SchoolResponse?>(MapToSchoolResponse(school));
    //}

    //public async Task<Result<IReadOnlyList<SchoolResponse>>> GetAllByOrganisationIdAsync(
    //    Guid organisationId, CancellationToken ct = default)
    //{
    //    if (organisationId == Guid.Empty)
    //        return Result.Failure<IReadOnlyList<SchoolResponse>>(SchoolErrors.InvalidInput);

    //    // Verify the organisation exists
    //    var organisation = await _unitOfWork.OrganisationRepository.GetByIdAsync(organisationId, ct);
    //    if (organisation is null)
    //        return Result.Failure<IReadOnlyList<SchoolResponse>>(SchoolErrors.OrganisationNotFound);

    //    var schools = await _unitOfWork.SchoolRepository
    //        .GetAllAsync(s => s.OrganisationId == organisationId, ct);

    //    var response = schools.Select(MapToSchoolResponse).ToList();
    //    return Result.Success<IReadOnlyList<SchoolResponse>>(response);
    //}

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
            PhoneNumber = createDto.PhoneNumber,
            SchoolAdminId = createDto.SchoolAdminId,
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

    // Update School
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
            var newOrg = await _unitOfWork.OrganisationRepository.GetByIdAsync(updateDto.OrganisationId, ct);

            if (newOrg is null)
                return Result.Failure<SchoolResponse?>(SchoolErrors.OrganisationNotFound);
        }

        // 4. Apply updates
        school.Name = updateDto.Name;
        school.Address = updateDto.Address;
        school.Email = updateDto.Email;

        school.PhoneNumber = updateDto.PhoneNumber;

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

    // Delete School
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

    // Get the admin of a school
    public async Task<Result<SchoolAdminResponse?>> GetSchoolAdminByIdAsync(
        Guid schoolId, CancellationToken ct = default)
    {
        // 1. Validate input
        if (schoolId == Guid.Empty)
            return Result.Failure<SchoolAdminResponse?>(SchoolAdminErrors.InvalidInput);

        // 2. School must exist
        var school = await _unitOfWork.SchoolRepository.GetByIdAsync(schoolId, ct);
        if (school is null)
            return Result.Failure<SchoolAdminResponse?>(SchoolErrors.NotFound);

        // 3. School must have an admin
        if (school.SchoolAdminId == Guid.Empty)
            return Result.Failure<SchoolAdminResponse?>(SchoolAdminErrors.NotFound);

        // 4. Load the admin user (inactive = removed)
        var admin = await _userManager.FindByIdAsync(school.SchoolAdminId.ToString());
        if (admin is null)
            return Result.Failure<SchoolAdminResponse?>(SchoolAdminErrors.NotFound);

        // 5. Return mapped response (never includes the password)
        return Result.Success<SchoolAdminResponse?>(MapToSchoolAdminResponse(admin, schoolId));
    }

    public async Task<Result<SchoolAdminResponse?>> CreateSchoolAdminAsync(Guid schoolId, CreateSchoolAdminRequest createDto, CancellationToken ct = default)
    {
        // 1. Validate input
        if (schoolId == Guid.Empty || createDto is null)
            return Result.Failure<SchoolAdminResponse?>(SchoolAdminErrors.InvalidInput);

        if (string.IsNullOrWhiteSpace(createDto.FirstName) ||
            string.IsNullOrWhiteSpace(createDto.LastName) ||
            string.IsNullOrWhiteSpace(createDto.Email) ||
            string.IsNullOrWhiteSpace(createDto.Username) ||
            string.IsNullOrWhiteSpace(createDto.Password))
            return Result.Failure<SchoolAdminResponse?>(
                new Error("SchoolAdmin.InvalidInput", "First name, last name, email, username and password are required."));

        // 2. School must exist
        var school = await _unitOfWork.SchoolRepository.GetByIdAsync(schoolId, ct);
        if (school is null)
            return Result.Failure<SchoolAdminResponse?>(SchoolErrors.NotFound);

        // 3. One admin per school
        if (school.SchoolAdminId.HasValue)
            return Result.Failure<SchoolAdminResponse?>(SchoolAdminErrors.AlreadyExists);

        // 4. Unique username / email
        //if (await _userManager.FindByNameAsync(createDto.Username) is not null ||
        //    await _userManager.FindByEmailAsync(createDto.Email) is not null)
        //     return Result.Failure<SchoolAdminResponse?>(SchoolAdminErrors.DuplicateUser);

        // 4. check on userName , email
        var existingUserByUsername =
         await _userManager.FindByNameAsync(createDto.Username);

        var existingUserByEmail =
            await _userManager.FindByEmailAsync(createDto.Email);

        //5. Put a breakpoint here and inspect both variables.
        if (existingUserByUsername is not null)
        {
            return Result.Failure<SchoolAdminResponse?>(
                SchoolAdminErrors.DuplicateUsername);
        }

        if (existingUserByEmail is not null)
        {
            return Result.Failure<SchoolAdminResponse?>(
                SchoolAdminErrors.DuplicateEmail);
        }

        // 6. Find the existing SchoolAdmin role
        var schoolAdminRole =
            await _roleManager.FindByNameAsync("SchoolAdmin");

        if (schoolAdminRole is null)
        {
            return Result.Failure<SchoolAdminResponse?>(
                SchoolAdminErrors.RoleNotConfigured);
        }

        // 7. Build the user (Identity hashes the password inside CreateAsync)
        var admin = new ApplicationUser
        {
            Id = Guid.NewGuid(),
            FirstName = createDto.FirstName,
            LastName = createDto.LastName,
            Email = createDto.Email,
            UserName = createDto.Username,
            SchoolId = schoolId,
            DateOfBirth = createDto.DateOfBirth,
            Address = createDto.Address,
            RoleId = schoolAdminRole.Id, // set it from DB
            CreatedAt = DateTime.UtcNow,
        };

        // 8. Create the user (password is hashed by UserManager)

        var createResult = await _userManager.CreateAsync(admin, createDto.Password);

        if (!createResult.Succeeded)
            return Result.Failure<SchoolAdminResponse?>(new Error(
                "SchoolAdmin.CreationFailed",
                string.Join(" ", createResult.Errors.Select(e => e.Description))));

        // 9. Assign Admin to School Then save
        school.SchoolAdminId = admin.Id;
        school.ModifiedAt = DateTime.UtcNow;

        // 10. Update the school with the new admin ID
        _unitOfWork.SchoolRepository.Update(school);

        try
        {
            await _unitOfWork.SaveChangesAsync(ct);
        }
        catch (Exception)
        {
            await _userManager.DeleteAsync(admin);   // rollback the user
            return Result.Failure<SchoolAdminResponse?>(SchoolAdminErrors.CreationFailed);
        }

        // 11. Return Response
        return Result.Success<SchoolAdminResponse?>(MapToSchoolAdminResponse(admin, schoolId));
    }

    public async Task<Result<SchoolAdminResponse?>> UpdateSchoolAdminAsync(
     Guid schoolId, UpdateSchoolAdminRequest updateDto, CancellationToken ct = default)
    {
        // 1. Validate input
        if (schoolId == Guid.Empty || updateDto is null)
            return Result.Failure<SchoolAdminResponse?>(SchoolAdminErrors.InvalidInput);

        // 2. Load school, then its admin
        var school = await _unitOfWork.SchoolRepository.GetByIdAsync(schoolId, ct);
        if (school is null)
            return Result.Failure<SchoolAdminResponse?>(SchoolErrors.NotFound);

        if (school.SchoolAdminId == Guid.Empty)
            return Result.Failure<SchoolAdminResponse?>(SchoolAdminErrors.NotFound);

        var admin = await _userManager.FindByIdAsync(school.SchoolAdminId.ToString());
        if (admin is null)
            return Result.Failure<SchoolAdminResponse?>(SchoolAdminErrors.NotFound);

        // If the email changed, it must still be unique
        if (!string.Equals(admin.Email, updateDto.Email, StringComparison.OrdinalIgnoreCase))
        {
            var emailOwner = await _userManager.FindByEmailAsync(updateDto.Email);
            if (emailOwner is not null && emailOwner.Id != admin.Id)
                return Result.Failure<SchoolAdminResponse?>(SchoolAdminErrors.DuplicateUser);
        }

        admin.FirstName = updateDto.FirstName;
        admin.LastName = updateDto.LastName;
        admin.Email = updateDto.Email;
        admin.DateOfBirth = updateDto.DateOfBirth;
        admin.Address = updateDto.Address;
        admin.UserName = updateDto.Username;
        admin.ModifiedAt = DateTime.UtcNow;

        var updateResult = await _userManager.UpdateAsync(admin);
        if (!updateResult.Succeeded)
            return Result.Failure<SchoolAdminResponse?>(SchoolAdminErrors.UpdateFailed);

        // Optional password reset
        if (!string.IsNullOrWhiteSpace(updateDto.Password))
        {
            var token = await _userManager.GeneratePasswordResetTokenAsync(admin);
            var pwResult = await _userManager.ResetPasswordAsync(admin, token, updateDto.Password);
            if (!pwResult.Succeeded)
                return Result.Failure<SchoolAdminResponse?>(new Error(
                    "SchoolAdmin.UpdateFailed",
                    string.Join(" ", pwResult.Errors.Select(e => e.Description))));
        }

        return Result.Success<SchoolAdminResponse?>(MapToSchoolAdminResponse(admin, schoolId));
    }

    public async Task<Result> RemoveSchoolAdminAsync(Guid schoolId, CancellationToken ct = default)
    {
        if (schoolId == Guid.Empty)
            return Result.Failure(SchoolAdminErrors.InvalidInput);

        var school = await _unitOfWork.SchoolRepository.GetByIdAsync(schoolId, ct);
        if (school is null)
            return Result.Failure(SchoolErrors.NotFound);

        if (school.SchoolAdminId == Guid.Empty)
            return Result.Failure(SchoolAdminErrors.NotFound);

        var admin = await _unitOfWork.UserRepository.GetByIdAsync(school.SchoolAdminId!.Value, ct);
        if (admin is null)
            return Result.Failure(SchoolAdminErrors.NotFound);

        // Soft delete the user and unlink from the school
        //admin.IsActive = false;
        admin.ModifiedAt = DateTime.UtcNow;
        _unitOfWork.UserRepository.Update(admin);

        school.SchoolAdminId = null;
        school.ModifiedAt = DateTime.UtcNow;
        _unitOfWork.SchoolRepository.Update(school);

        try
        {
            await _unitOfWork.SaveChangesAsync(ct);
        }
        catch (Exception)
        {
            return Result.Failure(SchoolAdminErrors.DeletionFailed);
        }

        return Result.Success();
    }

    // Helper: User -> SchoolAdminResponse
    private static SchoolAdminResponse MapToSchoolAdminResponse(ApplicationUser user, Guid schoolId)
    {
        return new SchoolAdminResponse(
            user.Id,
            user.FirstName,
            user.LastName,
            user.Email,
            user.DateOfBirth,
            user.Address,
            user.UserName,
            //user.SchoolId.HasValue && user.SchoolId.Value != Guid.Empty ? user.SchoolId.Value : Guid.Empty
            schoolId
        );
    }

    // Helper — School -> SchoolResponse
    private static SchoolResponse MapToSchoolResponse(School school)
    {
        return new SchoolResponse
        {
            Name = school.Name,
            Address = school.Address,
            Email = school.Email,
            PhoneNumber = school.PhoneNumber,
            OrganisationId = school.OrganisationId,
            OrganisationName = school.Organisation?.Name ?? string.Empty
        };
    }
}