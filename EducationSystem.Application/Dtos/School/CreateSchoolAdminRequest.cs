namespace EducationSystem.Application.Dtos.School;

public record CreateSchoolAdminRequest(
    string FirstName,
    string LastName,
    string Email,
    string Username,
    string Password
    //Guid RoleId
    );