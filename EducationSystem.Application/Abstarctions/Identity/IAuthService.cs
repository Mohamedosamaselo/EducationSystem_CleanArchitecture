using EducationSystem.Application.Dtos.Request.Auth;
using EducationSystem.Application.Dtos.Response.Auth;

namespace EducationSystem.Application.Abstarctions.Identity;

public interface IAuthService
{
    Task<AuthResponse?> RegisterAsync(RegisterRequestDto request, CancellationToken cancellationToken = default);

    Task<AuthResponse> LoginAsync(LoginRequestDto request, CancellationToken cancellationToken = default);

    //Task<bool> LogoutAsync(CancellationToken cancellationToken = default);

    //Task<>

    //Task<string> AddRoleAsync(AddRoleModel model);
}