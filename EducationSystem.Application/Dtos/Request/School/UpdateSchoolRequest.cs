using EducationSystem.Domain.Enums;

namespace EducationSystem.Application.Dtos.Request.School;

public class UpdateSchoolRequest
{
    public string Name { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string LogoUrl { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public SchoolStatus Status { get; set; } = SchoolStatus.Active;
    public Guid OrganisationId { get; set; }
}