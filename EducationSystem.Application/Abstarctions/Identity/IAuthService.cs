using EducationSystem.Application.Abstarctions.HandlingError;
using EducationSystem.Application.Dtos;

namespace EducationSystem.Application.Abstarctions.Identity;

public interface IAuthService
{
    Task<Result<AuthResponse?>> RegisterAsync(RegisterRequest request, CancellationToken cancellationToken = default);

    Task<Result> ConfirmEmailAsync(ConfirmEmailRequest request);

    Task<Result<AuthResponse?>> GetTokenAsync(string Email, string Password, CancellationToken cancellationToken = default);

    Task<Result<string>> ChangePasswordAsync(string userId, ChangePasswordRequest request);

    Task<Result<string>> ForgotPasswordAsync(ForgotPasswordRequest request, CancellationToken cancellationToken = default);

    Task<Result<bool>> ResetPasswordAsync(ResetPasswordRequest request, CancellationToken cancellationToken = default);

    Task<string> ResendConfirmationEmailAsync(ResendConfirmationEmailRequest request);
}