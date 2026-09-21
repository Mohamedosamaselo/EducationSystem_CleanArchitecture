using EducationSystem.Domain.Entities;

namespace EducationSystem.Application.Abstarctions.Identity;

public interface IJwtProvider
{
    //(string token, int expiresIn) GenerateToken(ApplicationUser user);
    (string token, int expiresIn) GenerateToken(ApplicationUser user, IEnumerable<string> roles);
}