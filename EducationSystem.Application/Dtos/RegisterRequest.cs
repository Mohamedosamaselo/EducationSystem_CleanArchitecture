namespace EducationSystem.Application.Dtos;

public record RegisterRequest(
    string FirstName,
    string LastName,
    string Email,
    DateTime? DateOfBirth,
    string Password,
    string Address,
    Guid? OrganisationId,
    Guid? SchoolId,
    Guid? GradeId,
    string? Role
);