namespace EducationSystem.Application.Dtos.Auth;

public record RegisterRequestDto(
    string Name,
    string Username,
    string Address,
    string Email,
    string Password,
    Guid SchoolId,
    Guid GradeId
);