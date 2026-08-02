using EducationSystem.Domain.Entities;
using EducationSystem.Infrastructure.Persistence.Configurations.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EducationSystem.Infrastructure.Persistence.Configurations;

public class OrganisationConfigurations : BaseAuditableEntityConfiguration<Organisation>
{
    public override void Configure(EntityTypeBuilder<Organisation> builder)
    {
        base.Configure(builder);

        builder.Property(o => o.Name)
              .IsRequired()
              .HasMaxLength(100);

        builder.HasMany(o => o.Schools)
               .WithOne(s => s.Organisation)
               .HasForeignKey(s => s.OrganisationId)
               .OnDelete(DeleteBehavior.Cascade);
    }
}