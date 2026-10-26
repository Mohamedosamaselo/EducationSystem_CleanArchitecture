using EducationSystem.Application.Abstarctions.HandlingError;
using EducationSystem.Application.Dtos.SchoolUsers;

namespace EducationSystem.Application.Abstarctions.Services;

public interface ISchoolUserService
{
    Task<Result<List<SchoolUserResponseDto>>> GetUsersAsync(
        Guid schoolAdminId, string role, CancellationToken ct = default);

    Task<Result<SchoolUserResponseDto?>> GetByIdAsync(
        Guid schoolAdminId, Guid userId, CancellationToken ct = default);

    Task<Result<SchoolUserResponseDto?>> CreateTeacherAsync(
        Guid schoolAdminId, CreateSchoolUserRequest request,
        CancellationToken ct = default);

    Task<Result<SchoolUserResponseDto?>> CreateStudentAsync(
        Guid schoolAdminId, CreateSchoolUserRequest request,
        CancellationToken ct = default);

    Task<Result<SchoolUserResponseDto?>> UpdateAsync(
        Guid schoolAdminId, Guid userId, UpdateSchoolUserRequest request,
        CancellationToken ct = default);

    Task<Result<bool>> DeleteAsync(
        Guid schoolAdminId, Guid userId, CancellationToken ct = default);

    Task<Result<bool>> ResetPasswordAsync(
        Guid schoolAdminId, Guid userId,
        ResetSchoolUserPasswordRequest request,
        CancellationToken ct = default);
}