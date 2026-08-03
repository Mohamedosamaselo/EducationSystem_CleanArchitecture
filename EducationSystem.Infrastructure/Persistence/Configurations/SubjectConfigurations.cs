using EducationSystem.Domain.Entities;
using EducationSystem.Infrastructure.Persistence.Configurations.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EducationSystem.Infrastructure.Persistence.Configurations;

public class SubjectConfigurations : BaseAuditableEntityConfiguration<Subject>
{
    public override void Configure(EntityTypeBuilder<Subject> builder)
    {
        base.Configure(builder);

        builder.Property(e => e.Name)
               .IsRequired()
               .HasMaxLength(100);

        builder.Property(e => e.Code)
            .IsRequired();

        // Configure the relationship between School and Subject entities [1:M]
        builder.HasOne(s => s.School)
               .WithMany(s => s.Subjects)
               .HasForeignKey(s => s.SchoolId)
               .OnDelete(DeleteBehavior.Restrict);

        // Configure the relationship between grade & subject[M:M]
        builder.HasMany(s => s.Grades)
               .WithMany(g => g.Subjects);
    }
}