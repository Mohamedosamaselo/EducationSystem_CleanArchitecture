using EducationSystem.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EducationSystem.Infrastructure.Persistence.Configurations;

public class UserSubjectConfigs : IEntityTypeConfiguration<UserSubject>
{
    public void Configure(EntityTypeBuilder<UserSubject> builder)
    {
        builder.HasKey(us => new { us.UserId, us.SubjectId }); // combined primary key

        builder.HasOne(us => us.User)
               .WithMany(u => u.UserSubjects)
               .HasForeignKey(us => us.UserId)
               .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(us => us.Subject)
                   .WithMany(s => s.UserSubjects)
                   .HasForeignKey(us => us.SubjectId)
                   .OnDelete(DeleteBehavior.Restrict);
    }
}