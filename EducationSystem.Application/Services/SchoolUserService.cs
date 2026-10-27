using EducationSystem.Application.Abstarctions.HandlingError;
using EducationSystem.Application.Abstarctions.Services;
using EducationSystem.Application.Abstarctions.UnitOfWork;
using EducationSystem.Application.Dtos.School;
using EducationSystem.Application.Dtos.SchoolUsers;
using EducationSystem.Domain.Entities;
using Microsoft.AspNetCore.Identity;

namespace EducationSystem.Application.Services;

public class SchoolUserService(
    IUnitOfWork unitOfWork,
    UserManager<ApplicationUser> userManager,
    RoleManager<ApplicationRole> roleManager)
    : ISchoolUserService
{
    #region Feilds

    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly UserManager<ApplicationUser> _userManager = userManager;
    private readonly RoleManager<ApplicationRole> _roleManager = roleManager;

    #endregion Feilds

    #region Methods

    public Task<Result<SchoolUserResponseDto?>> CreateTeacherAsync(
        Guid schoolAdminId,
        CreateSchoolUserRequest request,
        CancellationToken ct = default)
        => CreateUserAsync(schoolAdminId, request, "Teacher", ct);

    public Task<Result<SchoolUserResponseDto?>> CreateStudentAsync(
        Guid schoolAdminId,
        CreateSchoolUserRequest request,
        CancellationToken ct = default)
        => CreateUserAsync(schoolAdminId, request, "Student", ct);

    public async Task<Result<SchoolUserResponseDto?>> GetByIdAsync(
        Guid schoolAdminId, Guid userId, CancellationToken ct = default)
    {
        var school = await GetAdminSchoolAsync(schoolAdminId, ct);

        if (school is null)
            return Result.Failure<SchoolUserResponseDto?>(
                new Error("School.NotFound", "Assigned school was not found."));

        var user = await _unitOfWork.UserRepository.FirstOrDefaultAsync(
            u => u.Id == userId && u.SchoolId == school.Id, ct);

        if (user is null)
            return Result.Failure<SchoolUserResponseDto?>(
                new Error("User.NotFound",
                    "The user was not found in your school."));

        var role = await _roleManager.FindByIdAsync(user.RoleId.ToString());

        if (role is null || (role.Name != "Teacher" && role.Name != "Student"))
            return Result.Failure<SchoolUserResponseDto?>(
                new Error("User.InvalidRole",
                    "This endpoint only supports teachers and students."));

        return Result.Success<SchoolUserResponseDto?>(
            Map(user, role.Name));
    }

    public async Task<Result<SchoolUserResponseDto?>> UpdateAsync(
        Guid schoolAdminId, Guid userId,
        UpdateSchoolUserRequest request, CancellationToken ct = default)
    {
        var result = await GetByIdAsync(schoolAdminId, userId, ct);

        if (!result.IsSuccess || result.Value is null)
            return result;

        var user = await _unitOfWork.UserRepository.FirstOrDefaultAsync(
            u => u.Id == userId, ct);

        if (user is null)
            return Result.Failure<SchoolUserResponseDto?>(
                new Error("User.NotFound", "User was not found."));

        user.FirstName = request.FirstName.Trim();
        user.LastName = request.LastName.Trim();
        user.Address = request.Address!;
        user.DateOfBirth = request.DateOfBirth;
        user.Email = request.Email;

        var updateResult = await _userManager.UpdateAsync(user);

        if (!updateResult.Succeeded)
        {
            var description = string.Join(
                "; ", updateResult.Errors.Select(e => e.Description));

            return Result.Failure<SchoolUserResponseDto?>(
                new Error("User.UpdateFailed", description));
        }

        return result;
    }

    public async Task<Result<bool>> DeleteAsync(
        Guid schoolAdminId, Guid userId, CancellationToken ct = default)
    {
        var result = await GetByIdAsync(schoolAdminId, userId, ct);

        if (!result.IsSuccess || result.Value is null)
            return Result.Failure<bool>(result.Error);

        var user = await _userManager.FindByIdAsync(userId.ToString());

        if (user is null)
            return Result.Failure<bool>(
                new Error("User.NotFound", "User was not found."));

        var deleteResult = await _userManager.DeleteAsync(user);

        if (!deleteResult.Succeeded)
        {
            var description = string.Join(
                "; ", deleteResult.Errors.Select(e => e.Description));

            return Result.Failure<bool>(
                new Error("User.DeleteFailed", description));
        }

        return Result.Success(true);
    }

    public async Task<Result<bool>> ResetPasswordAsync(
        Guid schoolAdminId, Guid userId,
        ResetSchoolUserPasswordRequest request,
        CancellationToken ct = default)
    {
        var result = await GetByIdAsync(schoolAdminId, userId, ct);

        if (!result.IsSuccess || result.Value is null)
            return Result.Failure<bool>(result.Error);

        var user = await _userManager.FindByIdAsync(userId.ToString());

        if (user is null)
            return Result.Failure<bool>(
                new Error("User.NotFound", "User was not found."));

        var token = await _userManager.GeneratePasswordResetTokenAsync(user);

        var resetResult = await _userManager.ResetPasswordAsync(
            user, token, request.NewPassword);

        if (!resetResult.Succeeded)
        {
            var description = string.Join(
                "; ", resetResult.Errors.Select(e => e.Description));

            return Result.Failure<bool>(
                new Error("User.PasswordResetFailed", description));
        }

        return Result.Success(true);
    }

    public async Task<Result<List<SchoolUserResponseDto>>> GetUsersAsync(
        Guid schoolAdminId, string role, CancellationToken ct = default)
    {
        var school = await GetAdminSchoolAsync(schoolAdminId, ct);

        if (school is null)
            return Result.Failure<List<SchoolUserResponseDto>>(
                new Error("School.NotFound", "Assigned school was not found."));

        var roleName = role.Trim().ToLowerInvariant() switch
        {
            "teacher" => "Teacher",
            "student" => "Student",
            _ => null
        };

        if (roleName is null)
            return Result.Failure<List<SchoolUserResponseDto>>(
                new Error("User.InvalidRole",
                    "Role must be either 'teacher' or 'student'."));

        var roleEntity = await GetRoleAsync(roleName);

        if (roleEntity is null)
            return Result.Failure<List<SchoolUserResponseDto>>(
                new Error("User.RoleNotFound",
                    $"The {roleName} role is not configured."));

        var users = await _unitOfWork.UserRepository.GetAllAsync(
            u => u.SchoolId == school.Id && u.RoleId == roleEntity.Id);

        var response = users
            .Select(u => Map(u, roleName))
            .ToList();

        return Result.Success<List<SchoolUserResponseDto>>(response);
    }

    public async Task<Result<List<SchoolGradeResponseDto>>> GetGradesAsync(
        Guid schoolAdminId,
        CancellationToken ct = default)
    {
        // Validate the school admin and get the associated school
        var school = await GetAdminSchoolAsync(schoolAdminId, ct);

        if (school is null)
        {
            return Result.Failure<List<SchoolGradeResponseDto>>(
                new Error(
                    "School.NotFound",
                    "Assigned school was not found."));
        }

        // Retrieve all grades associated with the school
        var grades = await _unitOfWork.GradeRepository.GetAllAsync(
            g => g.SchoolId == school.Id);

        var response = grades
            .Select(g => new SchoolGradeResponseDto
            {
                Id = g.Id,
                Name = g.Name,
                Description = g.Description,
            })
            .ToList();

        return Result.Success<List<SchoolGradeResponseDto>>(response);
    }

    public async Task<Result<List<SchoolSubjectResponseDto>>> GetGradeSubjectsAsync(
      Guid schoolAdminId,
      Guid gradeId,
      CancellationToken ct = default)
    {
        // Validate the school admin and get the associated school
        var school = await GetAdminSchoolAsync(schoolAdminId, ct);

        if (school is null)
        {
            return Result.Failure<List<SchoolSubjectResponseDto>>(
                new Error(
                    "School.NotFound",
                    "Assigned school was not found."));
        }

        // Retrieve the grade associated with the school and the provided gradeId
        var grade = await _unitOfWork.GradeRepository.FirstOrDefaultAsync(
            g => g.Id == gradeId && g.SchoolId == school.Id,
            ct);

        if (grade is null)
        {
            return Result.Failure<List<SchoolSubjectResponseDto>>(
                new Error(
                    "Grade.NotFound",
                    "Grade was not found in your school."));
        }

        // Retrieve the subjects associated with the grade
        var subjects = grade.Subjects
            .Select(s => new SchoolSubjectResponseDto
            {
                Id = s.Id,
                Name = s.Name,
            })
            .ToList();

        return Result.Success<List<SchoolSubjectResponseDto>>(subjects);
    }

    public async Task<Result<List<SchoolGradeStudentResponseDto>>> GetGradeStudentsAsync(
     Guid schoolAdminId,
     Guid gradeId,
     CancellationToken ct = default)
    {
        // Validate the school admin and get the associated school
        var school = await GetAdminSchoolAsync(schoolAdminId, ct);

        if (school is null)
        {
            return Result.Failure<List<SchoolGradeStudentResponseDto>>(
                new Error(
                    "School.NotFound",
                    "Assigned school was not found."));
        }
        //  Retrieve the grade associated with the school and the provided gradeId
        var grade = await _unitOfWork.GradeRepository.FirstOrDefaultAsync(
            g => g.Id == gradeId && g.SchoolId == school.Id,
            ct);

        if (grade is null)
        {
            return Result.Failure<List<SchoolGradeStudentResponseDto>>(
                new Error(
                    "Grade.NotFound",
                    "Grade was not found in your school."));
        }
        // Retrieve the "Student" role

        var studentRole = await _roleManager.FindByNameAsync("Student");

        if (studentRole is null)
        {
            return Result.Failure<List<SchoolGradeStudentResponseDto>>(
                new Error(
                    "User.RoleNotFound",
                    "Student role is not configured."));
        }
        // Retrieve all students associated with the grade and school

        var students = await _unitOfWork.UserRepository.GetAllAsync(
            u => u.SchoolId == school.Id
              && u.GradeId == gradeId
              && u.RoleId == studentRole.Id);

        var response = students
            .Select(s => new SchoolGradeStudentResponseDto
            {
                Id = s.Id,
                FirstName = s.FirstName,
                LastName = s.LastName,
                Email = s.Email ?? string.Empty,
                DateOfBirth = s.DateOfBirth ?? DateTime.MinValue
            })
            .ToList();

        return Result.Success<List<SchoolGradeStudentResponseDto>>(response);
    }

    //public async Task<Result<bool>> AssignSubjectAsync(
    //   Guid schoolAdminId, Guid gradeId, Guid subjectId,
    //   CancellationToken ct = default)
    //{
    //    var grade = await GetGradeInOrganisationAsync(
    //        schoolAdminId, gradeId, ct);

    //    if (grade is null)
    //        return Result.Failure<bool>(
    //            new Error("Grade.NotFound", "Grade was not found."));

    //    var subject = await _unitOfWork.SubjectRepository.FirstOrDefaultAsync(
    //        s => s.Id == subjectId, ct);

    //    if (subject is null)
    //        return Result.Failure<bool>(
    //            new Error("Subject.NotFound", "Subject was not found."));

    //    if (grade.Subjects.Any(s => s.Id == subjectId))
    //        return Result.Failure<bool>(
    //            new Error("GradeSubject.AlreadyAssigned",
    //                "This subject is already assigned to the grade."));

    //    grade.Subjects.Add(subject);
    //    await _unitOfWork.SaveChangesAsync(ct);

    //    return Result.Success(true);
    //}

    //public async Task<Result<bool>> RemoveSubjectAsync(
    //    Guid schoolAdminId, Guid gradeId, Guid subjectId,
    //    CancellationToken ct = default)
    //{
    //    var grade = await GetGradeInOrganisationAsync(
    //        organisationAdminId, gradeId, ct);

    //    if (grade is null)
    //        return Result.Failure<bool>(
    //            new Error("Grade.NotFound", "Grade was not found."));

    //    var subject = grade.Subjects.FirstOrDefault(s => s.Id == subjectId);

    //    if (subject is null)
    //        return Result.Failure<bool>(
    //            new Error("GradeSubject.NotFound",
    //                "Subject is not assigned to this grade."));

    //    grade.Subjects.Remove(subject);
    //    await _unitOfWork.SaveChangesAsync(ct);

    //    return Result.Success(true);
    //}

    #endregion Methods

    #region Helpers Method

    private async Task<ApplicationRole?> GetRoleAsync(string roleName)
    {
        return await _roleManager.FindByNameAsync(roleName);
    }

    private static SchoolUserResponseDto Map(ApplicationUser user, string role)
    {
        return new SchoolUserResponseDto
        {
            Id = user.Id,
            FirstName = user.FirstName,
            LastName = user.LastName,
            Email = user.Email ?? string.Empty,
            Address = user.Address,
            DateOfBirth = user.DateOfBirth,
            Role = role,
            SchoolId = user.SchoolId.ToString() is not null ? Guid.Parse(user.SchoolId.ToString()!) : Guid.Empty
        };
    }

    private async Task<Result<SchoolUserResponseDto?>> CreateUserAsync(
        Guid schoolAdminId,
        CreateSchoolUserRequest request,
        string roleName,
        CancellationToken ct)
    {
        var school = await GetAdminSchoolAsync(schoolAdminId, ct);

        if (school is null)
        {
            return Result.Failure<SchoolUserResponseDto?>(
                new Error("School.NotFound",
                    "No school is assigned to this SchoolAdmin."));
        }

        var existing = await _userManager.FindByEmailAsync(request.Email);

        if (existing is not null)
        {
            return Result.Failure<SchoolUserResponseDto?>(
                new Error("User.DuplicateEmail",
                    "A user with this email already exists."));
        }

        var role = await GetRoleAsync(roleName);

        if (role is null)
        {
            return Result.Failure<SchoolUserResponseDto?>(
                new Error("User.RoleNotFound",
                    $"The {roleName} role is not configured."));
        }

        var user = new ApplicationUser
        {
            Id = Guid.NewGuid(),
            FirstName = request.FirstName.Trim(),
            LastName = request.LastName.Trim(),
            Email = request.Email.Trim(),
            UserName = request.UserName.Trim(),
            Address = request.Address!,
            DateOfBirth = request.DateOfBirth,
            SchoolId = school.Id,
            RoleId = role.Id,
            EmailConfirmed = true,
        };

        var createResult = await _userManager.CreateAsync(
            user, request.Password);

        if (!createResult.Succeeded)
        {
            var description = string.Join(
                "; ", createResult.Errors.Select(e => e.Description));

            return Result.Failure<SchoolUserResponseDto?>(
                new Error("User.CreationFailed", description));
        }

        return Result.Success<SchoolUserResponseDto?>(
            Map(user, roleName));
    }

    private async Task<School?> GetAdminSchoolAsync(Guid schoolAdminId, CancellationToken ct)
    {
        return await _unitOfWork.SchoolRepository.FirstOrDefaultAsync(s => s.SchoolAdminId == schoolAdminId, ct);
    }

    #endregion Helpers Method
}