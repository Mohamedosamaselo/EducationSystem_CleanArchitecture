namespace EducationSystem.Application.Dtos.School;

public record UpdateSchoolAdminRequest
(
    string FirstName,
    string LastName,
    string Email,
    string Phone,
    string? Password // if null keep the current password
    );