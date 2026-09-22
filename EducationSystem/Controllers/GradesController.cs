using EducationSystem.Application.Abstarctions.Services;
using EducationSystem.Application.Dtos.Request.Grade;
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
    public async Task<IActionResult> GetAllGradesAsync()
    {
        var grades = await _gradeService.GetAllAsync();

        return grades is null ? NotFound() : Ok(grades);
    }

    [Authorize(Roles = "SchoolAdmin,OrganisationAdmin")]
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetGradeByIdAsync(Guid id)
    {
        var grade = await _gradeService.GetByIdAsync(id);

        return grade is null ? NotFound() : Ok(grade);
    }

    [HttpGet("search")]
    public async Task<IActionResult> SearchGradesAsync([FromQuery] string gradeName)
    {
        var grades = await _gradeService.SearchAsync(gradeName);
        return grades is null ? NotFound() : Ok(grades);
    }

    [Authorize(Roles = "SchoolAdmin,OrganisationAdmin")]
    [HttpPost]
    public async Task<IActionResult> CreateGradeAsync([FromBody] CreateGradeRequest createRequestDto)
    {
        var grade = await _gradeService.CreateAsync(createRequestDto);

        return Ok(grade);
    }

    [Authorize(Roles = "SchoolAdmin,OrganisationAdmin")]
    [HttpPut("{id:guid}")]
    public async Task<IActionResult> UpdateGradeAsync(Guid id, [FromBody] UpdateGradeRequest updateRequestDto)
    {
        var updatedGrade = await _gradeService.UpdateAsync(id, updateRequestDto);
        return updatedGrade is null ? NotFound() : Ok(updatedGrade);
    }

    [Authorize(Roles = "SchoolAdmin,OrganisationAdmin")]
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteGradeAsync(Guid id)
    {
        var deleted = await _gradeService.DeleteAsync(id);

        return deleted ? NoContent() : NotFound();
    }
}