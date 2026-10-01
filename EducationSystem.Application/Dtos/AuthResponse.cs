namespace EducationSystem.Application.Dtos;

public record AuthResponse(

      Guid Id,
    string Email,
    string Username,
    string FirstName,
    string LastName,
    string Address,
    DateTime? DateOfBirth,
    Guid? OrganisationId,
    Guid? SchoolId,
    Guid? GradeId,
    string Token,
    bool IsAuthenticated,
    DateTime ExpiresIn,
   string Role
);