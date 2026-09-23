using EducationSystem.Application.Abstarctions.Services;
using EducationSystem.Application.Dtos.Request.Organisation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EducationSystem.WebApi.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize(Roles = "OrganisationAdmin")]
public class OrganisationsController(IOrganisationService _organisationService) : ControllerBase
{
    private readonly IOrganisationService organisationService = _organisationService;

    [HttpGet("{id:guid}")]
    public async Task<ActionResult> GetByIdAsync(Guid id)
    {
        var organisation = await organisationService.GetByIdAsync(id);

        if (organisation is not null)
            return Ok(organisation);

        return NotFound("sorry organisation not found ");
    }

    [HttpPost]
    public async Task<ActionResult> AddAsync([FromBody] CreateOrganisationRequest createRequest)
    {
        var newOrganisation = await organisationService.AddAsync(createRequest);

        if (newOrganisation is null)
            return BadRequest("Failed to create organisation.");
        return Ok(newOrganisation);
    }
};