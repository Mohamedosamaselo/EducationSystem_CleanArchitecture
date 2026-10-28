namespace EducationSystem.Application.Dtos.Grade;

public sealed record GradeResponseDto(
    Guid Id,
    string Name,
    string? Description,
    Guid SchoolId,
    string SchoolName
);