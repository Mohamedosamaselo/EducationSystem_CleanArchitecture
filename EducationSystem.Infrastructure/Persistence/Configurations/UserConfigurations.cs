using EducationSystem.Domain.Entities;
using EducationSystem.Infrastructure.Persistence.Configurations.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EducationSystem.Infrastructure.Persistence.Configurations;

public class UserConfigurations : BaseAuditableEntityConfiguration<User>
{
    public override void Configure(EntityTypeBuilder<User> builder)
    {
        base.Configure(builder);

        builder.Property(u => u.Name)
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

        builder.Property(u => u.Address)
            .HasMaxLength(200);

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