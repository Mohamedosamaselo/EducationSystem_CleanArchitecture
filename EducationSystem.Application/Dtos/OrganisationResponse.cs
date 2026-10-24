namespace EducationSystem.Application.Dtos.Response.Organisation;

public class OrganisationResponse
{
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public Guid? ApplicationUserId { get; set; }  // Foreign key to ApplicationUser
    public virtual ICollection<SchoolResponse> Schools { get; set; } = new List<SchoolResponse>();
}