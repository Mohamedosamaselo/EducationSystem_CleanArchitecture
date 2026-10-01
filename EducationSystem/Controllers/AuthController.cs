//using EducationSystem.Application.Abstarctions.Identity;
using EducationSystem.Application.Abstarctions.HandlingError;
using EducationSystem.Application.Abstarctions.Identity;
using EducationSystem.Application.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace EducationSystem.WebApi.Controllers;

[Route("[controller]")]
[ApiController]
public class AuthController(IAuthService authService) : ControllerBase
{
    private readonly IAuthService _authService = authService;

    [HttpPost("Register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequest registerdata, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var result = await _authService.RegisterAsync(registerdata);
        //return result.IsSuccess ? Ok(result.Value) : Unauthorized(new { code = result.Error.Code, description = result.Error.Description });
        return result.IsSuccess ? Ok(result.Value) : result.ToActionResult(this);
    }

    [HttpPost("Login")]
    public async Task<IActionResult> LoginAsync([FromBody] LoginRequest request, CancellationToken cancellationToken)
    {
        var result = await _authService.GetTokenAsync(request.Email, request.Password, cancellationToken);

        return result.ToActionResult(this);
    }

    [HttpPost("ForgotPassword")]
    public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordRequest request, CancellationToken cancellationToken)
    {
        var result = await _authService.ForgotPasswordAsync(request, cancellationToken);
        return result.IsSuccess ? Ok(new { message = result.Value }) : result.ToActionResult(this);
    }

    [HttpPost("ResetPassword")]
    public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordRequest request, CancellationToken cancellationToken)
    {
        var result = await _authService.ResetPasswordAsync(request, cancellationToken);
        return result.IsSuccess
            ? Ok(new { message = "Password has been reset successfully." })
            : result.ToActionResult(this);
    }

    [HttpPost("ChangePassword")]
    public async Task<IActionResult> ChangePassword(
    [FromBody] ChangePasswordRequest request,
    CancellationToken cancellationToken)
    {
        var result = await _authService.ChangePasswordAsync(
            request,
            cancellationToken);

        return result.IsSuccess
            ? Ok(new { message = result.Value })
            : result.ToActionResult(this);
    }
}