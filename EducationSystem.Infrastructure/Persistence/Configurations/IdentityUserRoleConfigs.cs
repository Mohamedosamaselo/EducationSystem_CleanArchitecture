using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EducationSystem.Infrastructure.Persistence.Configurations;

public class IdentityUserRoleConfigs
    : IEntityTypeConfiguration<IdentityUserRole<Guid>>
{
    public void Configure(
        EntityTypeBuilder<IdentityUserRole<Guid>> builder)
    {
        builder.HasKey(ur => new
        {
            ur.UserId,
            ur.RoleId
        });

        builder.ToTable("AspNetUserRoles");
    }
}