using EducationSystem.Application.Abstarctions.Services;
using EducationSystem.Application.Dtos.Grade;
using EducationSystem.Application.Dtos.Subject;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

[ApiController]
[Route("api/school")]
[Authorize(Roles = "SchoolAdmin")]
public class SchoolAssignmentsController(
    ISchoolAssignmentService assignmentService) : ControllerBase
{
    [HttpGet("teachers/{teacherId:guid}/grades")]
    public async Task<IActionResult> GetTeacherGrades(
        Guid teacherId, CancellationToken ct)
    {
        if (!TryGetSchoolAdminId(out var adminId))
            return Unauthorized();

        var result = await assignmentService.GetTeacherGradesAsync(
            adminId, teacherId, ct);

        if (!result.IsSuccess)
            return result.Error.Code.EndsWith("NotFound")
                ? NotFound(result.Error)
                : BadRequest(result.Error);

        return Ok(result.Value);
    }

    [HttpPost("teachers/{teacherId:guid}/grades")]
    public async Task<IActionResult> AssignTeacherToGrade(
        Guid teacherId,
        [FromBody] AssignGradeRequestDto request,
        CancellationToken ct)
    {
        if (!TryGetSchoolAdminId(out var adminId))
            return Unauthorized();

        var result = await assignmentService.AssignTeacherToGradeAsync(
            adminId, teacherId, request.GradeId, ct);

        if (!result.IsSuccess)
            return result.Error.Code.EndsWith("NotFound")
                ? NotFound(result.Error)
                : BadRequest(result.Error);

        return NoContent();
    }

    [HttpDelete("teachers/{teacherId:guid}/grades/{gradeId:guid}")]
    public async Task<IActionResult> RemoveTeacherFromGrade(
        Guid teacherId, Guid gradeId, CancellationToken ct)
    {
        if (!TryGetSchoolAdminId(out var adminId))
            return Unauthorized();

        var result = await assignmentService.RemoveTeacherFromGradeAsync(
            adminId, teacherId, gradeId, ct);

        if (!result.IsSuccess)
            return result.Error.Code.EndsWith("NotFound")
                ? NotFound(result.Error)
                : BadRequest(result.Error);

        return NoContent();
    }

    [HttpPost("teachers/{teacherId:guid}/subjects")]
    public async Task<IActionResult> AssignTeacherToSubject(
        Guid teacherId,
        [FromBody] AssignSubjectRequestDto request,
        CancellationToken ct)
    {
        if (!TryGetSchoolAdminId(out var adminId))
            return Unauthorized();

        var result = await assignmentService.AssignTeacherToSubjectAsync(
            adminId, teacherId, request.SubjectId, ct);

        if (!result.IsSuccess)
            return result.Error.Code.EndsWith("NotFound")
                ? NotFound(result.Error)
                : BadRequest(result.Error);

        return NoContent();
    }

    [HttpDelete("teachers/{teacherId:guid}/subjects/{subjectId:guid}")]
    public async Task<IActionResult> RemoveTeacherFromSubject(
        Guid teacherId, Guid subjectId, CancellationToken ct)
    {
        if (!TryGetSchoolAdminId(out var adminId))
            return Unauthorized();

        var result = await assignmentService.RemoveTeacherFromSubjectAsync(
            adminId, teacherId, subjectId, ct);

        if (!result.IsSuccess)
            return result.Error.Code.EndsWith("NotFound")
                ? NotFound(result.Error)
                : BadRequest(result.Error);

        return NoContent();
    }

    [HttpPost("students/{studentId:guid}/subjects")]
    public async Task<IActionResult> EnrolStudentInSubject(
        Guid studentId,
        [FromBody] AssignSubjectRequestDto request,
        CancellationToken ct)
    {
        if (!TryGetSchoolAdminId(out var adminId))
            return Unauthorized();

        var result = await assignmentService.EnrolStudentInSubjectAsync(
            adminId, studentId, request.SubjectId, ct);

        if (!result.IsSuccess)
            return result.Error.Code.EndsWith("NotFound")
                ? NotFound(result.Error)
                : BadRequest(result.Error);

        return NoContent();
    }

    [HttpDelete("students/{studentId:guid}/subjects/{subjectId:guid}")]
    public async Task<IActionResult> RemoveStudentFromSubject(
        Guid studentId, Guid subjectId, CancellationToken ct)
    {
        if (!TryGetSchoolAdminId(out var adminId))
            return Unauthorized();

        var result = await assignmentService.RemoveStudentFromSubjectAsync(
            adminId, studentId, subjectId, ct);

        if (!result.IsSuccess)
            return result.Error.Code.EndsWith("NotFound")
                ? NotFound(result.Error)
                : BadRequest(result.Error);

        return NoContent();
    }

    private bool TryGetSchoolAdminId(out Guid schoolAdminId)
    {
        return Guid.TryParse(
            User.FindFirstValue(ClaimTypes.NameIdentifier),
            out schoolAdminId);
    }
}