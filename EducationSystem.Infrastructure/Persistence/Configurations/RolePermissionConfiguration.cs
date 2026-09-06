using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using EducationSystem.Domain.Entities;

namespace EducationSystem.Infrastructure.Persistence.Configurations
{
    public class RolePermissionConfiguration : IEntityTypeConfiguration<RolePermission>
    {
        public void Configure(EntityTypeBuilder<RolePermission> builder)
        {
            // Map to table (optional)
            builder.ToTable("RolePermissions");

            // Configure composite primary key
            builder.HasKey(rp => new { rp.RoleId, rp.PermissionId });

            // Relationship to Role (Role does not have a RolePermissions nav in current model)
            builder.HasOne(rp => rp.Role)
                   .WithMany()
                   .HasForeignKey(rp => rp.RoleId)
                   .OnDelete(DeleteBehavior.Cascade);

            // Relationship to Permission (Permission has RolePermissions collection)
            builder.HasOne(rp => rp.Permission)
                   .WithMany(p => p.RolePermissions)
                   .HasForeignKey(rp => rp.PermissionId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
