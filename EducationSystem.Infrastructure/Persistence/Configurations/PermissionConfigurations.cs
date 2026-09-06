using EducationSystem.Domain.Entities;
using EducationSystem.Domain.Enums;
using EducationSystem.Infrastructure.Persistence.Configurations.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EducationSystem.Infrastructure.Persistence.Configurations;

internal class PermissionConfigurations : BaseAuditableEntityConfiguration<Permission>
{
    public override void Configure(EntityTypeBuilder<Permission> builder)
    {
        base.Configure(builder);

        builder.Property(p => p.Name)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(u => u.Status)
            .IsRequired()
            .HasConversion<string>()  // Convert enum to string
            .HasDefaultValue(PermissionStatus.Active);  // UserStatus.Active

        builder.Property(p => p.Description)
           .IsRequired()
           .HasMaxLength(100);
    }
}