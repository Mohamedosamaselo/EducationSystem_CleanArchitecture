using EducationSystem.Domain.Entities;
using EducationSystem.Infrastructure.Persistence.Configurations.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EducationSystem.Infrastructure.Persistence.Configurations;

public class GradeConfigs : BaseAuditableEntityConfiguration<Grade>
{
    public override void Configure(EntityTypeBuilder<Grade> builder)
    {
        base.Configure(builder);

        builder.Property(x => x.Name)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(x => x.Description)
            .HasMaxLength(250)
            .IsRequired(false);

        builder.Property(x => x.IsActive)
            .IsRequired()
            .HasDefaultValue(true);

        // Configure the relationship between school & grade [1:M]
        // school 1 => has many Grades
        builder.HasOne(x => x.School)
            .WithMany(x => x.Grades)
            .HasForeignKey(x => x.SchoolId)
            .OnDelete(DeleteBehavior.Restrict);

        // relationship between grade & subject[M:M]
        builder.HasMany(x => x.Subjects)
            .WithMany(x => x.Grades);

        // Grade 1 -> Many Users
        builder.HasMany(x => x.Users)
                .WithOne(x => x.Grade)
                .HasForeignKey(x => x.GradeId)
                .OnDelete(DeleteBehavior.Restrict);
    }
}