using EducationSystem.Domain.Entities;
using EducationSystem.Domain.Enums;
using EducationSystem.Infrastructure.Persistence.Configurations.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EducationSystem.Infrastructure.Persistence.Configurations;

public class ApplicationUserConfigurations : BaseAuditableEntityConfiguration<ApplicationUser>
{
    public override void Configure(EntityTypeBuilder<ApplicationUser> builder)
    {
        base.Configure(builder);

        builder.Property(u => u.FirstName)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(u => u.LastName)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(u => u.Email)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(u => u.UserName)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(u => u.PasswordHash)
            .IsRequired();

        builder.Property(u => u.PhoneNumber)
            .HasMaxLength(20);

        builder.Property(u => u.Status)
            .IsRequired()
            .HasConversion<string>()  // Convert enum to string
            .HasDefaultValue(UserStatus.Active);  // UserStatus.Active

        builder.Property(u => u.Address)
            .IsRequired()
            .HasMaxLength(500);

        builder.HasOne(u => u.School)
              .WithMany(s => s.Users)
              .HasForeignKey(u => u.SchoolId)
              .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(u => u.Grade)
               .WithMany(g => g.Users)
               .HasForeignKey(u => u.GradeId)
               .OnDelete(DeleteBehavior.Restrict);
    }
}