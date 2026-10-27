using EducationSystem.Application.Abstarctions.Services;
using EducationSystem.Application.Dtos.Subject;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EducationSystem.WebApi.Controllers;

[Route("[controller]")]
[ApiController]
[Authorize(Roles = "OrganisationAdmin")] // Apply authorization to all actions in the controller
public class SubjectsController(ISubjectService subjectService) : ControllerBase
{
    private readonly ISubjectService _subjectService = subjectService;

    [HttpGet()]
    public async Task<IActionResult> GetAllSubjectsAsync()
    {
        var subjects = await _subjectService.GetAllAsync();
        return subjects is null ? NotFound() : Ok(subjects);
    }

    [HttpGet("{subjectId:guid}")]
    public async Task<ActionResult> GetByIdAsync(Guid subjectId)
    {
        var subject = await _subjectService.GetByIdAsync(subjectId);
        return subject is null ? NotFound() : Ok(subject);
    }

    // search by SubjectName
    //[HttpGet("{SubjectName:alpha}")]
    //public async Task<ActionResult> GetSubjectAsync(string SubjectName)
    //{
    //    var subject = await _subjectService.GetByNameAsync(SubjectName);
    //    return subject is null ? NotFound() : Ok(subject);
    //}

    [HttpPost]
    public async Task<ActionResult> AddAsync(CreateSubjectRequest subjectRequest)
    {
        var subject = await _subjectService.AddAsync(subjectRequest);
        return subject is null ? NotFound() : Ok(subject);
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult> updateAsync(Guid id,
        [FromBody] UpdateSubjectRequest subjectReq)
    {
        var updatedSubject = await _subjectService.UpdateAsync(id, subjectReq);

        return updatedSubject is null ? NotFound() : Ok(updatedSubject);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteAsync(Guid id)
    {
        var deleted = await _subjectService.DeleteAsync(id);
        return deleted ? NoContent() : NotFound();
    }
}