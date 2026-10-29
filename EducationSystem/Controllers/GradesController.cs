using EducationSystem.Application.Abstarctions.HandlingError; // Required for ToActionResult
using EducationSystem.Application.Abstarctions.Services;
using EducationSystem.Application.Dtos.Grade;
using EducationSystem.Application.Dtos.Subject;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EducationSystem.WebApi.Controllers;

[Route("[controller]")]
[ApiController]
[Authorize] // Apply authorization to all actions in the controller
public class GradesController(IGradeService gradeService)
    : ControllerBase
{
    private readonly IGradeService _gradeService = gradeService;

    [HttpGet("")]
    [Authorize(Roles = "OrganisationAdmin , SchoolAdmin")]
    public async Task<IActionResult> GetAllAsync(CancellationToken cancellationToken)
    {
        var result = await _gradeService.GetAllAsync(cancellationToken);
        return result.ToActionResult(this);
    }

    [HttpGet("{id:guid}")]
    [Authorize(Roles = "OrganisationAdmin , SchoolAdmin")]
    public async Task<IActionResult> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        var result = await _gradeService.GetByIdAsync(id, cancellationToken);
        return result.ToActionResult(this);
    }

    [HttpPost]
    [Authorize(Roles = "OrganisationAdmin")]
    public async Task<IActionResult> CreateAsync([FromBody] CreateGradeRequest createRequestDto, CancellationToken cancellationToken)
    {
        var result = await _gradeService.AddAsync(createRequestDto, cancellationToken);
        return result.ToActionResult(this);
    }

    [HttpPut("{id:guid}")]
    [Authorize(Roles = "OrganisationAdmin")]
    public async Task<IActionResult> UpdateAsync(Guid id, [FromBody] UpdateGradeRequest updateRequestDto, CancellationToken cancellationToken)
    {
        var result = await _gradeService.UpdateAsync(id, updateRequestDto, cancellationToken);
        return result.ToActionResult(this);
    }

    [HttpDelete("{id:guid}")]
    [Authorize(Roles = "OrganisationAdmin")]
    public async Task<IActionResult> RemoveAsync(Guid id, CancellationToken cancellationToken)
    {
        var result = await _gradeService.DeleteAsync(id, cancellationToken);

        // Uses the non-generic ToActionResult (returns 204 No Content on success, or error on failure)
        return result.ToActionResult(this);
    }

    // ---- Grade & Subject ----
    [HttpGet("{gradeId:guid}/subjects")]
    [Authorize(Roles = "OrganisationAdmin , SchoolAdmin")]
    public async Task<IActionResult> GetSubjects(Guid gradeId, CancellationToken ct)
    {
        var result = await gradeService.GetSubjectsAsync(gradeId, ct);
        return result.IsSuccess ? Ok(result.Value) : ToProblem(result.Error);
    }

    [HttpPost("{gradeId:guid}/subjects")]
    [Authorize(Roles = "OrganisationAdmin")]
    public async Task<IActionResult> AssignSubject(
    Guid gradeId, [FromBody] AssignSubjectToGradeRequest request, CancellationToken ct)
    {
        var result = await _gradeService.AssignSubjectAsync(gradeId, request, ct);

        return result.IsSuccess
            ? Created($"/grades/{gradeId}/subjects", result.Value)
            : ToProblem(result.Error);
    }

    [HttpDelete("{gradeId:guid}/subjects/{subjectId:guid}")]
    [Authorize(Roles = "OrganisationAdmin")]
    public async Task<IActionResult> RemoveSubject(
        Guid gradeId, Guid subjectId, CancellationToken ct)
    {
        var result = await _gradeService.RemoveSubjectAsync(gradeId, subjectId, ct);

        return result.IsSuccess ? NoContent() : ToProblem(result.Error);
    }

    //[HttpGet("search")]
    //public async Task<IActionResult> SearchGradesAsync([FromQuery] string gradeName, CancellationToken cancellationToken)
    //{
    //    var result = await _gradeService.SearchAsync(gradeName, cancellationToken);
    //    return result.ToActionResult(this);
    //}

    // Skip this if you already have a Result-to-HTTP mapper
    private IActionResult ToProblem(Error error) => error.Code switch
    {
        var c when c.EndsWith("NotFound") => NotFound(new { error.Code, error.Description }),
        var c when c.EndsWith("AlreadyExists") || c.EndsWith("Duplicate")
          => Conflict(new { error.Code, error.Description }),
        _ => BadRequest(new { error.Code, error.Description })
    };
}