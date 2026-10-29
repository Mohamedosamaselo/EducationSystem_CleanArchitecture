using EducationSystem.Application.Abstarctions.HandlingError;
using EducationSystem.Application.Dtos.Teacher;

namespace EducationSystem.Application.Abstarctions.Services;

public interface ITeacherService
{
    Task<Result<IReadOnlyList<TeacherGradeResponseDto>>> GetMyGradesAsync(
        Guid teacherId,
        CancellationToken ct);

    Task<Result<IReadOnlyList<TeacherSubjectResponseDto>>> GetMySubjectsAsync(
        Guid teacherId,
        CancellationToken ct);

    Task<Result<IReadOnlyList<TeacherStudentResponseDto>>> GetGradeStudentsAsync(
        Guid teacherId,
        Guid gradeId,
        CancellationToken ct);

    Task<Result<IReadOnlyList<TeacherStudentResponseDto>>> GetSubjectStudentsAsync(
        Guid teacherId,
        Guid gradeId,
        Guid subjectId,
        CancellationToken ct);

    Task<Result<TeacherStudentResponseDto>> GetStudentAsync(
        Guid teacherId,
        Guid studentId,
        CancellationToken ct);

    Task<Result<IReadOnlyList<TeacherEvaluationResponseDto>>> GetEvaluationsAsync(
        Guid teacherId,
        Guid subjectId,
        Guid gradeId,
        CancellationToken ct);

    Task<Result> AddEvaluationAsync(
        Guid teacherId,
        Guid subjectId,
        Guid studentId,
        string finalEvaluation,
        CancellationToken ct);

    Task<Result> UpdateEvaluationAsync(
        Guid teacherId,
        Guid subjectId,
        Guid studentId,
        string finalEvaluation,
        CancellationToken ct);

    Task<Result> DeleteEvaluationAsync(
        Guid teacherId,
        Guid subjectId,
        Guid studentId,
        CancellationToken ct);
}