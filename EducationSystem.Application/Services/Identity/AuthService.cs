using EducationSystem.Application.Abstarctions.Identity;
using EducationSystem.Application.Dtos.Auth;
using EducationSystem.Application.Dtos.Request;
using EducationSystem.Application.Dtos.Request.Auth;
using EducationSystem.Application.Dtos.Response.Auth;
using EducationSystem.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace EducationSystem.Infrastructure.Identity;

public class AuthService : IAuthService
{
    #region Fields

    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IOptions<JwtSetting> _jwt;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly RoleManager<ApplicationRole> _roleManager;

    #endregion Fields

    #region Constructors

    public AuthService(UserManager<ApplicationUser> userManager,
        IOptions<JwtSetting> jwt,
        SignInManager<ApplicationUser> signInManager,
        RoleManager<ApplicationRole> roleManager)
    {
        _userManager = userManager;
        _roleManager = roleManager;
        _signInManager = signInManager;
        _jwt = jwt;
    }

    #endregion Constructors

    #region Methods

    public async Task<AuthResponse?> RegisterAsync(RegisterRequestDto request,
        CancellationToken cancellationToken = default)
    {
        // Check on UserEmail
        var IsEmailExists = await _userManager.FindByEmailAsync(request.Email);

        if (IsEmailExists is not null)
            return new AuthResponse { Message = "Email is already Registered" };

        // create user
        var User = new ApplicationUser
        {
            Email = request.Email,
            UserName = request.Email,
            FirstName = request.FirstName,
            LastName = request.LastName,
        };

        // set user in DB
        var result = await _userManager.CreateAsync(User, request.Password);

        if (!result.Succeeded)// if Result Failed
        {
            var errors = string.Empty;

            foreach (var Error in result.Errors)
            {
                errors += $" {Error.Code} , {Error.Description} ";
            }
            return new AuthResponse { Message = errors };
        }

        // we can add user in any role like user [ b y defualt we add any new user to user Role ]
        await _userManager.AddToRoleAsync(User, "User");

        // the last step to generate userToken
        var jwtSecurityToken = await CreateJwtTokenAsync(User);

        // Return RegisterResonseDto
        return new AuthResponse()
        {
            Message = "User registered Successfuly ",
            IsAuthenticated = true,
            ExpiresAt = jwtSecurityToken.ValidTo,
            Email = request.Email,
            Roles = new List<string> { "User" },
            UserToken = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken),
            Username = request.Email,
        };
    }

    public async Task<AuthResponse> LoginAsync(LoginRequestDto request, CancellationToken cancellationToken = default)
    {
        var authModel = new AuthResponse();

        //1. check email or username First
        var user = await _userManager.FindByEmailAsync(request.Email);

        //2. check on  user & Password
        if (user is null || !await _userManager.CheckPasswordAsync(user, request.Password)) //CheckOn user then  password
        {
            authModel.Message = "Invalid Email or Password ";
            return authModel;
        }

        //3. check email Confirmation
        if (!await _userManager.IsEmailConfirmedAsync(user))
        {
            authModel.Message = "Please confirm your email before logging in.";
            return authModel;
        }

        //4. Generate Jwt token
        var jwtSecurityToken = await CreateJwtTokenAsync(user);

        //5.Get user roles
        var rolesList = await _userManager.GetRolesAsync(user);

        var loginRespose = new AuthResponse()
        {
            Message = "User login successfully",
            IsAuthenticated = true,
            UserToken = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken),
            Email = user.Email!,
            Username = user.Email!,
            ExpiresAt = jwtSecurityToken.ValidTo,
            Roles = rolesList.ToList()
        };

        return loginRespose;
    }

    //public async Task<string> AddRoleAsync(AddRoleModel model)
    //{
    //    // check on username in DB
    //    var user = await _userManager.FindByIdAsync(model.UserId);
    //    if (user is null)
    //        return "Invalid UserId Or Role ";

    //    // check on RoleName in DB
    //    var Role = await _roleManager.RoleExistsAsync(model.RoleName);
    //    if (!Role)
    //        return "Invalid UserId Or Role ";

    //    // check is User Assigned to this Role Or Not
    //    var isUserAssignedToRole = await _userManager.IsInRoleAsync(user, model.RoleName);

    //    if (isUserAssignedToRole) return "User Already Assigned to this Role ";

    //    // Add User to this Role
    //    var result = await _userManager.AddToRoleAsync(user, model.RoleName);

    //    return result.Succeeded ? string.Empty : "there is issue when you Adding User to this Role ";
    //}

    #endregion Methods

    #region Helpers

    private async Task<JwtSecurityToken> CreateJwtTokenAsync(ApplicationUser user)
    {
        // Get custom claims already assigned to the user
        var userClaims = await _userManager.GetClaimsAsync(user);

        // Get roles assigned to the user
        var roles = await _userManager.GetRolesAsync(user);

        // Create a collection for role claims
        var roleClaims = new List<Claim>();

        // Convert every role into a Claim
        foreach (var role in roles)
        {
            roleClaims.Add(new Claim("roles", role));
        }

        // Create the main claims for the JWT
        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.UserName!),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim(JwtRegisteredClaimNames.Email, user.Email!),
            //new Claim("uid",(Guid) user.Id)
        }
        .Union(userClaims)
        .Union(roleClaims);

        // Create the secret key that responsible for encoding , decoding token
        var symmetricSecurityKey =
            new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(_jwt.Value.Key)
            );

        // Create Signing credentials
        var signingCredentials =
            new SigningCredentials(symmetricSecurityKey, SecurityAlgorithms.HmacSha256);

        // Create the JWT
        var jwtSecurityToken = new JwtSecurityToken(
            issuer: _jwt.Value.Issuer,
            audience: _jwt.Value.Audience,
            claims: claims,
            expires: DateTime.Now.AddDays(_jwt.Value.ExpiresIn), // expire in Double
            signingCredentials: signingCredentials
        );

        return jwtSecurityToken;
    }

    #endregion Helpers
}