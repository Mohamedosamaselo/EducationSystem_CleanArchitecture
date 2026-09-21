using EducationSystem.Application.Abstarctions.Identity;
using EducationSystem.Application.Dtos.Request.Auth;
using EducationSystem.Application.Dtos.Response.Auth;
using EducationSystem.Domain.Entities;
using Microsoft.AspNetCore.Identity;

namespace EducationSystem.Infrastructure.Identity;

public class AuthService(
                            UserManager<ApplicationUser> userManager,
                            IJwtProvider jwtProvider
                                ) : IAuthService
{
    private readonly UserManager<ApplicationUser> _userManager = userManager;
    private readonly IJwtProvider _jwtProvider = jwtProvider;

    public async Task<AuthResponse?> RegisterAsync(RegisterRequest request, CancellationToken cancellationToken = default)
    {
        // 1. Check if email already exists
        var existingUser =
            await _userManager.FindByEmailAsync(request.Email);

        if (existingUser is not null)
            return null;

        //2- create user
        var user = new ApplicationUser
        {
            Id = Guid.NewGuid(),
            Email = request.Email,
            UserName = request.Email,
            FirstName = request.FirstName,
            LastName = request.LastName,
            Address = request.Address
        };

        // 3. Create user with password
        var result =
            await _userManager.CreateAsync(
                user,
                request.Password);

        if (!result.Succeeded)// if Result Failed
            return null;

        // 4. Assign default role
        const string defaultRole = "Student";

        var roleResult =
            await _userManager.AddToRoleAsync(
                user,
                defaultRole);

        // If role assignment fails,
        // remove the user to avoid partial registration.
        if (!roleResult.Succeeded)
        {
            await _userManager.DeleteAsync(user);
            return null;
        }

        // 5. Get user's roles
        var roles =
            await _userManager.GetRolesAsync(user);

        // 6. Generate JWT
        var (token, expiresIn) =
            _jwtProvider.GenerateToken(user,
                                        roles);

        // 7. Return response
        return new AuthResponse(
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
    }

    // login
    public async Task<AuthResponse?> GetTokenAsync(string Email, string password, CancellationToken cancellationToken = default)
    {
        // 1. Find User
        var user = await _userManager.FindByEmailAsync(Email);

        if (user is null)
            return null;

        // 2. Check password
        var isPasswordValid =
            await _userManager.CheckPasswordAsync(user, password);

        if (!isPasswordValid)
            return null;

        // 3. Get User Roles

        var roles = await _userManager.GetRolesAsync(user);

        // 4. Generate Token
        var (token, expiresIn) = _jwtProvider.GenerateToken(user, roles);

        // 5. Return AuthReponse
        return new AuthResponse(
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
    }
}