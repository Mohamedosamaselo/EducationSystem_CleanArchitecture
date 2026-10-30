namespace EducationSystem.Application.Dtos.Auth;

public record RegisterRequest(
    string FirstName,
    string LastName,
    string Email,
    string UserName,
    DateTime? BirthDate,
    string Password,
    string Address,
    Guid? OrganisationId,
    Guid? SchoolId,
    Guid? GradeId,
    string? Role

);