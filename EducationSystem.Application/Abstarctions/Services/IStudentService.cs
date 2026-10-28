using EducationSystem.Application.Abstarctions.HandlingError;
using EducationSystem.Application.Dtos.Student;

namespace EducationSystem.Application.Abstarctions.Services;

public interface IStudentService
{
    Task<Result<StudentGradeResponseDto>> GetMyGradeAsync(
       Guid studentId,
       CancellationToken ct = default);

    Task<Result<IReadOnlyList<StudentSubjectResponseDto>>> GetMySubjectsAsync(
        Guid studentId,
        CancellationToken ct = default);

    Task<Result<IReadOnlyList<StudentTeacherResponseDto>>> GetMyTeachersAsync(
        Guid studentId,
        CancellationToken ct = default);

    Task<Result<IReadOnlyList<StudentEvaluationResponseDto>>> GetMyEvaluationsAsync(
        Guid studentId,
        CancellationToken ct = default);

    Task<Result<StudentEvaluationResponseDto>> GetMySubjectEvaluationAsync(
        Guid studentId,
        Guid subjectId,
        CancellationToken ct = default);
}