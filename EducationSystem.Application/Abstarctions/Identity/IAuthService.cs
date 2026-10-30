using EducationSystem.Application.Abstarctions.HandlingError;
using EducationSystem.Application.Dtos.Auth;

namespace EducationSystem.Application.Abstarctions.Identity;

public interface IAuthService
{
    Task<Result<RegisterResponse?>> RegisterAsync(RegisterRequest request, CancellationToken cancellationToken = default);

    Task<Result> ConfirmEmailAsync(ConfirmEmailRequest request, CancellationToken cancellationToken = default);

    Task<Result<AuthResponse?>> GetTokenAsync(string Email, string Password, CancellationToken cancellationToken = default);

    Task<Result<string>> ChangePasswordAsync(string userId, ChangePasswordRequest request);

    Task<Result<string>> ForgotPasswordAsync(ForgotPasswordRequest request);

    Task<Result<bool>> ResetPasswordAsync(ResetPasswordRequest request);

    Task<string> ResendConfirmationEmailAsync(ResendConfirmationEmailRequest request);
}