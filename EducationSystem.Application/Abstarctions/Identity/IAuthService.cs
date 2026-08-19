using EducationSystem.Application.Dtos.Auth;

namespace EducationSystem.Application.Abstarctions.Identity;

public interface IAuthService
{
    Task<AuthModel> RegisterUserAsync(RegisterRequestDto request);

    Task<AuthModel> LoginUserAsync(LoginRequestDto request);

    Task<string> AddRoleAsync(AddRoleModel model);
}