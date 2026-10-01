using EducationSystem.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EducationSystem.Infrastructure.Persistence.Configurations;

public class StudentParentConfigs : IEntityTypeConfiguration<StudentParent>
{
    public void Configure(EntityTypeBuilder<StudentParent> builder)
    {
        builder.ToTable("StudentParents");
        builder.HasKey(sp => new { sp.StudentId, sp.ParentId }); // composite = no duplicate pairs

        builder.HasOne(sp => sp.Student)
               .WithMany()
               .HasForeignKey(sp => sp.StudentId)
               .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(sp => sp.Parent)
               .WithMany()
               .HasForeignKey(sp => sp.ParentId)
               .OnDelete(DeleteBehavior.Restrict);
    }
}