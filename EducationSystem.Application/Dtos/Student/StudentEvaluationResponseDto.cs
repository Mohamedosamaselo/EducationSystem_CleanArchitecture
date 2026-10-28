namespace EducationSystem.Application.Dtos.Student;

public sealed record StudentEvaluationResponseDto(
    Guid StudentId,
    Guid SubjectId,
    string SubjectName,
    string? FinalEvaluation
);