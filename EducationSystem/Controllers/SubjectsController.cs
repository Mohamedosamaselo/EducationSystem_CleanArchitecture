using EducationSystem.Application.Abstarctions.Services;
using EducationSystem.Application.Dtos.Request.Subject;
using EducationSystem.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace EducationSystem.WebApi.Controllers;

[Route("[controller]")]
[ApiController]
public class SubjectsController(ISubjectService subjectService) : ControllerBase
{
    private readonly ISubjectService _subjectService = subjectService;

    [Authorize(Roles = "Teacher,Student,SchoolAdmin,OrganisationAdmin")]
    [HttpGet()]
    public async Task<IActionResult> GetAllSubjectsAsync()
    {
        var subjects = await _subjectService.GetAllSubjectsAsync();
        return subjects is null ? NotFound() : Ok(subjects);
    }

    [Authorize(Roles = "SchoolAdmin,OrganisationAdmin")]
    [HttpGet("{subjectId:guid}")]
    public async Task<ActionResult> GetSubjectAsync(Guid subjectId)
    {
        var subject = await _subjectService.GetSubjectAsync(subjectId);
        return subject is null ? NotFound() : Ok(subject);
    }

    // search by SubjectName
    [HttpGet("{SubjectName:alpha}")]
    public async Task<ActionResult> GetSubjectAsync(string SubjectName)
    {
        var subject = await _subjectService.GetByNameAsync(SubjectName);
        return subject is null ? NotFound() : Ok(subject);
    }

    [Authorize(Roles = "SchoolAdmin,OrganisationAdmin")]
    [HttpPost]
    public async Task<ActionResult> AddAsync(CreateSubjectRequest subjectRequest)
    {
        var subject = await _subjectService.CreateSubjectAsync(subjectRequest);
        return subject is null ? NotFound() : Ok(subject);
    }

    [Authorize(Roles = "SchoolAdmin,OrganisationAdmin")]
    [HttpPut("{id:guid}")]
    public async Task<ActionResult> updateAsync(Guid id,
        [FromBody] UpdateSubjectRequest subjectReq)
    {
        var updatedSubject = await _subjectService.UpdateSubjectAsync(id, subjectReq);

        return updatedSubject is null ? NotFound() : Ok(updatedSubject);
    }

    [Authorize(Roles = "SchoolAdmin,OrganisationAdmin")]
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteAsync(Guid id)
    {
        var deleted = await _subjectService.DeleteAsync(id);
        return deleted ? NoContent() : NotFound();
    }
}