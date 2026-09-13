using EducationSystem.Domain.Entities.Common;
using EducationSystem.Domain.Interfaces.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EducationSystem.Infrastructure.Persistence.Configurations.Common;

public abstract class BaseAuditableEntityConfiguration<TEntity> :
                      IEntityTypeConfiguration<TEntity> where TEntity :
                      BaseAuditableEntity
{
    public virtual void Configure(EntityTypeBuilder<TEntity> builder)
    {
        builder.HasKey(e => e.Id);
        // Auditing
        builder.Property(e => e.CreatedAt)
            .IsRequired()
            .HasDefaultValueSql("GETUTCDATE()")
            .ValueGeneratedOnAdd();

        builder.Property(e => e.CreatedBy)
            .IsRequired(false);

        builder.Property(e => e.ModifiedAt)
            .IsRequired(false);

        builder.Property(e => e.LastModifiedBy)
            .IsRequired(false);
    }
}