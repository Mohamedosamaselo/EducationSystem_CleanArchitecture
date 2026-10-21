using EducationSystem.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EducationSystem.Infrastructure.Persistence.Configurations;

public class RoleClaimsConfigs
    : IEntityTypeConfiguration<IdentityRoleClaim<Guid>>
{
    public void Configure(
        EntityTypeBuilder<IdentityRoleClaim<Guid>> builder)
    {
        builder.ToTable("AspNetRoleClaims");
        builder.HasKey(rc => rc.Id);

        builder.Property(rc => rc.ClaimType).HasMaxLength(256);
        builder.Property(rc => rc.ClaimValue).HasMaxLength(1024);

        builder.HasOne<ApplicationRole>()
               .WithMany(r => r.Claims)
               .HasForeignKey(rc => rc.RoleId)
               .IsRequired()
               .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(rc => rc.RoleId);
    }
}