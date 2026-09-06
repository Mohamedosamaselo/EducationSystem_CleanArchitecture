namespace EducationSystem.Application.Dtos.Request;

public record RegisterRequestDto(
    string FirstName,
    string LastName,
    string Username,
    string Address,
    string Email,
    string Password,
    Guid? SchoolId,
    Guid? GradeId
);