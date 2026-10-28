namespace EducationSystem.Application.Dtos.Student;

public sealed record StudentTeacherResponseDto(
    Guid Id,
    string FirstName,
    string LastName
);