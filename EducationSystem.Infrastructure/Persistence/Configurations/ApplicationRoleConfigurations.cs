using EducationSystem.Domain.Entities;
using EducationSystem.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EducationSystem.Infrastructure.Persistence.Configurations;

public class ApplicationRoleConfigurations : IEntityTypeConfiguration<ApplicationRole>
{
    public void Configure(EntityTypeBuilder<ApplicationRole> builder)
    {
        // Primary Key
        builder.HasKey(r => r.Id);

        // Identity Role Name
        builder.Property(r => r.Name)
            .IsRequired()
            .HasMaxLength(100);

        // Custom properties
        builder.Property(r => r.Description)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(r => r.Status)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(50)
            .HasDefaultValue(RoleStatus.Active);

        // Auditing
        builder.Property(r => r.CreatedAt)
            .IsRequired()
            .HasDefaultValueSql("GETUTCDATE()")
            .ValueGeneratedOnAdd();

        builder.Property(r => r.CreatedBy)
            .IsRequired(false);

        builder.Property(r => r.ModifiedAt)
            .IsRequired(false);

        builder.Property(r => r.LastModifiedBy)
            .IsRequired(false);

        builder.HasMany(x => x.RolePermissions)
          .WithOne(x => x.Role)
          .HasForeignKey(x => x.RoleId)
          .OnDelete(DeleteBehavior.Cascade);
    }
}