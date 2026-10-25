namespace EducationSystem.Application.Dtos.School;

public record UpdateSchoolAdminRequest
(
    string FirstName,
    string LastName,
    string Email,
    DateTime? DateOfBirth,
    string Address,
    string Username,
    string? Password // if null keep the old password 
    );