using EducationSystem.Domain.Entities;
using EducationSystem.Infrastructure.Persistence.Configurations.Common;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EducationSystem.Infrastructure.Persistence.Configurations;

public class GradeConfigurations : BaseAuditableEntityConfiguration<Grade>
{
    public override void Configure(EntityTypeBuilder<Grade> builder)
    {
        base.Configure(builder);

        builder.Property(x => x.Name)
            .IsRequired()
            .HasMaxLength(100);

        builder.HasOne(x => x.School)
            .WithMany(x => x.Grades)
            .HasForeignKey(x => x.SchoolId)
            .OnDelete(Microsoft.EntityFrameworkCore.DeleteBehavior.Cascade);
    }
}