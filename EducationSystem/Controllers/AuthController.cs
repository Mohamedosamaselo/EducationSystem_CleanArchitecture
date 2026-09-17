using EducationSystem.Application.Abstarctions.Identity;
using EducationSystem.Application.Dtos.Request;
using EducationSystem.Application.Dtos.Request.Auth;
using Microsoft.AspNetCore.Mvc;

namespace EducationSystem.WebApi.Controllers;

[Route("[controller]")]
[ApiController]
public class AuthController(IAuthService authService) : ControllerBase
{
    private readonly IAuthService _authService = authService;

    [HttpPost("Register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequestDto registerdata)
    {
        // ckeck on modelState
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        // Check on Register Result
        var Result = await _authService.RegisterAsync(registerdata);

        if (!Result!.IsAuthenticated) // if user Not Authenticated [not have email , username]
            return BadRequest(Result.Message);

        return Ok(new { token = Result.UserToken, expireIn = Result.ExpiresAt });
    }

    [HttpPost("Login")]
    public async Task<IActionResult> Login([FromBody] LoginRequestDto logindata)
    {
        // ckeck on modelState
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        // Check on login Result
        var Result = await _authService.LoginAsync(logindata);

        if (!Result.IsAuthenticated) // if user Not Authenticated [not have email , username , ]
            return BadRequest(Result.Message);

        return Ok(Result);
        //return Ok(new { token = Result.UserToken, expireIn = Result.ExpiresAt });
    }

    //[Authorize] // to add Role you must be Authorized User
    //[HttpPost("AddRole")]
    //public async Task<IActionResult> AddRoleAsync([FromBody] AddRoleModel model)
    //{
    //    // ckeck on modelState
    //    if (!ModelState.IsValid)
    //        return BadRequest(ModelState);

    //    // Check on login Result
    //    var Result = await _authService.AddRoleAsync(model);

    //    if (!string.IsNullOrEmpty(Result))
    //        return BadRequest(Result);

    //    return Ok(model);
    //}
}