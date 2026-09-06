using EducationSystem.Domain.Entities;
using EducationSystem.Domain.Enums;
using EducationSystem.Infrastructure.Persistence.Configurations.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EducationSystem.Infrastructure.Persistence.Configurations;

public class SchoolConfigurations : BaseAuditableEntityConfiguration<School>
{
    public override void Configure(EntityTypeBuilder<School> builder)
    {
        base.Configure(builder);

        builder.Property(s => s.Name)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(s => s.Email)
            .IsRequired()
            .HasMaxLength(256);

        builder.Property(s => s.LogoUrl)
            .HasMaxLength(500);

        builder.Property(s => s.Status)
            .IsRequired()
            .HasConversion<string>()  // Convert enum to string
            .HasDefaultValue(SchoolStatus.Active);  // SchoolStatus.Active

        builder.Property(s => s.Address)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(s => s.PhoneNumber)
            .IsRequired()
            .HasMaxLength(20);

        // School with organization relationship
        builder.HasOne(s => s.Organisation)
            .WithMany(o => o.Schools)
            .HasForeignKey(s => s.OrganisationId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}