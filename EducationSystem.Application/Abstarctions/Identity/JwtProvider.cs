using EducationSystem.Application.Dtos.Auth;
using EducationSystem.Domain.Entities;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace EducationSystem.Application.Abstarctions.Identity;

public class JwtProvider : IJwtProvider
{
    private const int ExpirationInMinutes = 60;
    private readonly JwtSetting _jwtSetting;

    public JwtProvider(IOptions<JwtSetting> jwtSetting)
    {
        _jwtSetting = jwtSetting.Value;
    }

    public (string token, int expiresIn) GenerateToken(ApplicationUser user, IEnumerable<string> roles)
    {
        //1- Createclaims
        var claims = new List<Claim>
        {
            // User ID
            new(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),

            // User information
            new(JwtRegisteredClaimNames.Email, user.Email!),
            new(JwtRegisteredClaimNames.GivenName, user.FirstName),
            new(JwtRegisteredClaimNames.FamilyName, user.LastName),

            // Unique token ID
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        //2.Add user roles
        foreach (var role in roles)
        {
            claims.Add(new Claim(ClaimTypes.Role, role));
        }

        //3-  create Security Key

        var Key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSetting.Key));

        //4- create Signing credentials
        var signingCredentials = new SigningCredentials(
                                                Key,
                                                SecurityAlgorithms.HmacSha256);
        //5- Token Expiration
        var expiresIn = ExpirationInMinutes;

        var expirationDate =
            DateTime.UtcNow.AddMinutes(_jwtSetting.DurationInMinutes);

        //6- Create Jwt
        var token = new JwtSecurityToken(
            issuer: _jwtSetting.Issuer,
            audience: _jwtSetting.Audience,
            claims: claims,
            expires: expirationDate,
            signingCredentials: signingCredentials
        );

        //7- convert Jwt to string
        var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

        // 8- Return Token & Expiration
        return (
            token: tokenString,
            expiresIn: expiresIn
        );
    }
}