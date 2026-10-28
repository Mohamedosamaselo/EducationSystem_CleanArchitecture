using EducationSystem.Application.Abstarctions.Services;
using EducationSystem.Application.Dtos.SchoolUsers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace EducationSystem.WebApi.Controllers;

[ApiController]
[Route("api/school")]
[Authorize(Roles = "SchoolAdmin")]
public class SchoolUsersController(ISchoolUserService schoolUserService) : ControllerBase
{
    private readonly ISchoolUserService _schoolUserService = schoolUserService;

    // GET: api/school/users?role=teacher
    // GET: api/school/users?role=student

    [HttpGet("users")]
    public async Task<IActionResult> GetUsers([FromQuery] string role, CancellationToken ct)
    {
        var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (!Guid.TryParse(userIdClaim, out var schoolAdminId))
        {
            return Unauthorized(new
            {
                Code = "Auth.InvalidUserId",
                Description = "The user ID claim is missing or invalid."
            });
        }

        var result = await _schoolUserService.GetUsersAsync(
            schoolAdminId, role, ct);

        if (!result.IsSuccess)
            return BadRequest(new
            {
                result.Error.Code,
                result.Error.Description
            });

        return Ok(result.Value);
    }

    // POST: api/school/teachers

    [HttpPost("teachers")]
    public async Task<IActionResult> CreateTeacher(
        [FromBody] CreateSchoolUserRequest request,
        CancellationToken ct)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var schoolAdminId = Guid.Parse(
            User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        var result = await _schoolUserService.CreateTeacherAsync(
            schoolAdminId, request, ct);

        if (!result.IsSuccess)
            return BadRequest(new
            {
                result.Error.Code,
                result.Error.Description
            });

        return Created(
            $"/api/school/users/{result.Value!.Id}",
            result.Value);
    }

    // POST: api/school/students

    [HttpPost("students")]
    public async Task<IActionResult> CreateStudent(
        [FromBody] CreateSchoolUserRequest request,
        CancellationToken ct)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var schoolAdminId = Guid.Parse(
            User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        var result = await _schoolUserService.CreateStudentAsync(
            schoolAdminId, request, ct);

        if (!result.IsSuccess)
            return BadRequest(new
            {
                result.Error.Code,
                result.Error.Description
            });

        return Created(
            $"/api/school/users/{result.Value!.Id}",
            result.Value);
    }

    // GET: api/school/users/{userId}

    [HttpGet("users/{userId:guid}")]
    public async Task<IActionResult> GetUserById(
        Guid userId,
        CancellationToken ct)
    {
        var schoolAdminId = Guid.Parse(
            User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        var result = await _schoolUserService.GetByIdAsync(
            schoolAdminId, userId, ct);

        if (!result.IsSuccess)
            return NotFound(new
            {
                result.Error.Code,
                result.Error.Description
            });

        return Ok(result.Value);
    }

    // PUT: api/school/users/{userId}

    [HttpPut("users/{userId:guid}")]
    public async Task<IActionResult> UpdateUser(
        Guid userId,
        [FromBody] UpdateSchoolUserRequest request,
        CancellationToken ct)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var schoolAdminId = Guid.Parse(
            User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        var result = await _schoolUserService.UpdateAsync(
            schoolAdminId, userId, request, ct);

        if (!result.IsSuccess)
            return BadRequest(new
            {
                result.Error.Code,
                result.Error.Description
            });

        return Ok(result.Value);
    }

    // DELETE: api/school/users/{userId}

    [HttpDelete("users/{userId:guid}")]
    public async Task<IActionResult> DeleteUser(
        Guid userId,
        CancellationToken ct)
    {
        var schoolAdminId = Guid.Parse(
            User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        var result = await _schoolUserService.DeleteAsync(
            schoolAdminId, userId, ct);

        if (!result.IsSuccess)
            return BadRequest(new
            {
                result.Error.Code,
                result.Error.Description
            });

        return NoContent();
    }

    // PUT: api/school/users/{userId}/password

    [HttpPut("users/{userId:guid}/ResetPassword")]
    public async Task<IActionResult> ResetPassword(
        Guid userId,
        [FromBody] ResetSchoolUserPasswordRequest request,
        CancellationToken ct)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var schoolAdminId = Guid.Parse(
            User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        var result = await _schoolUserService.ResetPasswordAsync(
            schoolAdminId, userId, request, ct);

        if (!result.IsSuccess)
            return BadRequest(new
            {
                result.Error.Code,
                result.Error.Description
            });

        return NoContent();
    }

    // GET: api/school/grades
    [HttpGet("grades")]
    public async Task<IActionResult> GetGrades(
        CancellationToken ct)
    {
        var schoolAdminId = Guid.Parse(
            User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        var result = await _schoolUserService.GetGradesAsync(
            schoolAdminId,
            ct);

        if (!result.IsSuccess)
        {
            return BadRequest(new
            {
                result.Error.Code,
                result.Error.Description
            });
        }

        return Ok(result.Value);
    }

    // GET: api/school/grades/{gradeId}/subjects
    [HttpGet("grades/{gradeId:guid}/subjects")]
    public async Task<IActionResult> GetGradeSubjects(
        Guid gradeId,
        CancellationToken ct)
    {
        var schoolAdminId = Guid.Parse(
            User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        var result = await _schoolUserService.GetGradeSubjectsAsync(
            schoolAdminId,
            gradeId,
            ct);

        if (!result.IsSuccess)
        {
            return BadRequest(new
            {
                result.Error.Code,
                result.Error.Description
            });
        }

        return Ok(result.Value);
    }

    // GET: api/school/grades/{gradeId}/students
    [HttpGet("grades/{gradeId:guid}/students")]
    public async Task<IActionResult> GetGradeStudents(
        Guid gradeId,
        CancellationToken ct)
    {
        var schoolAdminId = Guid.Parse(
            User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        var result = await _schoolUserService.GetGradeStudentsAsync(
            schoolAdminId,
            gradeId,
            ct);

        if (!result.IsSuccess)
        {
            return BadRequest(new
            {
                result.Error.Code,
                result.Error.Description
            });
        }

        return Ok(result.Value);
    }
}