using EducationSystem.Application.Abstarctions.HandlingError;
using EducationSystem.Application.Dtos.Request.Auth;
using EducationSystem.Application.Dtos.Response.Auth;

namespace EducationSystem.Application.Abstarctions.Identity;

public interface IAuthService
{
    Task<Result<AuthResponse?>> RegisterAsync(RegisterRequest request, CancellationToken cancellationToken = default);

    Task<Result<AuthResponse?>> GetTokenAsync(string Email, string Password, CancellationToken cancellationToken = default);
}