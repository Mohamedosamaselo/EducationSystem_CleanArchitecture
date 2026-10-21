using EducationSystem.Domain.Entities;
using EducationSystem.Infrastructure.Persistence.Configurations.Common;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EducationSystem.Infrastructure.Persistence.Configurations;

public class SubjectConfigs : BaseAuditableEntityConfiguration<Subject>
{
    public override void Configure(EntityTypeBuilder<Subject> builder)
    {
        builder.Property(e => e.Name)
               .IsRequired()
               .HasMaxLength(100);

        // Configure the relationship between grade & subject[M:M]
        builder.HasMany(s => s.Grades)
               .WithMany(g => g.Subjects);
    }
}