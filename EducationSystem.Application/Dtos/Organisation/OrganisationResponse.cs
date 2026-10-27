using EducationSystem.Application.Dtos.School;

namespace EducationSystem.Application.Dtos.Organisation;

public class OrganisationResponse
{
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public Guid? ApplicationUserId { get; set; }  // Foreign key to ApplicationUser
    public virtual ICollection<SchoolResponse> Schools { get; set; } = new List<SchoolResponse>();
}