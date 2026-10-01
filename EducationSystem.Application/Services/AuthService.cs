using EducationSystem.Application.Abstarctions.HandlingError;
using EducationSystem.Application.Abstarctions.Identity;
using EducationSystem.Application.Abstractions.HandlingError.Errors;
using EducationSystem.Application.Dtos;
using EducationSystem.Domain.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using System.Security.Principal;

namespace EducationSystem.Application.Services;

public class AuthService(UserManager<ApplicationUser> userManager,
                         RoleManager<ApplicationRole> roleManager,
                            IJwtProvider jwtProvider,
                            Abstarctions.Identity.IEmailSender emailSender,
                            IHttpContextAccessor httpContextAccessor) : IAuthService
{
    private readonly UserManager<ApplicationUser> _userManager = userManager;
    private readonly RoleManager<ApplicationRole> _roleManager = roleManager;
    private readonly IJwtProvider _jwtProvider = jwtProvider;
    private readonly Abstarctions.Identity.IEmailSender _emailSender = emailSender;
    private readonly IHttpContextAccessor httpContextAccessor = httpContextAccessor;

    public async Task<Result<AuthResponse?>> RegisterAsync(
        RegisterRequest request,
        CancellationToken cancellationToken = default)
    {
        // 1. Check if the email already exists
        var existingUser = await _userManager.FindByEmailAsync(request.Email);

        if (existingUser is not null)
        {
            return Result.Failure<AuthResponse?>(
                UserErrors.DuplicateEmail);
        }

        // 2. Determine the role,   If no role is provided, assign Student by default

        var role = string.IsNullOrWhiteSpace(request.Role)
            ? "Student" : request.Role.Trim();

        // 3. Validate that the requested role exists
        if (!await _roleManager.RoleExistsAsync(role))
        {
            return Result.Failure<AuthResponse?>(
                new Error(
                    "Role.NotFound",
                    $"Role '{role}' does not exist."));
        }

        // 4. Create user
        var user = new ApplicationUser
        {
            Id = Guid.NewGuid(),

            UserName = request.Email,
            Email = request.Email,

            FirstName = request.FirstName,
            LastName = request.LastName,
            Address = request.Address,
            DateOfBirth = request.DateOfBirth,

            OrganisationId = request.OrganisationId,
            SchoolId = request.SchoolId,
            GradeId = request.GradeId,

            EmailConfirmed = false,
            IsActive = true,

            // Your custom Role column in Users table
            Role = role,

            CreatedAt = DateTime.UtcNow
        };

        // 5. Create user + hash password
        var createResult = await _userManager.CreateAsync(
            user,
            request.Password);

        if (!createResult.Succeeded)
        {
            var firstError = createResult.Errors.First();

            var error = firstError.Code switch
            {
                "DuplicateEmail" =>
                    UserErrors.DuplicateEmail,

                "DuplicateUserName" =>
                    UserErrors.DuplicateUserName,

                "PasswordRequiresLower" =>
                    UserErrors.WeakPassword,

                _ =>
                    new Error(
                        "User.CreationFailed",
                        firstError.Description)
            };

            return Result.Failure<AuthResponse?>(error);
        }

        // 6. Assign role through ASP.NET Identity
        var roleResult = await _userManager.AddToRoleAsync(
            user,
            role);

        if (!roleResult.Succeeded)
        {
            // Rollback user if role assignment fails
            await _userManager.DeleteAsync(user);

            return Result.Failure<AuthResponse?>(
                new Error(
                    "User.RoleAssignmentFailed",
                    $"Failed to assign role '{role}'."));
        }

        // 7. Get user's role from Identity
        var userRole = await _userManager.GetRolesAsync(user);

        // 8. Generate JWT
        var (token, expiresIn) =
            _jwtProvider.GenerateToken(user, userRole);

        // 9. Create response
        var response = new AuthResponse(
            Id: user.Id,
            Email: user.Email!,
            Username: user.UserName!,
            FirstName: user.FirstName,
            LastName: user.LastName,
            Address: user.Address,
            DateOfBirth: user.DateOfBirth,
            OrganisationId: user.OrganisationId,
            SchoolId: user.SchoolId,
            GradeId: user.GradeId,
            Token: token,
            IsAuthenticated: true,
            ExpiresIn: DateTime.UtcNow.AddMinutes(expiresIn),
            Role: userRole.FirstOrDefault() ?? string.Empty
        );

        return Result.Success<AuthResponse?>(response);
    }

    // login
    public async Task<Result<AuthResponse?>> GetTokenAsync(string Email, string password, CancellationToken cancellationToken = default)
    {
        // 1. Find User
        var user = await _userManager.FindByEmailAsync(Email);

        if (user is null)
            return Result.Failure<AuthResponse?>(UserErrors.InvalidCredentials!);

        // 2. Check password
        var isPasswordValid =
        await _userManager.CheckPasswordAsync(user, password);

        if (!isPasswordValid)
            return Result.Failure<AuthResponse?>(UserErrors.InvalidCredentials!);

        // 3. Get User Roles

        var roles = await _userManager.GetRolesAsync(user);

        // 4. Generate Token
        var (token, expiresIn) = _jwtProvider.GenerateToken(user, roles);

        // 5. Return AuthReponse
        var response = new AuthResponse(
                                        Id: user.Id,
                                        Email: user.Email!,
                                        Username: user.UserName!,
                                        FirstName: user.FirstName,
                                        LastName: user.LastName,
                                        Address: user.Address,
                                        DateOfBirth: user.DateOfBirth,
                                        OrganisationId: user.OrganisationId,
                                        SchoolId: user.SchoolId,
                                        GradeId: user.GradeId,
                                        Token: token,
                                        IsAuthenticated: true,
                                        ExpiresIn: DateTime.UtcNow.AddMinutes(expiresIn),
                                        Role: roles.ToList().FirstOrDefault() ?? string.Empty
                                        );

        return Result.Success(response)!;
    }

    public async Task<Result<string>> ForgotPasswordAsync(ForgotPasswordRequest request, CancellationToken cancellationToken = default)
    {
        // search for user
        var user = await _userManager.FindByEmailAsync(request.Email);
        // 2. If the user doesn't exist, return SUCCESS with a generic message.
        //    WHY? If we returned "user not found", attackers could use this endpoint
        //    to test which emails are registered (a "user enumeration" attack).
        if (user is null)
            return Result.Success("If this email is registered, a reset link has been sent.");

        // 3. Ask Identity to generate a one-time, expiring reset token for this user
        var token = await _userManager.GeneratePasswordResetTokenAsync(user);

        // 4. Build the link for the frontend.
        //    IMPORTANT: tokens contain URL-unsafe characters (+, /, =),
        //    so they MUST be URL-encoded inside a link.
        var encodedToken = Uri.EscapeDataString(token);
        var link = $"https://your-frontend.com/reset-password" +
                   $"?email={Uri.EscapeDataString(request.Email)}" +
                   $"&token={encodedToken}";

        // 5. "Send" it (currently just logs — see EmailSender)
        await _emailSender.SendAsync(
            request.Email,
            "Reset Your Password",
            $"Click the link to reset your password:\n{link}",
            cancellationToken);

        // ⚠️ DEV ONLY: uncomment the next line while testing in Postman so you
        // get the RAW token back. DELETE this line before production!
        // return Result.Success(token);

        return Result.Success("If this email is registered, a reset link has been sent.");
    }

    public async Task<Result<bool>> ResetPasswordAsync(ResetPasswordRequest request,
                                                 CancellationToken cancellationToken = default)
    {
        // 1. Find the user
        var user = await _userManager.FindByEmailAsync(request.Email);
        if (user is null)
            return Result.Failure<bool>(UserErrors.InvalidResetToken);

        // 2. ResetPasswordAsync = "validate token + set new password" in one call.
        //    It also re-hashes the password properly for you.
        var result = await _userManager.ResetPasswordAsync(user,
                                                           request.Token,
                                                           request.NewPassword);

        if (!result.Succeeded)
        {
            var firstError = result.Errors.First();
            var error = firstError.Code switch
            {
                "InvalidToken" => UserErrors.InvalidResetToken,
                "PasswordTooShort" => UserErrors.WeakPassword,
                "PasswordRequiresDigit" => UserErrors.WeakPassword,
                "PasswordRequiresUpper" => UserErrors.WeakPassword,
                "PasswordRequiresLower" => UserErrors.WeakPassword,
                _ => new Error("User.ResetPasswordFailed", firstError.Description)
            };
            return Result.Failure<bool>(error);
        }

        return Result.Success(true);
    }

    public async Task<Result<string>> ChangePasswordAsync(ChangePasswordRequest request, CancellationToken cancellationToken = default)
    {
        //1-  Get the current user from the JWT.
        //2-  Check that the user exists.
        var user = await _userManager.GetUserAsync(httpContextAccessor.HttpContext?.User!);
        if (user is null)
        {
            return Result.Failure<string>(
                new Error(
                    "User.NotFound",
                    "User was not found."));
        }

        //3-  Check that the account is active.
        if (!user.IsActive)
        {
            return Result.Failure<string>(
                new Error(
                    "User.Inactive",
                    "Your account has been deactivated."));
        }

        //4-  Call _userManager.ChangePasswordAsync().
        var result = await _userManager.ChangePasswordAsync(
                            user,
                            request.CurrentPassword,
                            request.NewPassword);

        //5-  Return a success message or the Identity error.
        if (!result.Succeeded)
        {
            var firstError = result.Errors.First();

            var error = firstError.Code switch
            {
                "PasswordMismatch" =>
                    new Error(
                        "Password.InvalidCurrentPassword",
                        "The current password is incorrect."),

                "PasswordTooShort" =>
                    new Error(
                        "Password.TooShort",
                        firstError.Description),

                "PasswordRequiresDigit" =>
                    new Error(
                        "Password.RequiresDigit",
                        firstError.Description),

                "PasswordRequiresLower" =>
                    new Error(
                        "Password.RequiresLower",
                        firstError.Description),

                "PasswordRequiresUpper" =>
                    new Error(
                        "Password.RequiresUpper",
                        firstError.Description),

                "PasswordRequiresNonAlphanumeric" =>
                    new Error(
                        "Password.RequiresSpecialCharacter",
                        firstError.Description),

                _ =>
                    new Error(
                        "Password.ChangeFailed",
                        firstError.Description)
            };

            return Result.Failure<string>(error);
        }

        // 5. Password changed successfully
        return Result.Success(
            "Password changed successfully.");
    }
}