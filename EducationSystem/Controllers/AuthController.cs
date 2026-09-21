//using EducationSystem.Application.Abstarctions.Identity;
using EducationSystem.Application.Abstarctions.Identity;
using EducationSystem.Application.Dtos.Request.Auth;
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

        var Result = await _authService.RegisterAsync(registerdata);

        if (!Result!.IsAuthenticated)
            return BadRequest();

        return Ok(new { token = Result.Token, expireIn = Result.ExpiresIn });
    }

    [HttpPost("Login")]
    public async Task<IActionResult> LoginAsync([FromBody] LoginRequest request, CancellationToken cancellationToken)
    {
        var authRequestResult = await _authService.GetTokenAsync(request.Email, request.Password, cancellationToken);

        if (authRequestResult is null)
            return BadRequest("Invalid Email/Password");

        return Ok(authRequestResult);
    }
}