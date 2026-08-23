namespace EducationSystem.Application.Dtos.Request;

public class CreateSchoolRequest
{
    public string Name { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public Guid OrganisationId { get; set; }
}