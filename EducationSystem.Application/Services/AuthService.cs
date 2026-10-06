using EducationSystem.Application.Abstarctions.HandlingError;
using EducationSystem.Application.Abstarctions.Identity;
using EducationSystem.Application.Abstarctions.Services;
using EducationSystem.Application.Abstractions.HandlingError.Errors;
using EducationSystem.Application.Dtos;
using EducationSystem.Domain.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

//using NETCore.MailKit.Core;
using System.Text;

namespace EducationSystem.Application.Services;

public class AuthService(UserManager<ApplicationUser> userManager,
                            SignInManager<ApplicationUser> signInManager,
                            RoleManager<ApplicationRole> roleManager,
                            IJwtProvider jwtProvider,
                            IEmailService MailKitEmailService,
                            IHttpContextAccessor httpContextAccessor,
                            ILogger<IAuthService> logger,
                            IConfiguration configuration) : IAuthService
{
    #region Fields

    private readonly UserManager<ApplicationUser> _userManager = userManager;
    private readonly SignInManager<ApplicationUser> _signInManager = signInManager;
    private readonly RoleManager<ApplicationRole> _roleManager = roleManager;
    private readonly IJwtProvider _jwtProvider = jwtProvider;

    private readonly IEmailService _emailService = MailKitEmailService;

    private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;
    private readonly ILogger _logger = logger;
    private readonly IConfiguration _configuration = configuration;

    #endregion Fields

    // 1. REGISTER
    public async Task<Result<AuthResponse?>> RegisterAsync(
        RegisterRequest request,
        CancellationToken cancellationToken = default)
    {
        // --------------------------------------------------------
        // 1.1 Check whether the email already exists
        // --------------------------------------------------------

        var existingUser = await _userManager.FindByEmailAsync(request.Email);

        if (existingUser is not null)
        {
            return Result.Failure<AuthResponse?>(
                UserErrors.DuplicateEmail);
        }

        // --------------------------------------------------------
        // 1.2 Determine the user's role
        // --------------------------------------------------------
        // If the client does not send a role,
        // Student will be assigned by default.

        var role = string.IsNullOrWhiteSpace(request.Role)
            ? "Student"
            : request.Role.Trim();

        // --------------------------------------------------------
        // 1.3 Validate that the requested role exists
        // --------------------------------------------------------

        if (!await _roleManager.RoleExistsAsync(role))
        {
            return Result.Failure<AuthResponse?>(
                new Error(
                    "Role.NotFound",
                    $"Role '{role}' does not exist."));
        }

        // --------------------------------------------------------
        // 1.4 Create the ApplicationUser object
        // --------------------------------------------------------
        // EmailConfirmed MUST remain false until the user
        // successfully confirms the email.

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

            // This is your custom Role column.
            // Identity authorization should still use UserRoles.
            Role = role,

            CreatedAt = DateTime.UtcNow
        };

        // --------------------------------------------------------
        // 1.5 Create the user
        // --------------------------------------------------------
        // CreateAsync will hash the password using Identity's
        // configured PasswordHasher.

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

                "PasswordTooShort" =>
                    UserErrors.WeakPassword,

                "PasswordRequiresLower" =>
                    UserErrors.WeakPassword,

                "PasswordRequiresUpper" =>
                    UserErrors.WeakPassword,

                "PasswordRequiresDigit" =>
                    UserErrors.WeakPassword,

                "PasswordRequiresNonAlphanumeric" =>
                    UserErrors.WeakPassword,

                _ =>
                    new Error(
                        "User.CreationFailed",
                        firstError.Description)
            };

            return Result.Failure<AuthResponse?>(error);
        }

        // --------------------------------------------------------
        // 1.6 Assign the Identity role
        // --------------------------------------------------------
        // This creates the relation in AspNetUserRoles.

        var roleResult = await _userManager.AddToRoleAsync(
            user,
            role);

        if (!roleResult.Succeeded)
        {
            // If role assignment fails, remove the user
            // because registration is not complete.

            await _userManager.DeleteAsync(user);

            var roleError = roleResult.Errors.FirstOrDefault();

            return Result.Failure<AuthResponse?>(
                new Error(
                    "User.RoleAssignmentFailed",
                    roleError?.Description
                        ?? $"Failed to assign role '{role}'."));
        }

        // --------------------------------------------------------
        // 1.7 Generate email confirmation token
        // --------------------------------------------------------
        // IMPORTANT:
        // Generate the token AFTER creating the user and
        // successfully assigning the role.

        var confirmationToken =
            await _userManager.GenerateEmailConfirmationTokenAsync(user);

        // --------------------------------------------------------
        // 1.8 Encode the confirmation token
        // --------------------------------------------------------
        // Identity's original token may contain characters that
        // are unsafe inside URLs.
        //
        // Base64UrlEncode converts the token into a URL-safe value.

        var encodedConfirmationToken =
            WebEncoders.Base64UrlEncode(
                Encoding.UTF8.GetBytes(confirmationToken));

        // --------------------------------------------------------
        // 1.9 Build the confirmation link
        // --------------------------------------------------------
        // Replace this URL with your real Angular application URL.

        var confirmationLink =
            $"https://your-frontend.com/confirm-email" +
            $"?userId={Uri.EscapeDataString(user.Id.ToString())}" +
            $"&token={Uri.EscapeDataString(encodedConfirmationToken)}";

        // --------------------------------------------------------
        // 1.10 Log the token during development
        // --------------------------------------------------------
        // NEVER log confirmation tokens in production.
        // This is only useful while testing.

        _logger.LogInformation(
            "Email confirmation code generated for user {UserId}: {ConfirmationCode}",
            user.Id,
            encodedConfirmationToken);

        // --------------------------------------------------------
        // 1.11 Send the confirmation email
        // --------------------------------------------------------

        //await _emailService.SendAsync(
        //    user.Email!,
        //    "Confirm your email",
        //    $"""
        //    Hello {user.FirstName},

        //    Please confirm your email by clicking the following link:

        //    {confirmationLink}

        //    This confirmation link will expire according to
        //    your Identity token configuration.
        //    """);
        try
        {
            await _emailService.SendAsync(
                user.Email!,
                "Confirm your email",
                $"""
                        <h2>Welcome {user.FirstName}!</h2>

                        <p>Please confirm your email by clicking the button below:</p>

                        <p>
                            <a href="{confirmationLink}">
                                Confirm Email
                            </a>
                        </p>
                        """);
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Failed to send confirmation email to user {UserId}",
                user.Id);

            return Result.Failure<AuthResponse?>(
                new Error(
                    "Email.SendFailed",
                    "The account was created, but we could not send the confirmation email."));
        }

        // --------------------------------------------------------
        // 1.12 Get the user's Identity roles
        // --------------------------------------------------------

        var userRoles = await _userManager.GetRolesAsync(user);

        // --------------------------------------------------------
        // 1.13 IMPORTANT:
        // Do NOT generate a JWT here if email confirmation
        // is required before login.
        // --------------------------------------------------------
        //
        // Your Identity configuration contains:
        //
        // options.SignIn.RequireConfirmedEmail = true;
        //
        // Therefore the user should confirm the email first
        // and then login to receive a JWT.

        // --------------------------------------------------------
        // 1.14 Return registration response
        // --------------------------------------------------------
        // We are using AuthResponse because that is your
        // existing DTO.
        //
        // Token is empty because the user is not authenticated yet.

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

            Token: string.Empty,

            IsAuthenticated: false,

            ExpiresIn: DateTime.UtcNow,

            Role: userRoles.FirstOrDefault() ?? string.Empty
        );

        return Result.Success<AuthResponse?>(response);
    }

    // 2. LOGIN
    public async Task<Result<AuthResponse?>> GetTokenAsync(
        string email,
        string password,
        CancellationToken cancellationToken = default)
    {
        // --------------------------------------------------------
        // 2.1 Find the user by email
        // --------------------------------------------------------

        var user = await _userManager.FindByEmailAsync(email);

        if (user is null)
        {
            return Result.Failure<AuthResponse?>(
                UserErrors.InvalidCredentials!);
        }

        // --------------------------------------------------------
        // 2.2 Check whether the account is active
        // --------------------------------------------------------

        if (!user.IsActive)
        {
            return Result.Failure<AuthResponse?>(
                new Error(
                    "User.Inactive",
                    "Your account has been deactivated."));
        }

        // --------------------------------------------------------
        // 2.3 Check password + Identity sign-in rules
        // --------------------------------------------------------
        //
        // false = don't remember login
        // false = don't lock out on failure

        var signInResult =
            await _signInManager.PasswordSignInAsync(
                user,
                password,
                isPersistent: false,
                lockoutOnFailure: false);

        // --------------------------------------------------------
        // 2.4 Password and Identity checks succeeded
        // --------------------------------------------------------

        if (signInResult.Succeeded)
        {
            // --------------------------------------------
            // 2.4.1 Get user's roles
            // --------------------------------------------

            var roles = await _userManager.GetRolesAsync(user);

            // --------------------------------------------
            // 2.4.2 Generate JWT
            // --------------------------------------------

            var (token, expiresIn) =
                _jwtProvider.GenerateToken(user, roles);

            // --------------------------------------------
            // 2.4.3 Build response
            // --------------------------------------------

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

                Role: roles.FirstOrDefault() ?? string.Empty
            );

            return Result.Success<AuthResponse?>(response);
        }

        // --------------------------------------------------------
        // 2.5 Email has not been confirmed
        // --------------------------------------------------------

        if (signInResult.IsNotAllowed)
        {
            return Result.Failure<AuthResponse?>(
                UserErrors.EmailNotConfirmed);
        }

        // --------------------------------------------------------
        // 2.6 Account is locked out
        // --------------------------------------------------------

        if (signInResult.IsLockedOut)
        {
            return Result.Failure<AuthResponse?>(
                new Error(
                    "User.LockedOut",
                    "Your account is temporarily locked."));
        }

        // --------------------------------------------------------
        // 2.7 Invalid credentials
        // --------------------------------------------------------

        return Result.Failure<AuthResponse?>(
            UserErrors.InvalidCredentials!);
    }

    // 3. FORGOT PASSWORD
    public async Task<Result<string>> ForgotPasswordAsync(ForgotPasswordRequest request)
    {
        // --------------------------------------------------------
        // 3.1 Find the user
        // --------------------------------------------------------

        var user =
            await _userManager.FindByEmailAsync(request.Email);

        // --------------------------------------------------------
        // 3.2 Do NOT reveal whether the email exists
        // --------------------------------------------------------
        // This prevents user enumeration attacks.

        if (user is null)
        {
            return Result.Success(
                "If this email is registered, a reset link has been sent.");
        }

        // --------------------------------------------------------
        // 3.3 Generate password reset token
        // --------------------------------------------------------

        var resetToken =
            await _userManager.GeneratePasswordResetTokenAsync(user);

        Console.WriteLine($"Raw token      : {resetToken}");

        // 4. Encode token so it can safely travel inside a URL
        var encodedToken = WebEncoders.Base64UrlEncode(
            Encoding.UTF8.GetBytes(resetToken));

        // 5. Create reset password URL
        var resetUrl =
            $"https://localhost:4200/reset-password?email={Uri.EscapeDataString(user.Email!)}&token={Uri.EscapeDataString(encodedToken)}";

        // 6. Send email

        try
        {
            await _emailService.SendAsync(
                user.Email!,
                "Reset Your Password",
                $"""
                        <h2>Reset Your Password</h2>

                        <p>Click the link below to reset your password:</p>

                        <a href="{resetUrl}">Reset Password</a>
                        """);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"EMAIL ERROR: {ex}");
            throw;
        }
        // --------------------------------------------------------
        // 3.7 Return generic response
        // --------------------------------------------------------

        return Result.Success(
            "If this email is registered, a reset link has been sent.");
    }

    // 4. RESET PASSWORD
    public async Task<Result<bool>> ResetPasswordAsync(ResetPasswordRequest request)
    {
        // 4.1 Find the user
        var user =
            await _userManager.FindByEmailAsync(request.Email);

        if (user is null)
        {
            _logger.LogWarning("ResetPassword: no user found for email {Email}", request.Email);

            return Result.Failure<bool>(
                UserErrors.InvalidResetToken);
        }
        else

            if (string.IsNullOrWhiteSpace(request.Token))
                return Result.Failure<bool>(UserErrors.InvalidResetToken);

        // 4.2 Decode the token received from the frontend
        string resetToken = request.Token.Trim(); ;

        return Result.Failure<bool>(
            UserErrors.InvalidResetToken);

        // 4.3 Ask Identity to validate the token
        // and reset the password
        var result =
            await _userManager.ResetPasswordAsync(
                user,
                resetToken,
                request.NewPassword);

        // 4.4 Handle Identity errors
        if (!result.Succeeded)
        {
            var firstError = result.Errors.First();

            var error = firstError.Code switch
            {
                "InvalidToken" =>
                    UserErrors.InvalidResetToken,

                "PasswordTooShort" =>
                    UserErrors.WeakPassword,

                "PasswordRequiresDigit" =>
                    UserErrors.WeakPassword,

                "PasswordRequiresUpper" =>
                    UserErrors.WeakPassword,

                "PasswordRequiresLower" =>
                    UserErrors.WeakPassword,

                "PasswordRequiresNonAlphanumeric" =>
                    UserErrors.WeakPassword,

                _ =>
                    new Error(
                        "User.ResetPasswordFailed",
                        firstError.Description)
            };

            return Result.Failure<bool>(error);
        }

        return Result.Success(true);
    }

    // 5. CHANGE PASSWORD
    public async Task<Result<string>> ChangePasswordAsync(string userId, ChangePasswordRequest request)
    {
        // --------------------------------------------------------
        // 5.1 Get the current HTTP context
        // --------------------------------------------------------

        var httpContext = _httpContextAccessor.HttpContext;

        if (httpContext is null)
        {
            return Result.Failure<string>(
                new Error(
                    "Auth.HttpContextUnavailable",
                    "HTTP context is unavailable."));
        }

        // 5.2 Get the authenticated user principal from JWT
        // --------------------------------------------------------

        var principal = httpContext.User;

        if (principal?.Identity?.IsAuthenticated != true)
        {
            return Result.Failure<string>(
                new Error(
                    "Auth.Unauthorized",
                    "User is not authenticated."));
        }

        // 5.3 Find the ApplicationUser
        // --------------------------------------------------------

        var user = await _userManager.GetUserAsync(principal);

        // 5.4 Check whether the account is active
        // --------------------------------------------------------

        if (!user!.IsActive)
        {
            return Result.Failure<string>(
                new Error(
                    "User.Inactive",
                    "Your account has been deactivated."));
        }

        // --------------------------------------------------------
        // 5.5 Change the password
        // --------------------------------------------------------

        var result =
            await _userManager.ChangePasswordAsync(user,
                                                    request.CurrentPassword,
                                                    request.NewPassword);

        // --------------------------------------------------------
        // 5.6 Handle Identity validation errors
        // --------------------------------------------------------

        if (!result.Succeeded)
        {
            var firstError =
                result.Errors.First();

            var error = firstError.Code switch
            {
                "PasswordMismatch" =>
                    new Error(
                        "Password.InvalidCurrentPassword",
                        "The current password is incorrect."),

                //"PasswordTooShort" =>
                //    new Error(
                //        "Password.TooShort",
                //        firstError.Description),

                //"PasswordRequiresDigit" =>
                //    new Error(
                //        "Password.RequiresDigit",
                //        firstError.Description),

                //"PasswordRequiresLower" =>
                //    new Error(
                //        "Password.RequiresLower",
                //        firstError.Description),

                //"PasswordRequiresUpper" =>
                //    new Error(
                //        "Password.RequiresUpper",
                //        firstError.Description),

                //"PasswordRequiresNonAlphanumeric" =>
                //    new Error(
                //        "Password.RequiresSpecialCharacter",
                //        firstError.Description),

                _ =>
                    new Error(
                        "Password.ChangeFailed",
                        firstError.Description)
            };

            return Result.Failure<string>(error);
        }

        // --------------------------------------------------------
        // 5.7 Password changed successfully
        // --------------------------------------------------------

        return Result.Success(
            "Password changed successfully.");
    }

    // 6. CONFIRM EMAIL
    public async Task<Result> ConfirmEmailAsync(ConfirmEmailRequest request)
    {
        // --------------------------------------------------------
        // 6.1 Find the user
        // --------------------------------------------------------

        var user =
            await _userManager.FindByIdAsync(request.UserId);

        if (user is null)
        {
            return Result.Failure(
                new Error(
                    "User.NotFound",
                    "User not found."));
        }

        // --------------------------------------------------------
        // 6.2 Check whether the email is already confirmed
        // --------------------------------------------------------

        if (user.EmailConfirmed)
        {
            return Result.Failure(
                new Error(
                    "User.EmailAlreadyConfirmed",
                    "Email is already confirmed."));
        }

        // --------------------------------------------------------
        // 6.3 Decode the Base64Url token
        // --------------------------------------------------------
        //
        // During registration:
        //
        // Original Identity Token
        //       ↓
        // UTF8 bytes
        //       ↓
        // Base64UrlEncode
        //
        // Here we perform the reverse operation.

        string token;

        try
        {
            token = Encoding.UTF8.GetString(
                WebEncoders.Base64UrlDecode(
                    request.Token));
        }
        catch (FormatException)
        {
            return Result.Failure(
                new Error(
                    "User.InvalidToken",
                    "Invalid confirmation token."));
        }

        // --------------------------------------------------------
        // 6.4 Ask ASP.NET Identity to validate the token
        // --------------------------------------------------------
        //
        // IMPORTANT:
        // Use `token`, NOT `request.Token`.
        //
        // request.Token = encoded token
        // token         = original Identity token

        var result =
            await _userManager.ConfirmEmailAsync(
                user,
                token);

        // --------------------------------------------------------
        // 6.5 Check Identity result
        // --------------------------------------------------------

        if (!result.Succeeded)
        {
            var errors =
                string.Join(
                    ", ",
                    result.Errors.Select(e => e.Description));

            return Result.Failure(
                new Error(
                    "User.ConfirmEmailFailed",
                    errors));
        }

        // --------------------------------------------------------
        // 6.6 Email confirmation succeeded
        // --------------------------------------------------------
        //
        // Identity updates:
        //
        // user.EmailConfirmed
        //
        // from:
        // false
        //
        // to:
        // true

        return Result.Success();
    }

    //7. RESEND CONFIRMATION EMAIL
    public async Task<string> ResendConfirmationEmailAsync(ResendConfirmationEmailRequest request)
    {
        // 1. find the user by email
        var user = await _userManager.FindByEmailAsync(request.Email);

        // 2. Don't reveal whether the email exists
        // This prevents user/account enumeration.
        if (user is null)
            return "If this email exists, a confirmation email has been sent.";

        // 3. Check if the email is already confirmed
        if (user.EmailConfirmed)
            return "Email is already confirmed.";

        // 4. Generate a new email confirmation token
        var confirmationToken =
            await _userManager.GenerateEmailConfirmationTokenAsync(user);

        // 5. Encode the token so it can safely be placed inside a URL
        var encodedToken = WebEncoders.Base64UrlEncode(
            Encoding.UTF8.GetBytes(confirmationToken));

        // 6. Get frontend confirmation URL from configuration
        var confirmationUrl = _configuration["Frontend:ConfirmationUrl"];

        if (string.IsNullOrWhiteSpace(confirmationUrl))
        {
            throw new InvalidOperationException(
                "Frontend:ConfirmationUrl is not configured.");
        }

        // 7. Build confirmation link
        var confirmationLink =
            $"{confirmationUrl}?userId={user.Id}&token={encodedToken}";

        // 8. Send confirmation email
        await _emailService.SendAsync(
            user.Email!,
            "Confirm your email",
            $"""
            Hello {user.UserName},

            Please confirm your email by clicking the following link:

            {confirmationLink}

            If you did not create this account, you can ignore this email.
            """);

        // 9. Return success message
        return "If this email exists, a confirmation email has been sent.";
    }
}