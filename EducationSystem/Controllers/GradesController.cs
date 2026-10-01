using EducationSystem.Application.Abstarctions.HandlingError; // Required for ToActionResult
using EducationSystem.Application.Abstarctions.Services;
using EducationSystem.Application.Dtos;
using EducationSystem.Application.Dtos.Grade;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EducationSystem.WebApi.Controllers;

[Route("[controller]")]
[ApiController]
public class GradesController(IGradeService gradeService) : ControllerBase
{
    private readonly IGradeService _gradeService = gradeService;

    [Authorize(Roles = "Teacher,Student,SchoolAdmin,OrganisationAdmin")]
    [HttpGet("")]
    public async Task<IActionResult> GetAllGradesAsync(CancellationToken cancellationToken)
    {
        var result = await _gradeService.GetAllAsync(cancellationToken);

        // Translates Result<T> to 200 OK (with list) or Error to 400/404/500
        return result.ToActionResult(this);
    }

    [Authorize(Roles = "SchoolAdmin,OrganisationAdmin")]
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetGradeByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        var result = await _gradeService.GetByIdAsync(id, cancellationToken);
        return result.ToActionResult(this);
    }

    [HttpGet("search")]
    public async Task<IActionResult> SearchGradesAsync([FromQuery] string gradeName, CancellationToken cancellationToken)
    {
        var result = await _gradeService.SearchAsync(gradeName, cancellationToken);
        return result.ToActionResult(this);
    }

    [Authorize(Roles = "SchoolAdmin,OrganisationAdmin")]
    [HttpPost]
    public async Task<IActionResult> CreateGradeAsync([FromBody] CreateGradeRequest createRequestDto, CancellationToken cancellationToken)
    {
        var result = await _gradeService.CreateAsync(createRequestDto, cancellationToken);
        return result.ToActionResult(this);
    }

    [Authorize(Roles = "SchoolAdmin,OrganisationAdmin")]
    [HttpPut("{id:guid}")]
    public async Task<IActionResult> UpdateGradeAsync(Guid id, [FromBody] UpdateGradeRequest updateRequestDto, CancellationToken cancellationToken)
    {
        var result = await _gradeService.UpdateAsync(id, updateRequestDto, cancellationToken);
        return result.ToActionResult(this);
    }

    [Authorize(Roles = "SchoolAdmin,OrganisationAdmin")]
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteGradeAsync(Guid id, CancellationToken cancellationToken)
    {
        var result = await _gradeService.DeleteAsync(id, cancellationToken);

        // Uses the non-generic ToActionResult (returns 204 No Content on success, or error on failure)
        return result.ToActionResult(this);
    }
}