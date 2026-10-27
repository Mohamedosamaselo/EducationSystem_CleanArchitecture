namespace EducationSystem.Application.Dtos.School;

public class CreateSchoolRequest
{
    public string Name { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;

    public Guid OrganisationId { get; set; }
    public Guid? SchoolAdminId { get; set; }

    //public string OrganisationName { get; set; } = string.Empty;
}