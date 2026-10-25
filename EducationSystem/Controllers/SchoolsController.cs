using EducationSystem.Application.Abstarctions.HandlingError;
using EducationSystem.Application.Abstarctions.Services;
using EducationSystem.Application.Dtos;
using EducationSystem.Application.Dtos.School;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EducationSystem.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SchoolsController(ISchoolService schoolService) : ControllerBase
{
    private readonly ISchoolService _schoolService = schoolService;
    // GET

    [Authorize(Roles = "OrganisationAdmin")]
    [HttpGet("GetAll")]
    public async Task<IActionResult> GetAllAsync(CancellationToken ct)
        => (await _schoolService.GetAllAsync(ct)).ToActionResult(this);

    //// GET
    [Authorize(Roles = "SchoolAdmin,OrganisationAdmin")]
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetByIdAsync(Guid id, CancellationToken ct)
                    => (await _schoolService.GetByIdAsync(id, ct)).ToActionResult(this);

    // POST api/schools/Create

    //[Authorize(Roles = "OrganisationAdmin")]
    [HttpPost("Create")]
    public async Task<IActionResult> AddAsync([FromBody] CreateSchoolRequest request, CancellationToken ct)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        return (await _schoolService.AddAsync(request, ct)).ToActionResult(this);
    }

    // PUT  api/schools/Update/{id}

    [Authorize(Roles = "OrganisationAdmin")]
    [HttpPut("Update/{id:guid}")]
    public async Task<IActionResult> UpdateAsync(
        Guid id, [FromBody] UpdateSchoolRequest request, CancellationToken ct)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        return (await _schoolService.UpdateAsync(id, request, ct)).ToActionResult(this);
    }

    // DELETE api/schools/Delete/{id}     ← uses non-generic ToActionResult overload

    [Authorize(Roles = "OrganisationAdmin")]
    [HttpDelete("Delete/{id:guid}")]
    public async Task<IActionResult> DeleteAsync(Guid id, CancellationToken ct)
        => (await _schoolService.DeleteAsync(id, ct)).ToActionResult(this);

    // GET  api/schools/organisation/{organisationId}

    //[HttpGet("organisation/{organisationId:guid}")]
    //public async Task<IActionResult> GetAllByOrganisationIdAsync(
    //    [FromRoute] Guid organisationId, CancellationToken ct)
    //    => (await _schoolService.GetAllByOrganisationIdAsync(organisationId, ct))
    //          .ToActionResult(this);

    // GET  api/schools/search?schoolName=...

    //[HttpGet("search")]
    //public async Task<IActionResult> GetByName(
    //    [FromQuery] string schoolName, CancellationToken ct)
    //    => (await _schoolService.GetByNameAsync(schoolName, ct)).ToActionResult(this);

    // School Admin management endpoints

    [HttpGet("{schoolId:guid}/admin")]
    [Authorize(Roles = "OrganisationAdmin")]
    public async Task<IActionResult> GetSchoolAdmin(Guid schoolId, CancellationToken ct)
    {
        var result = await _schoolService.GetSchoolAdminByIdAsync(schoolId, ct);
        return result.IsSuccess ? Ok(result.Value) : ToProblem(result.Error);
    }

    [HttpPost("{schoolId:guid}/admin")]
    public async Task<IActionResult> CreateAdmin(Guid schoolId, [FromBody] CreateSchoolAdminRequest request, CancellationToken ct)
    {
        var result = await _schoolService.CreateSchoolAdminAsync(schoolId, request, ct);

        return result.IsSuccess
            ? Created($"/schools/{schoolId}/admin", result.Value)
            : ToProblem(result.Error);
    }

    [HttpPut("{schoolId:guid}/admin")]
    [Authorize(Roles = "OrganisationAdmin")]
    public async Task<IActionResult> UpdateAdmin(
        Guid schoolId, [FromBody] UpdateSchoolAdminRequest request, CancellationToken ct)
    {
        var result = await _schoolService.UpdateSchoolAdminAsync(schoolId, request, ct);

        return result.IsSuccess ? Ok(result.Value) : ToProblem(result.Error);
    }

    [HttpDelete("{schoolId:guid}/admin")]
    public async Task<IActionResult> RemoveAdmin(Guid schoolId, CancellationToken ct)
    {
        var result = await _schoolService.RemoveSchoolAdminAsync(schoolId, ct);

        return result.IsSuccess ? NoContent() : ToProblem(result.Error);
    }

    // Skip this if you already have a Result-to-HTTP mapper
    private IActionResult ToProblem(Error error) => error.Code switch
    {
        var c when c.EndsWith("NotFound") => NotFound(new { error.Code, error.Description }),
        var c when c.EndsWith("AlreadyExists") || c.EndsWith("Duplicate")
                                                                => Conflict(new { error.Code, error.Description }),
        _ => BadRequest(new { error.Code, error.Description })
    };
}