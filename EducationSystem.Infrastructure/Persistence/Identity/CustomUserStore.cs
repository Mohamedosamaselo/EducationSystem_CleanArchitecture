using EducationSystem.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace EducationSystem.Infrastructure.Persistence.Identity;

public class CustomUserStore(ApplicationDbContext db) :
    IUserStore<ApplicationUser>,
    IUserPasswordStore<ApplicationUser>,
    IUserEmailStore<ApplicationUser>,
    IUserRoleStore<ApplicationUser>,
    IUserSecurityStampStore<ApplicationUser>
{
    // ==================== IUserStore ====================

    public Task<string> GetUserIdAsync(ApplicationUser user, CancellationToken ct)
        => Task.FromResult(user.Id.ToString());

    public Task<string?> GetUserNameAsync(ApplicationUser user, CancellationToken ct)
        => Task.FromResult<string?>(user.UserName);

    public Task SetUserNameAsync(ApplicationUser user, string? userName, CancellationToken ct)
    {
        user.UserName = userName!;
        return Task.CompletedTask;
    }

    public Task<string?> GetNormalizedUserNameAsync(ApplicationUser user, CancellationToken ct)
        => Task.FromResult<string?>(user.UserName.ToUpperInvariant());

    public Task SetNormalizedUserNameAsync(ApplicationUser user, string? normalizedUserName, CancellationToken ct)
        => Task.CompletedTask;

    public async Task<IdentityResult> CreateAsync(ApplicationUser user, CancellationToken ct)
    {
        db.Users.Add(user);
        await db.SaveChangesAsync(ct);
        return IdentityResult.Success;
    }

    public async Task<IdentityResult> UpdateAsync(ApplicationUser user, CancellationToken ct)
    {
        // Attach FIRST so EF remembers the original stamp for the WHERE clause,
        // then regenerate. (No-op if the entity is already tracked.)
        db.Attach(user);
        user.ConcurrencyStamp = Guid.NewGuid().ToString();
        db.Entry(user).State = EntityState.Modified;

        try
        {
            await db.SaveChangesAsync(ct);
        }
        catch (DbUpdateConcurrencyException)
        {
            return IdentityResult.Failed(new IdentityError
            {
                Code = "ConcurrencyFailure",
                Description = "The user was modified by another operation. Please reload and try again."
            });
        }
        return IdentityResult.Success;
    }

    public async Task<IdentityResult> DeleteAsync(ApplicationUser user, CancellationToken ct)
    {
        db.Users.Remove(user);
        await db.SaveChangesAsync(ct);
        return IdentityResult.Success;
    }

    public async Task<ApplicationUser?> FindByIdAsync(string userId, CancellationToken ct)
    {
        if (!Guid.TryParse(userId, out var id))
            return null;

        return await db.Users.FirstOrDefaultAsync(u => u.Id == id, ct);
    }

    public Task<ApplicationUser?> FindByNameAsync(string normalizedUserName, CancellationToken ct)
        => db.Users.FirstOrDefaultAsync(u => u.UserName.ToUpper() == normalizedUserName, ct);

    // ==================== IUserPasswordStore ====================

    public Task SetPasswordHashAsync(ApplicationUser user, string? passwordHash, CancellationToken ct)
    {
        user.PasswordHash = passwordHash!;
        return Task.CompletedTask;
    }

    public Task<string?> GetPasswordHashAsync(ApplicationUser user, CancellationToken ct)
        => Task.FromResult<string?>(user.PasswordHash);

    public Task<bool> HasPasswordAsync(ApplicationUser user, CancellationToken ct)
        => Task.FromResult(!string.IsNullOrEmpty(user.PasswordHash));

    // ==================== IUserEmailStore ====================

    public Task SetEmailAsync(ApplicationUser user, string? email, CancellationToken ct)
    {
        user.Email = email!;
        return Task.CompletedTask;
    }

    public Task<string?> GetEmailAsync(ApplicationUser user, CancellationToken ct)
        => Task.FromResult<string?>(user.Email);

    public Task<bool> GetEmailConfirmedAsync(ApplicationUser user, CancellationToken ct)
        => Task.FromResult(user.EmailConfirmed);

    public Task SetEmailConfirmedAsync(ApplicationUser user, bool confirmed, CancellationToken ct)
    {
        user.EmailConfirmed = confirmed;
        return Task.CompletedTask;
    }

    public Task<ApplicationUser?> FindByEmailAsync(string normalizedEmail, CancellationToken ct)
        => db.Users.FirstOrDefaultAsync(u => u.Email.ToUpper() == normalizedEmail, ct);

    public Task<string?> GetNormalizedEmailAsync(ApplicationUser user, CancellationToken ct)
        => Task.FromResult<string?>(user.Email.ToUpperInvariant());

    public Task SetNormalizedEmailAsync(ApplicationUser user, string? normalizedEmail, CancellationToken ct)
        => Task.CompletedTask;

    // ==================== IUserSecurityStampStore ====================

    public Task SetSecurityStampAsync(ApplicationUser user, string stamp, CancellationToken ct)
    {
        user.SecurityStamp = stamp;
        return Task.CompletedTask;
    }

    public Task<string?> GetSecurityStampAsync(ApplicationUser user, CancellationToken ct)
        => Task.FromResult<string?>(user.SecurityStamp);

    // ==================== IUserRoleStore (single role via RoleId) ====================

    // Sets (or REPLACES) the user's one role. UserManager calls UpdateAsync right after,
    // which persists the change.
    public async Task AddToRoleAsync(ApplicationUser user, string roleName, CancellationToken ct)
    {
        var role = await FindRoleAsync(roleName, ct)
            ?? throw new InvalidOperationException(
                $"Role '{roleName}' does not exist. Did you seed roles at startup?");

        user.RoleId = role.Id;
    }

    // RoleId is required, so a user can never be role-less.
    // To change roles, call AddToRoleAsync with the new role instead.
    public Task RemoveFromRoleAsync(ApplicationUser user, string roleName, CancellationToken ct)
        => throw new NotSupportedException(
            "Every user must have exactly one role. Use AddToRoleAsync to change a user's role.");

    // Feeds the roles claim in your JWT (always 0 or 1 items)
    public async Task<IList<string>> GetRolesAsync(ApplicationUser user, CancellationToken ct)
    {
        var roleName = await db.Roles
            .Where(r => r.Id == user.RoleId)
            .Select(r => r.Name)
            .FirstOrDefaultAsync(ct);

        return roleName is null ? new List<string>() : new List<string> { roleName };
    }

    public async Task<bool> IsInRoleAsync(ApplicationUser user, string roleName, CancellationToken ct)
    {
        var role = await FindRoleAsync(roleName, ct);
        return role is not null && user.RoleId == role.Id;
    }

    public async Task<IList<ApplicationUser>> GetUsersInRoleAsync(string roleName, CancellationToken ct)
    {
        var role = await FindRoleAsync(roleName, ct);
        if (role is null) return new List<ApplicationUser>();

        return await db.Users.Where(u => u.RoleId == role.Id).ToListAsync(ct);
    }

    private Task<ApplicationRole?> FindRoleAsync(string roleName, CancellationToken ct)
        => db.Roles.FirstOrDefaultAsync(r => r.NormalizedName == roleName.ToUpperInvariant(), ct);

    public void Dispose()
    { }
}