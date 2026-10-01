using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EducationSystem.Infrastructure.Persistence.Configurations;

public class IdentityRoleClaimConfigs
    : IEntityTypeConfiguration<IdentityRoleClaim<Guid>>
{
    public void Configure(
        EntityTypeBuilder<IdentityRoleClaim<Guid>> builder)
    {
        builder.HasKey(rc => rc.Id);

        builder.ToTable("AspNetRoleClaims");
    }
}