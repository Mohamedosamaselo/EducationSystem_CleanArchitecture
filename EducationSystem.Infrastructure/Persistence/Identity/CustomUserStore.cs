using EducationSystem.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace EducationSystem.Infrastructure.Persistence.Identity;

public class CustomUserStore(ApplicationDbContext db) :
    IUserStore<ApplicationUser>,
    IUserPasswordStore<ApplicationUser>,      // login: password hash
    IUserEmailStore<ApplicationUser>,         // FindByEmailAsync + unique email
    IUserRoleStore<ApplicationUser>,          // AddToRoleAsync / GetRolesAsync
    IUserSecurityStampStore<ApplicationUser>  // invalidates old reset tokens
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

    // No NormalizedUserName column → COMPUTE it instead of storing it
    public Task<string?> GetNormalizedUserNameAsync(ApplicationUser user, CancellationToken ct)
        => Task.FromResult<string?>(user.UserName.ToUpperInvariant());

    // UserManager calls this during CreateAsync — we ignore it.
    // This no-op IS the trick that removes the NormalizedUserName column.
    public Task SetNormalizedUserNameAsync(ApplicationUser user, string? normalizedUserName, CancellationToken ct)
        => Task.CompletedTask;

    // Called by: UserManager.CreateAsync
    public async Task<IdentityResult> CreateAsync(ApplicationUser user, CancellationToken ct)
    {
        db.Users.Add(user);
        await db.SaveChangesAsync(ct);   // your audit override stamps CreatedAt here
        return IdentityResult.Success;
    }

    // Called by: UserManager.UpdateAsync (e.g. right after AddToRoleAsync)
    public async Task<IdentityResult> UpdateAsync(ApplicationUser user, CancellationToken ct)
    {
        // Regenerate the stamp on every save (same as built-in IdentityUser) —
        // concurrent edits now fail instead of silently overwriting each other
        user.ConcurrencyStamp = Guid.NewGuid().ToString();

        db.Users.Update(user);
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

    // Called by: UserManager.FindByIdAsync
    public Task<ApplicationUser?> FindByIdAsync(string userId, CancellationToken ct)
        => db.Users.FirstOrDefaultAsync(u => u.Id == Guid.Parse(userId), ct);

    // Called by: UserManager.FindByNameAsync (+ username uniqueness validation)
    public Task<ApplicationUser?> FindByNameAsync(string normalizedUserName, CancellationToken ct)
        => db.Users.FirstOrDefaultAsync(u => u.UserName.ToUpper() == normalizedUserName, ct);

    // ==================== IUserPasswordStore ====================

    // UserManager hashes the password FIRST, then hands us the HASH here
    public Task SetPasswordHashAsync(ApplicationUser user, string? passwordHash, CancellationToken ct)
    {
        user.PasswordHash = passwordHash!;
        return Task.CompletedTask;
    }

    // Called by: CheckPasswordAsync (verifies password against this hash)
    public Task<string?> GetPasswordHashAsync(ApplicationUser user, CancellationToken ct)
        => Task.FromResult<string?>(user.PasswordHash);

    public Task<bool> HasPasswordAsync(ApplicationUser user, CancellationToken ct)
        => Task.FromResult(user.PasswordHash is not null);

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

    // Powers FindByEmailAsync → your Register duplicate-check AND Login
    public Task<ApplicationUser?> FindByEmailAsync(string normalizedEmail, CancellationToken ct)
        => db.Users.FirstOrDefaultAsync(u => u.Email.ToUpper() == normalizedEmail, ct);

    // No NormalizedEmail column → same compute-don't-store trick
    public Task<string?> GetNormalizedEmailAsync(ApplicationUser user, CancellationToken ct)
        => Task.FromResult<string?>(user.Email.ToUpperInvariant());

    public Task SetNormalizedEmailAsync(ApplicationUser user, string? normalizedEmail, CancellationToken ct)
        => Task.CompletedTask;

    // ==================== IUserSecurityStampStore ====================
    // UserManager auto-generates a stamp on CreateAsync and after every
    // password change/reset → old reset tokens stop working.

    public Task SetSecurityStampAsync(ApplicationUser user, string stamp, CancellationToken ct)
    {
        user.SecurityStamp = stamp;
        return Task.CompletedTask;
    }

    public Task<string?> GetSecurityStampAsync(ApplicationUser user, CancellationToken ct)
        => Task.FromResult<string?>(user.SecurityStamp);

    // ==================== IUserRoleStore ====================

    // Called by: UserManager.AddToRoleAsync
    public async Task AddToRoleAsync(ApplicationUser user, string roleName, CancellationToken ct)
    {
        var role = await FindRoleAsync(roleName, ct)
            ?? throw new InvalidOperationException(
                $"Role '{roleName}' does not exist. Did you seed roles at startup?");

        var alreadyInRole = await db.UserRoles
            .AnyAsync(ur => ur.UserId == user.Id && ur.RoleId == role.Id, ct);

        if (!alreadyInRole)
            db.UserRoles.Add(new IdentityUserRole<Guid> { UserId = user.Id, RoleId = role.Id });
        // No SaveChanges here — UserManager calls our UpdateAsync right after,
        // which persists the new join row.
    }

    public async Task RemoveFromRoleAsync(ApplicationUser user, string roleName, CancellationToken ct)
    {
        var role = await FindRoleAsync(roleName, ct);
        if (role is null) return;

        var joinRow = await db.UserRoles
            .FirstOrDefaultAsync(ur => ur.UserId == user.Id && ur.RoleId == role.Id, ct);
        if (joinRow is not null)
            db.UserRoles.Remove(joinRow);
    }

    // Called by: GetRolesAsync → feeds the roles claim in your JWT
    public async Task<IList<string>> GetRolesAsync(ApplicationUser user, CancellationToken ct)
        => await (from ur in db.UserRoles
                  join r in db.Roles on ur.RoleId equals r.Id
                  where ur.UserId == user.Id
                  select r.Name!)
                 .ToListAsync(ct);

    public async Task<bool> IsInRoleAsync(ApplicationUser user, string roleName, CancellationToken ct)
    {
        var role = await FindRoleAsync(roleName, ct);
        if (role is null) return false;

        return await db.UserRoles
            .AnyAsync(ur => ur.UserId == user.Id && ur.RoleId == role.Id, ct);
    }

    public async Task<IList<ApplicationUser>> GetUsersInRoleAsync(string roleName, CancellationToken ct)
    {
        var role = await FindRoleAsync(roleName, ct);
        if (role is null) return new List<ApplicationUser>();

        return await (from u in db.Users
                      join ur in db.UserRoles on u.Id equals ur.UserId
                      where ur.RoleId == role.Id
                      select u)
                     .ToListAsync(ct);
    }

    // db.Roles is DbSet<ApplicationRole> — your global query filter (IsDeleted)
    // applies automatically: soft-deleted roles are invisible here too.
    private Task<ApplicationRole?> FindRoleAsync(string roleName, CancellationToken ct)
        => db.Roles.FirstOrDefaultAsync(r => r.NormalizedName == roleName.ToUpperInvariant(), ct);

    // IUserStore inherits IDisposable — DI owns the DbContext, nothing to release
    public void Dispose()
    { }
}