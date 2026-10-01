using EducationSystem.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EducationSystem.Infrastructure.Persistence.Configurations;

public class ApplicationRoleConfigs : IEntityTypeConfiguration<ApplicationRole>
{
    public void Configure(EntityTypeBuilder<ApplicationRole> builder)
    {
        builder.ToTable("AspNetRoles");

        builder.HasKey(r => r.Id);

        builder.Property(r => r.Name).HasMaxLength(256);
        builder.Property(r => r.NormalizedName).HasMaxLength(256);
        builder.Property(r => r.ConcurrencyStamp).IsConcurrencyToken();
        //builder.Property(r => r.Description).HasMaxLength(300);

        // Detail 1: filtered unique index — a soft-DELETED role's name must not
        // block re-creating a role with the same name later
        builder.HasIndex(r => r.NormalizedName).IsUnique().HasFilter("[IsDeleted] = 0");

        // Detail 2: global query filter — soft-deleted roles vanish from ALL queries
        builder.HasQueryFilter(r => !r.IsDeleted);
    }
}