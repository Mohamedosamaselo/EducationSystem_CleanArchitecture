using EducationSystem.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EducationSystem.Infrastructure.Persistence.Configurations;

public class ApplicationUserConfigs : IEntityTypeConfiguration<ApplicationUser>
{
    public void Configure(EntityTypeBuilder<ApplicationUser> builder)
    {
        builder.ToTable("AspNetUsers");

        builder.HasKey(u => u.Id);

        // ---- Identity minimum columns ----
        builder.Property(u => u.UserName)
            .IsRequired()
            .HasMaxLength(256);

        builder.Property(u => u.Email)
            .IsRequired()
            .HasMaxLength(256);

        builder.Property(u => u.PasswordHash)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(u => u.SecurityStamp)
            .HasMaxLength(100);

        // Concurrency token: every UPDATE checks the stamp →
        // two simultaneous edits can't silently overwrite each other
        builder.Property(u => u.ConcurrencyStamp)
               .IsConcurrencyToken()
               .HasMaxLength(100);

        // ---- Your own columns ----
        builder.Property(u => u.FirstName).IsRequired().HasMaxLength(100);
        builder.Property(u => u.LastName).IsRequired().HasMaxLength(100);
        builder.Property(u => u.Address).HasMaxLength(300);

        // ---- Uniqueness enforced by the DB ----Email & UserName is Unique
        builder.HasIndex(u => u.Email).IsUnique();
        builder.HasIndex(u => u.UserName).IsUnique();

        //// Auditing
        builder.Property(e => e.CreatedAt)
            .IsRequired()
            .HasDefaultValueSql("GETUTCDATE()")
            .ValueGeneratedOnAdd();

        builder.Property(e => e.CreatedBy)
            .IsRequired(false);

        builder.Property(e => e.ModifiedAt)
            .IsRequired(false);

        builder.Property(e => e.LastModifiedBy)
            .IsRequired(false);

        builder.HasOne(u => u.Role)
               .WithMany()
               .HasForeignKey(u => u.RoleId)
                .OnDelete(DeleteBehavior.Restrict);

        // school
        builder.HasOne(u => u.School)
               .WithMany(s => s.Users)
               .HasForeignKey(u => u.SchoolId)
                .OnDelete(DeleteBehavior.Restrict);

        // Grade with many students
        builder.HasOne(u => u.Grade)
               .WithMany(s => s.Students)
               .HasForeignKey(u => u.GradeId)
                .OnDelete(DeleteBehavior.Restrict);
    }
}