using EducationSystem.Application.Abstarctions.HandlingError;
using EducationSystem.Application.Abstarctions.Identity;
using EducationSystem.Application.Abstractions.HandlingError.Errors;
using EducationSystem.Application.Dtos.Request.Auth;
using EducationSystem.Application.Dtos.Response.Auth;
using EducationSystem.Domain.Entities;
using Microsoft.AspNetCore.Identity;

namespace EducationSystem.Infrastructure.Identity;

public class AuthService(UserManager<ApplicationUser> userManager,
                            IJwtProvider jwtProvider) : IAuthService
{
    private readonly UserManager<ApplicationUser> _userManager = userManager;
    private readonly IJwtProvider _jwtProvider = jwtProvider;

    //public async Task<Result<AuthResponse?>> RegisterAsync(RegisterRequest request, CancellationToken cancellationToken = default)
    //{
    //    // 1. Check if email already exists
    //    var Existinguser = await _userManager.FindByEmailAsync(request.Email);

    //    if (Existinguser is not null)
    //        //return null;
    //        return Result.Failure<AuthResponse>(UserErrors.UserNotFound!);

    //    //2- create user
    //    var user = new ApplicationUser
    //    {
    //        Id = Guid.NewGuid(),
    //        Email = request.Email,
    //        UserName = request.Email,
    //        FirstName = request.FirstName,
    //        LastName = request.LastName,
    //        PasswordHash = request.Password,
    //        Address = request.Address
    //    };

    //    // 3. Create user with password
    //    var result =
    //        await _userManager.CreateAsync(
    //            Existinguser!,
    //            request.Password);

    //    if (!result.Succeeded)// if Result creation user Failed
    //        return null;

    //    // 4. Assign default role
    //    const string defaultRole = "Student";

    //    var roleResult =
    //        await _userManager.AddToRoleAsync(
    //            Existinguser,
    //            defaultRole);

    //    // If role assignment fails,
    //    // remove the user to avoid partial registration.
    //    if (!roleResult.Succeeded)
    //    {
    //        await _userManager.DeleteAsync(Existinguser);
    //        return null;
    //    }

    //    // 5. Get user's roles
    //    var roles =
    //        await _userManager.GetRolesAsync(Existinguser);

    //    // 6. Generate JWT
    //    var (token, expiresIn) =
    //        _jwtProvider.GenerateToken(Existinguser,
    //                                    roles);

    //    // 7. Return response
    //    var response = new AuthResponse(
    //        Id: Existinguser.Id,
    //        Email: Existinguser.Email!,
    //        Username: Existinguser.UserName!,
    //        FirstName: Existinguser.FirstName,
    //        LastName: Existinguser.LastName,
    //        Token: token,
    //        IsAuthenticated: true,
    //        ExpiresIn: DateTime.UtcNow.AddMinutes(expiresIn),
    //        Roles: roles.ToList()
    //    );
    //    return Result.Success(response)!;
    //}

    public async Task<Result<AuthResponse?>> RegisterAsync(
    RegisterRequest request,
    CancellationToken cancellationToken = default)
    {
        // 1. Check if email is already taken
        var existingUser = await _userManager.FindByEmailAsync(request.Email);
        if (existingUser is not null)
            return Result.Failure<AuthResponse?>(UserErrors.DuplicateEmail);

        // 2. Build the new user entity
        //    NOTE: do NOT set PasswordHash here — UserManager.CreateAsync(user, password)
        //    will hash the password itself.
        var user = new ApplicationUser
        {
            Id = Guid.NewGuid(),
            Email = request.Email,
            UserName = request.Email,        // or request.UserName if you have one
            FirstName = request.FirstName,
            LastName = request.LastName,
            Address = request.Address,
            EmailConfirmed = false
        };

        // 3. Create the user with a hashed password
        var createResult = await _userManager.CreateAsync(user, request.Password);
        if (!createResult.Succeeded)
        {
            // Map Identity errors to our own error codes for consistent responses
            var firstError = createResult.Errors.First();
            var error = firstError.Code switch
            {
                "DuplicateEmail" => UserErrors.DuplicateEmail,
                "DuplicateUserName" => UserErrors.DuplicateUserName,
                "PasswordRequiresLower" => UserErrors.WeakPassword,
                _ => new Error("User.CreationFailed", firstError.Description)
            };

            return Result.Failure<AuthResponse?>(error);
        }

        // 4. Assign default role ("Student")
        const string defaultRole = "Student";
        var roleResult = await _userManager.AddToRoleAsync(user, defaultRole);
        if (!roleResult.Succeeded)
        {
            // Roll back: delete the partial user to avoid inconsistent state
            await _userManager.DeleteAsync(user);

            return Result.Failure<AuthResponse?>(
                new Error("User.RoleAssignmentFailed", "Failed to assign default role."));
        }

        // 5. Get user's roles (now includes the just-assigned role)
        var roles = await _userManager.GetRolesAsync(user);

        // 6. Generate JWT
        var (token, expiresIn) = _jwtProvider.GenerateToken(user, roles);

        // 7. Build and return the AuthResponse
        var response = new AuthResponse(
            Id: user.Id,
            Email: user.Email!,
            Username: user.UserName!,
            FirstName: user.FirstName,
            LastName: user.LastName,
            Token: token,
            IsAuthenticated: true,
            ExpiresIn: DateTime.UtcNow.AddMinutes(expiresIn),
            Roles: roles.ToList()
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
            Token: token,
            IsAuthenticated: true,
            ExpiresIn: DateTime.UtcNow.AddMinutes(expiresIn),
            Roles: roles.ToList()
        );
        return Result.Success(response)!;
    }
}