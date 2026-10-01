using EducationSystem.Application.Abstarctions.Services;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace EducationSystem.Infrastructure.Identity;

public class CurrentUserService(IHttpContextAccessor httpContextAccessor) : ICurrentUserService
{
    public Guid? UserId
    {
        get
        {
            var user = httpContextAccessor.HttpContext?.User;
            if (user is null || user.Identity?.IsAuthenticated != true)
                return null;   // anonymous request (register, login, seeding)

            var value = user.FindFirstValue(ClaimTypes.NameIdentifier)
                     ?? user.FindFirstValue("sub");

            return Guid.TryParse(value, out var id) ? id : null;
        }
    }
}