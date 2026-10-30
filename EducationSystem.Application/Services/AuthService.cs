using EducationSystem.Application.Abstarctions.HandlingError;
using EducationSystem.Application.Abstarctions.Identity;
using EducationSystem.Application.Abstarctions.Services;
using EducationSystem.Application.Abstarctions.UnitOfWork;
using EducationSystem.Application.Abstractions.HandlingError.Errors;
using EducationSystem.Application.Dtos.Auth;
using EducationSystem.Domain.Entities;
using Microsoft.AspNetCore.Hosting;

//using EducationSystem.Infrastructure.Persistence;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.VisualBasic;
using System.Data;
using System.Net;
using System.Net.Mail;
using System.Security.Cryptography;

//using NETCore.MailKit.Core;
using System.Text;
using System.Text.RegularExpressions;
using Error = EducationSystem.Application.Abstarctions.HandlingError.Error;

namespace EducationSystem.Application.Services;

public class AuthService(UserManager<ApplicationUser> userManager,
                            SignInManager<ApplicationUser> signInManager,
                            //RoleManager<ApplicationRole> roleManager,
                            IWebHostEnvironment environment,
                            IUnitOfWork unitOfWork,
                            IJwtProvider jwtProvider,
                            IEmailService MailKitEmailService,
                            IHttpContextAccessor httpContextAccessor,
                            ILogger<IAuthService> logger,
                            IPasswordHasher<ApplicationUser> passwordHasher,
                            IConfiguration Configuration
                            ) : IAuthService
{
    #region Fields

    private static readonly string[] AllowedPublicRoles = { "Student", "SchoolAdmin", "OrganisationAdmin", "Teacher" };

    private static readonly TimeSpan ConfirmationTokenLifetime = TimeSpan.FromHours(24);

    private readonly UserManager<ApplicationUser> _userManager = userManager;
    private readonly SignInManager<ApplicationUser> _signInManager = signInManager;
    private readonly IWebHostEnvironment _environment = environment;

    //private readonly RoleManager<ApplicationRole> _roleManager = roleManager;
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    private readonly IJwtProvider _jwtProvider = jwtProvider;

    private readonly IEmailService _emailService = MailKitEmailService;

    private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;

    private readonly ILogger _logger = logger;

    private readonly IPasswordHasher<ApplicationUser> _passwordHasher = passwordHasher;

    private readonly IConfiguration _configuration = Configuration;

    #endregion Fields

    public async Task<Result<RegisterResponse?>> RegisterAsync(
        RegisterRequest request,
        CancellationToken cancellationToken = default)
    {
        // --------------------------------------------------------
        // 1.1 Normalize input
        // --------------------------------------------------------
        var firstName = request.FirstName?.Trim() ?? "";
        var lastName = request.LastName?.Trim() ?? "";
        var email = request.Email?.Trim().ToLowerInvariant() ?? "";
        var userName = request.UserName?.Trim() ?? "";
        var address = request.Address?.Trim() ?? "";
        var password = request.Password ?? "";
        var normalizedUserName = userName.ToUpperInvariant();

        // --------------------------------------------------------
        // 1.2 Validate the data (replaces UserManager's validators)
        // --------------------------------------------------------
        if (firstName.Length is 0 or > 50 || lastName.Length is 0 or > 50)
            return Result.Failure<RegisterResponse?>(
                new Error("User.InvalidName", "First and last name are required (max 50 characters each)."));

        if (!IsValidEmail(email))
            return Result.Failure<RegisterResponse?>(
                new Error("User.InvalidEmail", "Email is not valid."));

        if (!Regex.IsMatch(userName, @"^[A-Za-z0-9_.]{3,30}$"))
            return Result.Failure<RegisterResponse?>(
                new Error("User.InvalidUserName", "Username must be 3-30 characters: letters, digits, '_' or '.'."));

        if (request.BirthDate is { } dob &&
            (dob.Date > DateTime.UtcNow.Date || dob.Date < DateTime.UtcNow.AddYears(-120).Date))
            return Result.Failure<RegisterResponse?>(
                new Error("User.InvalidDateOfBirth", "Date of birth is not valid."));

        if (!IsStrongPassword(password))
            return Result.Failure<RegisterResponse?>(UserErrors.WeakPassword);

        // --------------------------------------------------------
        // 1.3 Check whether the email / username already exist
        // --------------------------------------------------------
        //if (await _db.Users.AnyAsync(u => u.NormalizedEmail == normalizedEmail, cancellationToken))
        //    return Result.Failure<RegisterResponse?>(UserErrors.DuplicateEmail);

        //if (await _db.Users.AnyAsync(u => u.NormalizedUserName == normalizedUserName, cancellationToken))
        //    return Result.Failure<RegisterResponse?>(UserErrors.DuplicateUserName);

        // --------------------------------------------------------
        // 1.4 Determine the user's role (default: Student) + whitelist
        // --------------------------------------------------------
        var userRole = string.IsNullOrWhiteSpace(request.Role)
            ? "Student"
            : request.Role.Trim();

        if (!AllowedPublicRoles.Contains(userRole, StringComparer.OrdinalIgnoreCase))
            return Result.Failure<RegisterResponse?>(
                new Error("Role.NotAllowed", "This role cannot be self-registered."));

        // --------------------------------------------------------
        // 1.5 Make sure the role exists in AspNetRoles
        // --------------------------------------------------------
        var normalizedRole = userRole.ToUpperInvariant();
        var role = await _unitOfWork.RoleRepository.FirstOrDefaultAsync(r => r.Name == normalizedRole, cancellationToken);
        //var role = await unitOfWork.Roles.FirstOrDefaultAsync(r => r.Name == userRole, cancellationToken);

        if (role is null)
            return Result.Failure<RegisterResponse?>(
                new Error("Role.NotFound", $"Role '{userRole}' does not exist."));

        // --------------------------------------------------------
        // 1.6 Make sure the optional foreign keys really exist
        // --------------------------------------------------------
        if (request.OrganisationId is { } orgId &&
            !await _unitOfWork.OrganisationRepository.AnyAsync(o => o.Id == orgId, cancellationToken))
            return Result.Failure<RegisterResponse?>(
                new Error("Organisation.NotFound", "Organisation does not exist."));

        if (request.SchoolId is { } schoolId &&
            !await _unitOfWork.SchoolRepository.AnyAsync(s => s.Id == schoolId, cancellationToken))
            return Result.Failure<RegisterResponse?>(
                new Error("School.NotFound", "School does not exist."));

        if (request.GradeId is { } gradeId &&
            !await _unitOfWork.GradeRepository.AnyAsync(g => g.Id == gradeId, cancellationToken))
            return Result.Failure<RegisterResponse?>(
                new Error("Grade.NotFound", "Grade does not exist."));

        // --------------------------------------------------------
        // 1.7 Create the ApplicationUser object
        // --------------------------------------------------------
        // These are the fields UserManager.CreateAsync would normally fill in for you.
        var user = new ApplicationUser
        {
            Id = Guid.NewGuid(),
            FirstName = firstName,
            LastName = lastName,
            Address = address,
            DateOfBirth = request.BirthDate?.Date,
            UserName = userName,
            Email = email,
            EmailConfirmed = false,                                //   MUST stay false until the user confirms the email
            SecurityStamp = Guid.NewGuid().ToString("N"),   //   invalidates old tokens when it changes
            ConcurrencyStamp = Guid.NewGuid().ToString(),
            SchoolId = request.SchoolId,
            GradeId = request.GradeId,
            RoleId = role.Id,
            CreatedAt = DateTime.UtcNow
        };

        // 1.8 Hash the password (replaces CreateAsync's hashing)
        // --------------------------------------------------------
        // Same PBKDF2 hasher Identity uses, so hashes stay compatible if you ever adopt UserManager.
        user.PasswordHash = _passwordHasher.HashPassword(user, password);

        // --------------------------------------------------------
        // 1.9 Generate the email confirmation token
        // --------------------------------------------------------
        // Raw token goes in the email; only its SHA-256 hash is stored.
        var rawToken = WebEncoders.Base64UrlEncode(RandomNumberGenerator.GetBytes(32));

        var confirmationToken = new EmailConfirmationToken
        {
            UserId = user.Id,
            TokenHash = HashToken(rawToken),
            ExpiresAt = DateTime.UtcNow.Add(ConfirmationTokenLifetime)
        };

        // --------------------------------------------------------
        // 1.10 Save everything together (one SaveChanges = one transaction)
        // --------------------------------------------------------
        await _unitOfWork.UserRepository.AddAsync(user, cancellationToken);
        await _unitOfWork.EmailConfirmationTokenRepository.AddAsync(confirmationToken, cancellationToken);

        //// Keep AspNetUserRoles consistent too (remove this line if you only rely on user.RoleId).
        //_db.UserRoles.Add(new IdentityUserRole<Guid> { UserId = user.Id, RoleId = role.Id });

        try
        {
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateException ex)
        {
            // Race condition: another request took the same email/username after our checks.
            // The UNIQUE indexes caught it.
            _logger.LogWarning(ex, "Registration conflict for email {Email}", email);
            return Result.Failure<RegisterResponse?>(
                new Error("User.Duplicate", "Email or username is already in use."));
        }

        // --------------------------------------------------------
        // 1.11 Build the confirmation link
        // --------------------------------------------------------
        // Base64Url + GUID are already URL-safe, so no extra escaping.
        var frontendUrl = _configuration["App:FrontendUrl"] ?? "https://your-frontend.com";
        var confirmationLink = $"{frontendUrl}/confirm-email?userId={user.Id}&token={rawToken}";

        // Never log the token itself.
        // _logger.LogInformation("Email confirmation token created for user {UserId}", user.Id);
        if (_environment.IsDevelopment())
        {
            _logger.LogInformation(
                "Email confirmation token for UserId {UserId}: {Token}",
                user.Id,
                rawToken);
        }
        else
        {
            _logger.LogInformation(
                "Email confirmation token generated for UserId {UserId}",
                user.Id);
        }
        // --------------------------------------------------------
        // 1.12 Send the confirmation email
        // --------------------------------------------------------
        try
        {
            await _emailService.SendAsync(
                user.Email!,
                "Confirm your email",
                $"""
                        <h2>Welcome {WebUtility.HtmlEncode(user.FirstName)}!</h2>
                        <p>Please confirm your email by clicking the link below:</p>
                        <p><a href="{confirmationLink}">Confirm Email</a></p>
                        <p>This link expires in {ConfirmationTokenLifetime.TotalHours:0} hours.</p>
                """);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send confirmation email to user {UserId}", user.Id);

            return Result.Failure<RegisterResponse?>(
                new Error(
                    "Email.SendFailed",
                    "The account was created, but we could not send the confirmation email."));
        }

        // --------------------------------------------------------
        // 1.13 Build the response (no JWT yet: user is not authenticated until login)
        // --------------------------------------------------------
        var response = new RegisterResponse(
            IsSuccess: true,
            Message: "Registration successful. Please check your email to confirm your account.",
            IsConfirmed: false,
            Token: null,
            IsAuthenticated: true,
            ExpiresIn: DateTime.UtcNow.Add(ConfirmationTokenLifetime),
            Role: role.Name ?? userRole
        );

        return Result.Success<RegisterResponse?>(response);
    }

    // 1. REGISTER
    //public async Task<Result<AuthResponse?>> RegisterAsync(RegisterRequest request,CancellationToken cancellationToken = default)
    //{
    //    // --------------------------------------------------------
    //    // 1.1 Check whether the email already exists
    //    // --------------------------------------------------------
    //    var existingUser = await _userManager.FindByEmailAsync(request.Email);
    //    if (existingUser is not null)
    //    {
    //        return Result.Failure<AuthResponse?>(UserErrors.DuplicateEmail);
    //    }
    //    // --------------------------------------------------------
    //    // 1.2 Determine the user's role
    //    // --------------------------------------------------------
    //    // If the client does not send a role,
    //    // Student will be assigned by default.
    //    var userRole = string.IsNullOrWhiteSpace(request.Role)
    //        ? "Student"
    //        : request.Role.Trim();
    //    //  only allow safe roles for public registration
    //    var allowedPublicRoles = new[] { "Student", "SchoolAdmin", "OrganisationAdmin", "Teacher" };
    //    if (!allowedPublicRoles.Contains(userRole, StringComparer.OrdinalIgnoreCase))
    //    {
    //        return Result.Failure<AuthResponse?>(new Error("Role.NotAllowed", "This role cannot be self-registered."));
    //    }
    //    // --------------------------------------------------------
    //    // 1.3 Validate that the requested role exists
    //    // --------------------------------------------------------
    //    var role = await _roleManager.FindByNameAsync(userRole);
    //    if (role is null)
    //    {
    //        return Result.Failure<AuthResponse?>(
    //            new Error(
    //                "Role.NotFound",
    //                $"Role '{userRole}' does not exist."));
    //    }
    //    // --------------------------------------------------------
    //    // 1.4 Create the ApplicationUser object
    //    // --------------------------------------------------------
    //    // EmailConfirmed MUST remain false until the user
    //    // successfully confirms the email.
    //    var user = new ApplicationUser
    //    {
    //        Id = Guid.NewGuid(),
    //        FirstName = request.FirstName,
    //        LastName = request.LastName,
    //        Address = request.Address,
    //        DateOfBirth = request.DateOfBirth,
    //        UserName = request.UserName,
    //        Email = request.Email,
    //        //PasswordHash = request.Password,
    //        EmailConfirmed = false,
    //        SchoolId = request.SchoolId,
    //        GradeId = request.GradeId,
    //        RoleId = role.Id,
    //        CreatedAt = DateTime.UtcNow
    //    };
    //    // --------------------------------------------------------
    //    // 1.5 Create the user
    //    // --------------------------------------------------------
    //    // CreateAsync will hash the password using Identity's
    //    // configured PasswordHasher.
    //    var createResult = await _userManager.CreateAsync(user, request.Password);
    //    if (!createResult.Succeeded)
    //    {
    //        var firstError = createResult.Errors.First();
    //        var error = firstError.Code switch
    //        {
    //            "DuplicateEmail" =>
    //                UserErrors.DuplicateEmail,
    //            "DuplicateUserName" =>
    //                UserErrors.DuplicateUserName,
    //            "PasswordTooShort" =>
    //                UserErrors.WeakPassword,
    //            "PasswordRequiresLower" =>
    //                UserErrors.WeakPassword,
    //            "PasswordRequiresUpper" =>
    //                UserErrors.WeakPassword,
    //            "PasswordRequiresDigit" =>
    //                UserErrors.WeakPassword,
    //            "PasswordRequiresNonAlphanumeric" =>
    //                UserErrors.WeakPassword,
    //            _ =>
    //                new Error(
    //                    "User.CreationFailed",
    //                    firstError.Description)
    //        };
    //        return Result.Failure<AuthResponse?>(error);
    //    }
    //    // --------------------------------------------------------
    //    // 1.7 Generate email confirmation token
    //    // successfully assigning the role.
    //    // IMPORTANT:
    //    // Generate the token AFTER creating the user and
    //    // --------------------------------------------------------
    //    var confirmationToken = await _userManager.GenerateEmailConfirmationTokenAsync(user);
    //    // --------------------------------------------------------
    //    // 1.8 Encode the confirmation token
    //    // --------------------------------------------------------
    //    var encodedConfirmationToken =
    //        WebEncoders.Base64UrlEncode(
    //            Encoding.UTF8.GetBytes(confirmationToken));
    //    // --------------------------------------------------------
    //    // 1.9 Build the confirmation link
    //    // Replace this URL with your real Angular application URL.
    //    // --------------------------------------------------------
    //    var confirmationLink =
    //        $"https://your-frontend.com/confirm-email" +
    //        $"?userId={Uri.EscapeDataString(user.Id.ToString())}" +
    //        $"&token={Uri.EscapeDataString(encodedConfirmationToken)}";
    //    // --------------------------------------------------------
    //    // 1.10 Log the token during development
    //    // --------------------------------------------------------
    //    // NEVER log confirmation tokens in production.
    //    // This is only useful while testing.
    //    _logger.LogInformation(
    //        "Email confirmation code generated for user {UserId}: {ConfirmationCode}",
    //        user.Id,
    //        encodedConfirmationToken);
    //    // --------------------------------------------------------
    //    // 1.11 Send the confirmation email
    //    // --------------------------------------------------------
    //    try
    //    {
    //        await _emailService.SendAsync(
    //            user.Email!,
    //            "Confirm your email",
    //            $"""
    //                    <h2>Welcome {user.FirstName}!</h2>
    //                    <p>Please confirm your email by clicking the button below:</p>
    //                    <p>
    //                        <a href="{confirmationLink}">
    //                            Confirm Email
    //                        </a>
    //                    </p>
    //                    """);
    //    }
    //    catch (Exception ex)
    //    {
    //        _logger.LogError(
    //            ex,
    //            "Failed to send confirmation email to user {UserId}",
    //            user.Id);
    //        return Result.Failure<AuthResponse?>(
    //            new Error(
    //                "Email.SendFailed",
    //                "The account was created, but we could not send the confirmation email."));
    //    }
    //    // --------------------------------------------------------
    //    // 1.12 Get the user's Identity roles
    //    // --------------------------------------------------------
    //    var userRoles = await _userManager.GetRolesAsync(user);
    //    var response = new AuthResponse(
    //        Id: user.Id,
    //        Email: user.Email!,
    //        Username: user.UserName!,
    //        FirstName: user.FirstName,
    //        LastName: user.LastName,
    //        Address: user.Address,
    //        DateOfBirth: user.DateOfBirth,
    //        SchoolId: user.SchoolId,
    //        GradeId: user.GradeId,
    //        Token: string.Empty,
    //        IsAuthenticated: false,
    //        ExpiresIn: DateTime.UtcNow,
    //        Role: userRoles.FirstOrDefault() ?? string.Empty
    //    );
    //    return Result.Success<AuthResponse?>(response);
    //}

    // 2. LOGIN Method

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
        // 2.3 Check password + Identity sign-in rules
        // --------------------------------------------------------

        var signInResult =
            await _signInManager.PasswordSignInAsync(
                user,
                password,
                isPersistent: false,
                lockoutOnFailure: false);

        // 2.4 Password and Identity checks succeeded

        if (signInResult.Succeeded)
        {
            // 2.4.1 Get user's roles

            var roles = await _userManager.GetRolesAsync(user);

            // 2.4.2 Generate JWT

            var (token, expiresIn) =
                _jwtProvider.GenerateToken(user, roles);

            // 2.4.3 Build response

            var response = new AuthResponse(
                Id: user.Id,
                Email: user.Email!,
                Username: user.UserName!,
                FirstName: user.FirstName,
                LastName: user.LastName,
                Address: user.Address,
                DateOfBirth: user.DateOfBirth,
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
        string resetToken = request.Token.Trim();

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

    //6. Confirm Email
    public async Task<Result> ConfirmEmailAsync(
    ConfirmEmailRequest request,
    CancellationToken cancellationToken = default)
    {
        // 1. Find the user
        var user = await unitOfWork.UserRepository.FirstOrDefaultAsync(
                                                    u => u.Id.ToString() == request.UserId,
                                                    cancellationToken);

        if (user is null)
            return Result.Failure(
                new Error("User.NotFound", "User not found."));

        // 2. Check whether the email is already confirmed
        if (user.EmailConfirmed)
            return Result.Failure(
                new Error(
                    "User.EmailAlreadyConfirmed",
                    "Email is already confirmed."));

        // 3. Hash the token received from the frontend
        var tokenHash = HashToken(request.Token);

        // 4. Find the matching confirmation token
        var confirmationToken =
            await unitOfWork.EmailConfirmationTokenRepository
                            .FirstOrDefaultAsync(
                                t => t.UserId == user.Id &&
                                     t.TokenHash == tokenHash,
                                cancellationToken);

        if (confirmationToken is null)
            return Result.Failure(
                new Error(
                    "User.InvalidToken",
                    "Invalid confirmation token."));

        // 5. Check whether the token has already been used
        if (confirmationToken.UsedAt.HasValue)
            return Result.Failure(
                new Error(
                    "User.TokenAlreadyUsed",
                    "This confirmation token has already been used."));

        // 6. Check token expiration
        if (confirmationToken.ExpiresAt <= DateTime.UtcNow)
            return Result.Failure(
                new Error(
                    "User.TokenExpired",
                    "Confirmation token has expired."));

        // 7. Confirm the user's email
        user.EmailConfirmed = true;

        // 8. Mark the token as used
        confirmationToken.UsedAt = DateTime.UtcNow;

        // 9. Save both changes
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }

    // 6. CONFIRM EMAIL
    //public async Task<Result> ConfirmEmailAsync(ConfirmEmailRequest request)
    //{
    //    // --------------------------------------------------------
    //    // 6.1 Find the user
    //    // --------------------------------------------------------

    //    var user =
    //        await _userManager.FindByIdAsync(request.UserId);

    //    if (user is null)
    //    {
    //        return Result.Failure(
    //            new Error(
    //                "User.NotFound",
    //                "User not found."));
    //    }

    //    // --------------------------------------------------------
    //    // 6.2 Check whether the email is already confirmed
    //    // --------------------------------------------------------

    //    if (user.EmailConfirmed)
    //    {
    //        return Result.Failure(
    //            new Error(
    //                "User.EmailAlreadyConfirmed",
    //                "Email is already confirmed."));
    //    }

    //    // --------------------------------------------------------
    //    // 6.3 Decode the Base64Url token
    //    // --------------------------------------------------------
    //    //
    //    // During registration:
    //    //
    //    // Original Identity Token
    //    //       ↓
    //    // UTF8 bytes
    //    //       ↓
    //    // Base64UrlEncode
    //    //
    //    // Here we perform the reverse operation.

    //    string token;

    //    try
    //    {
    //        token = Encoding.UTF8.GetString(
    //            WebEncoders.Base64UrlDecode(
    //                request.Token));
    //    }
    //    catch (FormatException)
    //    {
    //        return Result.Failure(
    //            new Error(
    //                "User.InvalidToken",
    //                "Invalid confirmation token."));
    //    }

    //    // --------------------------------------------------------
    //    // 6.4 Ask ASP.NET Identity to validate the token
    //    // --------------------------------------------------------
    //    //
    //    // IMPORTANT:
    //    // Use `token`, NOT `request.Token`.
    //    //
    //    // request.Token = encoded token
    //    // token         = original Identity token

    //    var result =
    //        await _userManager.ConfirmEmailAsync(
    //            user,
    //            token);

    //    // --------------------------------------------------------
    //    // 6.5 Check Identity result
    //    // --------------------------------------------------------

    //    if (!result.Succeeded)
    //    {
    //        var errors =
    //            string.Join(
    //                ", ",
    //                result.Errors.Select(e => e.Description));

    //        return Result.Failure(
    //            new Error(
    //                "User.ConfirmEmailFailed",
    //                errors));
    //    }

    //    // --------------------------------------------------------
    //    // 6.6 Email confirmation succeeded
    //    // --------------------------------------------------------
    //    //
    //    // Identity updates:
    //    //
    //    // user.EmailConfirmed
    //    //
    //    // from:
    //    // false
    //    //
    //    // to:
    //    // true

    //    return Result.Success();
    //}

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

    // ============================================================
    // HELPERS
    // ============================================================
    private static bool IsValidEmail(string email)
    {
        if (string.IsNullOrWhiteSpace(email) || email.Length > 254) return false;
        return MailAddress.TryCreate(email, out var addr) && addr.Address == email;
    }

    // Replaces Identity's PasswordOptions (same rules as the Identity defaults, minus the symbol requirement).
    private static bool IsStrongPassword(string password) =>
        password.Length >= 8
        && Encoding.UTF8.GetByteCount(password) <= 72   // bcrypt ignores anything after 72 bytes
        && password.Any(char.IsUpper)
        && password.Any(char.IsLower)
        && password.Any(char.IsDigit);

    public static string HashToken(string rawToken) =>
        Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(rawToken)));
}