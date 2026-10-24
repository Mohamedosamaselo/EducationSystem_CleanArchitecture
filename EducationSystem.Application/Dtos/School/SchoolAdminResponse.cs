namespace EducationSystem.Application.Dtos.School;

public sealed record SchoolAdminResponse(
    Guid Id,
    string FirstName,
    string LastName,
    string Email,
    string Username,
    Guid SchoolId);