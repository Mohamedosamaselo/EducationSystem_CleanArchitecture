using EducationSystem.Application.Dtos.Auth;

namespace EducationSystem.Application.Abstarctions.Identity;

public interface IAuthService
{
    Task<AuthResponse?> RegisterUserAsync(RegisterRequestDto request, CancellationToken cancellationToken = default);

    Task<AuthResponse> LoginUserAsync(LoginRequestDto request, CancellationToken cancellationToken = default);

    //Task<string> AddRoleAsync(AddRoleModel model);
}