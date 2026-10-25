namespace EducationSystem.Application.Dtos.School;

public sealed record SchoolAdminResponse(
    Guid Id,
    string FirstName,
    string LastName,
    string Email,
    DateTime? DateOfBirth,
    string Address,
    string Username,
    Guid SchoolId);