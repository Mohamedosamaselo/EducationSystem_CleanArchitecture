using EducationSystem.Application.Abstarctions.HandlingError;
using EducationSystem.Application.Abstarctions.Services;
using EducationSystem.Application.Dtos.Request.School;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EducationSystem.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class SchoolsController(ISchoolService schoolService) : ControllerBase
{
    private readonly ISchoolService _schoolService = schoolService;

    // ──────────────────────────────────────────────────────────────────
    // GET  api/schools/GetAll
    // ──────────────────────────────────────────────────────────────────
    [Authorize(Roles = "Teacher,Student,SchoolAdmin,OrganisationAdmin")]
    [HttpGet("GetAll")]
    public async Task<IActionResult> GetAllAsync(CancellationToken ct)
        => (await _schoolService.GetAllAsync(ct)).ToActionResult(this);

    // ──────────────────────────────────────────────────────────────────
    // GET  api/schools/{id}
    // ──────────────────────────────────────────────────────────────────
    [Authorize(Roles = "SchoolAdmin,OrganisationAdmin")]
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetByIdAsync(Guid id, CancellationToken ct)
        => (await _schoolService.GetByIdAsync(id, ct)).ToActionResult(this);

    // ──────────────────────────────────────────────────────────────────
    // POST api/schools/Create
    // ──────────────────────────────────────────────────────────────────
    [Authorize(Roles = "OrganisationAdmin")]
    [HttpPost("Create")]
    public async Task<IActionResult> AddAsync(
        [FromBody] CreateSchoolRequest request, CancellationToken ct)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        return (await _schoolService.AddAsync(request, ct)).ToActionResult(this);
    }

    // ──────────────────────────────────────────────────────────────────
    // PUT  api/schools/Update/{id}
    // ──────────────────────────────────────────────────────────────────
    [Authorize(Roles = "SchoolAdmin,OrganisationAdmin")]
    [HttpPut("Update/{id:guid}")]
    public async Task<IActionResult> UpdateAsync(
        Guid id, [FromBody] UpdateSchoolRequest request, CancellationToken ct)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        return (await _schoolService.UpdateAsync(id, request, ct)).ToActionResult(this);
    }

    // ──────────────────────────────────────────────────────────────────
    // DELETE api/schools/Delete/{id}     ← uses non-generic ToActionResult overload
    // ──────────────────────────────────────────────────────────────────
    [Authorize(Roles = "OrganisationAdmin")]
    [HttpDelete("Delete/{id:guid}")]
    public async Task<IActionResult> DeleteAsync(Guid id, CancellationToken ct)
        => (await _schoolService.DeleteAsync(id, ct)).ToActionResult(this);

    // ──────────────────────────────────────────────────────────────────
    // GET  api/schools/organisation/{organisationId}
    // ──────────────────────────────────────────────────────────────────
    [HttpGet("organisation/{organisationId:guid}")]
    public async Task<IActionResult> GetAllByOrganisationIdAsync(
        [FromRoute] Guid organisationId, CancellationToken ct)
        => (await _schoolService.GetAllByOrganisationIdAsync(organisationId, ct))
              .ToActionResult(this);

    // ──────────────────────────────────────────────────────────────────
    // GET  api/schools/search?schoolName=...
    // ──────────────────────────────────────────────────────────────────
    [HttpGet("search")]
    public async Task<IActionResult> GetByName(
        [FromQuery] string schoolName, CancellationToken ct)
        => (await _schoolService.GetByNameAsync(schoolName, ct)).ToActionResult(this);
}