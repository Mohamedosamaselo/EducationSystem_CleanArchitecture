using EducationSystem.Application.Abstarctions.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

[ApiController]
[Route("student")]
[Authorize(Roles = "Student")]
public class StudentController : ControllerBase
{
    private readonly IStudentService _studentService;

    public StudentController(IStudentService studentService)
    {
        _studentService = studentService;
    }

    [HttpGet("grade")]
    public async Task<IActionResult> GetMyGrade(CancellationToken ct)
    {
        if (!TryGetStudentId(out var studentId))
            return Unauthorized();

        var result = await _studentService.GetMyGradeAsync(studentId, ct);

        if (!result.IsSuccess)
            return result.Error.Code.EndsWith("NotFound")
                ? NotFound(result.Error)
                : BadRequest(result.Error);

        return Ok(result.Value);
    }

    [HttpGet("subjects")]
    public async Task<IActionResult> GetMySubjects(CancellationToken ct)
    {
        if (!TryGetStudentId(out var studentId))
            return Unauthorized();

        var result = await _studentService.GetMySubjectsAsync(studentId, ct);

        if (!result.IsSuccess)
            return result.Error.Code.EndsWith("NotFound")
                ? NotFound(result.Error)
                : BadRequest(result.Error);

        return Ok(result.Value);
    }

    [HttpGet("teachers")]
    public async Task<IActionResult> GetMyTeachers(CancellationToken ct)
    {
        if (!TryGetStudentId(out var studentId))
            return Unauthorized();

        var result = await _studentService.GetMyTeachersAsync(studentId, ct);

        if (!result.IsSuccess)
            return result.Error.Code.EndsWith("NotFound")
                ? NotFound(result.Error)
                : BadRequest(result.Error);

        return Ok(result.Value);
    }

    [HttpGet("evaluations")]
    public async Task<IActionResult> GetMyEvaluations(CancellationToken ct)
    {
        if (!TryGetStudentId(out var studentId))
            return Unauthorized();

        var result = await _studentService.GetMyEvaluationsAsync(studentId, ct);

        if (!result.IsSuccess)
            return result.Error.Code.EndsWith("NotFound")
                ? NotFound(result.Error)
                : BadRequest(result.Error);

        return Ok(result.Value);
    }

    [HttpGet("subjects/{subjectId:guid}/evaluation")]
    public async Task<IActionResult> GetMySubjectEvaluation(
        Guid subjectId,
        CancellationToken ct)
    {
        if (!TryGetStudentId(out var studentId))
            return Unauthorized();

        var result = await _studentService.GetMySubjectEvaluationAsync(
            studentId, subjectId, ct);

        if (!result.IsSuccess)
            return result.Error.Code.EndsWith("NotFound")
                ? NotFound(result.Error)
                : BadRequest(result.Error);

        return Ok(result.Value);
    }

    private bool TryGetStudentId(out Guid studentId)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        return Guid.TryParse(userId, out studentId);
    }
}