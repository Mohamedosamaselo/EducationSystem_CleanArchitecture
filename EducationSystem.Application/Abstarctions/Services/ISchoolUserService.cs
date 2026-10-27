using EducationSystem.Application.Abstarctions.HandlingError;
using EducationSystem.Application.Dtos.School;
using EducationSystem.Application.Dtos.SchoolUsers;

namespace EducationSystem.Application.Abstarctions.Services;

public interface ISchoolUserService
{
    #region SchoolWithUser Methods

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

    #endregion SchoolWithUser Methods

    #region SchoolWith Grade & Students & Subjects Methods

    Task<Result<List<SchoolGradeResponseDto>>> GetGradesAsync(
        Guid schoolAdminId,
        CancellationToken ct = default);

    Task<Result<List<SchoolSubjectResponseDto>>> GetGradeSubjectsAsync(
        Guid schoolAdminId,
        Guid gradeId,
        CancellationToken ct = default);

    Task<Result<List<SchoolGradeStudentResponseDto>>> GetGradeStudentsAsync(
        Guid schoolAdminId,
        Guid gradeId,
        CancellationToken ct = default);

    //Task<Result<bool>> AssignSubjectAsync(
    //    Guid schoolAdminId, Guid gradeId, Guid subjectId,
    //    CancellationToken ct = default);

    //Task<Result<bool>> RemoveSubjectAsync(
    //    Guid schoolAdminId, Guid gradeId, Guid subjectId,
    //    CancellationToken ct = default);

    #endregion SchoolWith Grade & Students & Subjects Methods
}