using EducationSystem.Application.Abstarctions.Services;
using EducationSystem.Application.Dtos.Request;
using EducationSystem.Application.Dtos.Response;
using Microsoft.AspNetCore.Mvc;

using System.Diagnostics.CodeAnalysis;

namespace EducationSystem.WebApi.Controllers;

[Route("api/[controller]")]
[ApiController]
public class SchoolsController(ISchoolService schoolService) : ControllerBase
{
    private readonly ISchoolService _schoolService = schoolService;

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<SchoolResponse>>> GetAllAsync()
    {
        var schools = await _schoolService.GetAllAsync();

        return schools is not null ? Ok(schools) : NotFound("Sorry this school not Found ");
    }

    [HttpGet("organisation/{organisationId:guid}")]
    public async Task<ActionResult<IReadOnlyList<SchoolResponse>>> GetAllByOrganisationIdAsync([FromRoute] Guid organisationId)
    {
        var schools = await _schoolService.GetAllByOrganisationIdAsync(organisationId);
        return Ok(schools);
    }

    [HttpGet("search")]
    public async Task<IActionResult> GetByName([FromQuery] string schoolName)
    {
        var school = await _schoolService.GetByNameAsync(schoolName);
        if (school != null)
            return Ok(school);

        return NotFound("Sorry this School is not found ");
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult?> GetByIdAsync([FromQuery] Guid id)
    {
        var school = await _schoolService.GetByIdAsync(id);

        if (school != null)
            return Ok(school);

        return NotFound("Sorry this school Not Found");
    }

    [HttpPost]
    public async Task<ActionResult<SchoolResponse>> AddAsync([FromBody] CreateSchoolRequest request)
    {
        var school = await _schoolService.AddAsync(request);

        return Ok(school);
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<Application.Dtos.Response.SchoolResponse>> UpdateAsync(Guid id, [FromBody] UpdateSchoolRequest request)
    {
        var school = await _schoolService.UpdateAsync(id, request);

        return Ok(school);
    }

    [HttpDelete("Delete/{id:guid}")]
    public async Task<IActionResult> DeleteAsync(Guid id)
    {
        await _schoolService.DeleteAsync(id);

        return NoContent();
    }
}