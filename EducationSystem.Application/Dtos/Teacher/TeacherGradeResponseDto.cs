namespace EducationSystem.Application.Dtos.Teacher;

public sealed record TeacherGradeResponseDto(
    Guid Id,
    string Name,
    string? Description
);