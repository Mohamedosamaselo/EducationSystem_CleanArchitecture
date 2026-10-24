namespace EducationSystem.Application.Dtos;

public class UpdateOrganisationRequest
{
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public Guid? ApplicationUserId { get; set; }  // Foreign key to ApplicationUser
}