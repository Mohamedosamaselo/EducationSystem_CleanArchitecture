using EducationSystem.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace EducationSystem.Infrastructure.Persistence;

//public class ApplicationDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, Guid>
public class ApplicationDbContext
    : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
    }

    // DbSets
    public DbSet<ApplicationUser> Users => Set<ApplicationUser>();

    public DbSet<ApplicationRole> Roles => Set<ApplicationRole>();
    public DbSet<IdentityUserRole<Guid>> UserRoles => Set<IdentityUserRole<Guid>>();
    public DbSet<IdentityRoleClaim<Guid>> RoleClaims => Set<IdentityRoleClaim<Guid>>();
    public DbSet<Organisation> Organisations { get; set; }
    public DbSet<School> Schools { get; set; }
    public DbSet<Subject> Subjects { get; set; }
    public DbSet<Grade> Grades { get; set; }
}