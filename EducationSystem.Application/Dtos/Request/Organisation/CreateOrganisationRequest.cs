using System;
using System.Collections.Generic;
using System.Text;

namespace EducationSystem.Application.Dtos.Request.Organisation;

public class CreateOrganisationRequest
{
    //public Guid OrganisationId { get; set; } // Required to know which Org to add the school to
    public string Name { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
}