using EducationSystem.Application.Abstarctions.HandlingError;
using EducationSystem.Domain.Entities;

public interface ISchoolAssignmentService
{
    Task<Result<IReadOnlyList<Grade>>> GetTeacherGradesAsync(
        Guid schoolAdminId,
        Guid teacherId,
        CancellationToken ct = default);

    Task<Result<bool>> AssignTeacherToGradeAsync(
        Guid schoolAdminId,
        Guid teacherId,
        Guid gradeId,
        CancellationToken ct = default);

    Task<Result<bool>> RemoveTeacherFromGradeAsync(
        Guid schoolAdminId,
        Guid teacherId,
        Guid gradeId,
        CancellationToken ct = default);

    Task<Result<bool>> AssignTeacherToSubjectAsync(
        Guid schoolAdminId,
        Guid teacherId,
        Guid subjectId,
        CancellationToken ct = default);

    Task<Result<bool>> RemoveTeacherFromSubjectAsync(
        Guid schoolAdminId,
        Guid teacherId,
        Guid subjectId,
        CancellationToken ct = default);

    Task<Result<bool>> EnrolStudentInSubjectAsync(
        Guid schoolAdminId,
        Guid studentId,
        Guid subjectId,
        CancellationToken ct = default);

    Task<Result<bool>> RemoveStudentFromSubjectAsync(
        Guid schoolAdminId,
        Guid studentId,
        Guid subjectId,
        CancellationToken ct = default);
}