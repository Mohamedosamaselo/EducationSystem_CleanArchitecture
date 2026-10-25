using EducationSystem.Application.Abstarctions.HandlingError; // Required for ToActionResult
using EducationSystem.Application.Abstarctions.Services;
using EducationSystem.Application.Dtos;
using EducationSystem.Application.Dtos.Grade;
using EducationSystem.Application.Dtos.Subject;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EducationSystem.WebApi.Controllers;

[Route("[controller]")]
[ApiController]
public class GradesController(IGradeService gradeService) : ControllerBase
{
    private readonly IGradeService _gradeService = gradeService;

    //[Authorize(Roles = "SchoolAdmin,OrganisationAdmin")]
    [Authorize(Roles = "OrganisationAdmin")]
    [HttpGet("")]
    public async Task<IActionResult> GetAllGradesAsync(CancellationToken cancellationToken)
    {
        var result = await _gradeService.GetAllAsync(cancellationToken);
        return result.ToActionResult(this);
    }

    [Authorize(Roles = "OrganisationAdmin")]
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetGradeByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        var result = await _gradeService.GetByIdAsync(id, cancellationToken);
        return result.ToActionResult(this);
    }

    //[Authorize(Roles = "SchoolAdmin,OrganisationAdmin")]
    [Authorize(Roles = "OrganisationAdmin")]
    [HttpPost]
    public async Task<IActionResult> CreateGradeAsync([FromBody] CreateGradeRequest createRequestDto, CancellationToken cancellationToken)
    {
        var result = await _gradeService.CreateAsync(createRequestDto, cancellationToken);
        return result.ToActionResult(this);
    }

    //[Authorize(Roles = "SchoolAdmin,OrganisationAdmin")]
    [Authorize(Roles = "OrganisationAdmin")]
    [HttpPut("{id:guid}")]
    public async Task<IActionResult> UpdateGradeAsync(Guid id, [FromBody] UpdateGradeRequest updateRequestDto, CancellationToken cancellationToken)
    {
        var result = await _gradeService.UpdateAsync(id, updateRequestDto, cancellationToken);
        return result.ToActionResult(this);
    }

    //[Authorize(Roles = "SchoolAdmin,OrganisationAdmin")]
    [Authorize(Roles = "OrganisationAdmin")]
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteGradeAsync(Guid id, CancellationToken cancellationToken)
    {
        var result = await _gradeService.DeleteAsync(id, cancellationToken);

        // Uses the non-generic ToActionResult (returns 204 No Content on success, or error on failure)
        return result.ToActionResult(this);
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