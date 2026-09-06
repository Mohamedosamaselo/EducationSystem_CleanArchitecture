using EducationSystem.Domain.Entities;
using EducationSystem.Domain.Enums;
using EducationSystem.Infrastructure.Persistence.Configurations.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EducationSystem.Infrastructure.Persistence.Configurations;

public class ApplicationRoleConfigurations : BaseAuditableEntityConfiguration<ApplicationRole>
{
    public override void Configure(EntityTypeBuilder<ApplicationRole> builder)
    {
        base.Configure(builder);

        builder.Property(u => u.Name)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(u => u.Description)
           .IsRequired()
           .HasMaxLength(500);

        builder.Property(u => u.Status)
            .IsRequired()
            .HasConversion<string>()  // Convert enum to string
            .HasDefaultValue(RoleStatus.Active);  // UserStatus.Active
    }
}