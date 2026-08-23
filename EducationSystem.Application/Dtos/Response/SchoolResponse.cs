namespace EducationSystem.Application.Dtos.Response;

public class SchoolResponse
{
    public string Name { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;

    public Guid OrganisationId { get; set; }
}