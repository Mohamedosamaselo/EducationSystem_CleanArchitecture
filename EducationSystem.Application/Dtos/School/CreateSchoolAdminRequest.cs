namespace EducationSystem.Application.Dtos.School;

public record CreateSchoolAdminRequest(
    string FirstName,
    string LastName,
    string Email,
    DateTime? DateOfBirth,
    string Address,
    string Username,
    string Password
    //Guid RoleId
    );