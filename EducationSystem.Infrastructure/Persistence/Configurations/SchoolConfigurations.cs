using EducationSystem.Domain.Entities;
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

        builder.Property(s => s.Name)
            .HasMaxLength(100);

        // School with organization relationship
        builder.HasOne(s => s.Organisation)
            .WithMany(o => o.Schools)
            .HasForeignKey(s => s.OrganisationId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}