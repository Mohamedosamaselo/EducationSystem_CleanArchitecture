using EducationSystem.Application.Abstarctions.HandlingError;
using EducationSystem.Application.Abstarctions.Identity;
using EducationSystem.Application.Dtos;
using Microsoft.AspNetCore.Mvc;

//using NETCore.MailKit.Core;
using System.Security.Claims;

using EducationSystem.Application.Abstarctions.Services;

namespace EducationSystem.WebApi.Controllers;

[Route("[controller]")]
[ApiController]
public class AuthController(IAuthService authService, IEmailService emailService) : ControllerBase
{
    private readonly IAuthService _authService = authService;
    private readonly IEmailService _emailService = emailService;

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

    [HttpPost("ConfirmEmail")]
    public async Task<IActionResult> ConfirmEmail([FromBody] ConfirmEmailRequest request)
    {
        var result = await _authService.ConfirmEmailAsync(request);
        return result.IsSuccess ? Ok(new { message = result }) : result.ToActionResult(this);
    }

    [HttpPost("ResendConfirmEmail")]
    public async Task<IActionResult> ResendConfirmationEmail(
     [FromBody] ResendConfirmationEmailRequest request)
    {
        var result = await _authService.ResendConfirmationEmailAsync(request);

        return Ok(new
        {
            message = result
        });
    }

    [HttpPost("ForgotPassword")]
    public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordRequest request)
    {
        var result = await _authService.ForgotPasswordAsync(request);
        return result.IsSuccess ? Ok(new { message = result.Value }) : result.ToActionResult(this);
    }

    [HttpPost("ResetPassword")]
    public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordRequest request)
    {
        var result = await _authService.ResetPasswordAsync(request);
        return result.IsSuccess
            ? Ok(new { message = "Password has been reset successfully." })
            : result.ToActionResult(this);
    }

    [HttpPost("ChangePassword")]
    public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequest request, CancellationToken cancellationToken)
    {
        // Get the authenticated user's ID from the JWT
        var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);

        // Make sure the claim exists and contains a valid Guid
        if (!Guid.TryParse(userIdClaim, out var userId))
        {
            return Unauthorized(new
            {
                message = "Invalid user identity."
            });
        }
        var result = await _authService.ChangePasswordAsync(userIdClaim, request);

        return result.IsSuccess
            ? Ok(new { message = result.Value })
            : result.ToActionResult(this);
    }

    [HttpGet("test-email")]
    public async Task<IActionResult> TestEmail()
    {
        await _emailService.SendAsync(
            "mohamedosamaselo@gmail.com",
            "Education System Test",
            """
        <h1>Email Test</h1>
        <p>If you received this email, MailKit is working.</p>
        """);

        return Ok("Email sent successfully.");
    }
}