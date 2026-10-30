using EducationSystem.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace EducationSystem.Infrastructure.Persistence;

//public class ApplicationDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, Guid>
public class ApplicationDbContext : DbContext
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
    public DbSet<Organisation> Organisations => Set<Organisation>();
    public DbSet<School> Schools => Set<School>();
    public DbSet<Grade> Grades => Set<Grade>();
    public DbSet<Subject> Subjects => Set<Subject>();
    public DbSet<TeacherGrade> TeacherGrades => Set<TeacherGrade>();
    public DbSet<UserSubject> UserSubjects => Set<UserSubject>();

    public DbSet<EmailConfirmationToken> EmailConfirmationTokens => Set<EmailConfirmationToken>();
}