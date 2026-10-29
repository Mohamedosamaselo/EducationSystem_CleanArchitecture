using EducationSystem.Application.Abstarctions.HandlingError;
using EducationSystem.Application.Abstarctions.Services;
using EducationSystem.Application.Dtos.Teacher;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace EducationSystem.WebApi.Controllers;

[ApiController]
[Route("api/teacher")]
[Authorize(Roles = "Teacher")]
public sealed class TeachersController(ITeacherService teacherService)
    : ControllerBase
{
    private readonly ITeacherService _teacherService = teacherService;

    private bool TryGetTeacherId(out Guid teacherId)
    {
        return Guid.TryParse(
            User.FindFirstValue(ClaimTypes.NameIdentifier),
            out teacherId);
    }

    private IActionResult HandleError(Error error)
    {
        if (error.Code.EndsWith(
                "NotFound",
                StringComparison.OrdinalIgnoreCase))
        {
            return NotFound(error);
        }

        if (error.Code.Contains(
                "NotAssigned",
                StringComparison.OrdinalIgnoreCase))
        {
            return Forbid();
        }

        return BadRequest(error);
    }

    // GET: api/teacher/grades
    [HttpGet("grades")]
    public async Task<IActionResult> GetMyGrades(CancellationToken ct)
    {
        if (!TryGetTeacherId(out var teacherId))
            return Unauthorized();

        var result = await _teacherService.GetMyGradesAsync(teacherId, ct);

        return result.IsSuccess
            ? Ok(result.Value)
            : HandleError(result.Error);
    }

    // GET: api/teacher/subjects
    [HttpGet("subjects")]
    public async Task<IActionResult> GetMySubjects(CancellationToken ct)
    {
        if (!TryGetTeacherId(out var teacherId))
            return Unauthorized();

        var result = await _teacherService.GetMySubjectsAsync(teacherId, ct);

        return result.IsSuccess
            ? Ok(result.Value)
            : HandleError(result.Error);
    }

    // GET: api/teacher/grades/{gradeId}/students
    [HttpGet("grades/{gradeId:guid}/students")]
    public async Task<IActionResult> GetGradeStudents(
        Guid gradeId,
        CancellationToken ct)
    {
        if (!TryGetTeacherId(out var teacherId))
            return Unauthorized();

        var result = await _teacherService.GetGradeStudentsAsync(
            teacherId, gradeId, ct);

        return result.IsSuccess
            ? Ok(result.Value)
            : HandleError(result.Error);
    }

    // GET: api/teacher/grades/{gradeId}/subjects/{subjectId}/students
    [HttpGet("grades/{gradeId:guid}/subjects/{subjectId:guid}/students")]
    public async Task<IActionResult> GetSubjectStudents(
        Guid gradeId,
        Guid subjectId,
        CancellationToken ct)
    {
        if (!TryGetTeacherId(out var teacherId))
            return Unauthorized();

        var result = await _teacherService.GetSubjectStudentsAsync(
            teacherId, gradeId, subjectId, ct);

        return result.IsSuccess
            ? Ok(result.Value)
            : HandleError(result.Error);
    }

    // GET: api/teacher/students/{studentId}
    [HttpGet("students/{studentId:guid}")]
    public async Task<IActionResult> GetStudent(
        Guid studentId,
        CancellationToken ct)
    {
        if (!TryGetTeacherId(out var teacherId))
            return Unauthorized();

        var result = await _teacherService.GetStudentAsync(
            teacherId, studentId, ct);

        return result.IsSuccess
            ? Ok(result.Value)
            : HandleError(result.Error);
    }

    // GET: api/teacher/subjects/{subjectId}/evaluations?gradeId={gradeId}
    [HttpGet("subjects/{subjectId:guid}/evaluations")]
    public async Task<IActionResult> GetEvaluations(
        Guid subjectId,
        [FromQuery] Guid gradeId,
        CancellationToken ct)
    {
        if (!TryGetTeacherId(out var teacherId))
            return Unauthorized();

        var result = await _teacherService.GetEvaluationsAsync(
            teacherId, subjectId, gradeId, ct);

        return result.IsSuccess
            ? Ok(result.Value)
            : HandleError(result.Error);
    }

    // POST: api/teacher/subjects/{subjectId}/students/{studentId}/evaluation
    [HttpPost("subjects/{subjectId:guid}/students/{studentId:guid}/evaluation")]
    public async Task<IActionResult> AddEvaluation(
        Guid subjectId,
        Guid studentId,
        [FromBody] SaveEvaluationRequest request,
        CancellationToken ct)
    {
        if (!TryGetTeacherId(out var teacherId))
            return Unauthorized();

        if (string.IsNullOrWhiteSpace(request.FinalEvaluation))
            return BadRequest("Final evaluation is required.");

        var result = await _teacherService.AddEvaluationAsync(
            teacherId,
            subjectId,
            studentId,
            request.FinalEvaluation,
            ct);

        return result.IsSuccess
            ? NoContent()
            : HandleError(result.Error);
    }

    // PUT: api/teacher/subjects/{subjectId}/students/{studentId}/evaluation
    [HttpPut("subjects/{subjectId:guid}/students/{studentId:guid}/evaluation")]
    public async Task<IActionResult> UpdateEvaluation(
        Guid subjectId,
        Guid studentId,
        [FromBody] SaveEvaluationRequest request,
        CancellationToken ct)
    {
        if (!TryGetTeacherId(out var teacherId))
            return Unauthorized();

        if (string.IsNullOrWhiteSpace(request.FinalEvaluation))
            return BadRequest("Final evaluation is required.");

        var result = await _teacherService.UpdateEvaluationAsync(
            teacherId,
            subjectId,
            studentId,
            request.FinalEvaluation,
            ct);

        return result.IsSuccess
            ? NoContent()
            : HandleError(result.Error);
    }

    // DELETE: api/teacher/subjects/{subjectId}/students/{studentId}/evaluation
    [HttpDelete("subjects/{subjectId:guid}/students/{studentId:guid}/evaluation")]
    public async Task<IActionResult> DeleteEvaluation(
        Guid subjectId,
        Guid studentId,
        CancellationToken ct)
    {
        if (!TryGetTeacherId(out var teacherId))
            return Unauthorized();

        var result = await _teacherService.DeleteEvaluationAsync(
            teacherId, subjectId, studentId, ct);

        return result.IsSuccess
            ? NoContent()
            : HandleError(result.Error);
    }
}