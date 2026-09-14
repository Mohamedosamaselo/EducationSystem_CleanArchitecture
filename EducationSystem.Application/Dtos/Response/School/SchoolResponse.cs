using EducationSystem.Domain.Entities;
using EducationSystem.Domain.Enums;

namespace EducationSystem.Application.Dtos.Response;

public class SchoolResponse
{
    public string Name { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string LogoUrl { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public SchoolStatus Status { get; set; } = SchoolStatus.Active;
    public Guid OrganisationId { get; set; }

    public string OrganisationName { get; set; } = string.Empty;
}