namespace EducationSystem.Application.Dtos.Request.Auth;

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