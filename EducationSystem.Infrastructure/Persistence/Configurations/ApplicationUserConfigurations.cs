using EducationSystem.Domain.Entities;
using EducationSystem.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EducationSystem.Infrastructure.Persistence.Configurations;

public class ApplicationUserConfigurations : IEntityTypeConfiguration<ApplicationUser>
{
    public void Configure(EntityTypeBuilder<ApplicationUser> builder)
    {
        builder.Property(e => e.UserName)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(e => e.Email)
          .IsRequired()
          .HasMaxLength(100);

        builder.Property(e => e.FirstName)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(e => e.LastName)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(e => e.Address)
           .IsRequired()
           .HasMaxLength(500);

        builder.Property(e => e.Status)
             .IsRequired()
             .HasConversion<string>()  // Convert enum to string
             .HasDefaultValue(UserStatus.Active);  // UserStatus.Active

        builder.Property(e => e.PasswordHash)
            .IsRequired();

        builder.Property(e => e.PhoneNumber)
            .HasMaxLength(20);

        builder.HasOne(e => e.School)
              .WithMany(s => s.Users)
              .HasForeignKey(u => u.SchoolId)
              .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(e => e.Grade)
               .WithMany(g => g.Users)
               .HasForeignKey(u => u.GradeId)
               .OnDelete(DeleteBehavior.Restrict);

        // Auditing
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
    }
}