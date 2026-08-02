using EducationSystem.Domain.Common;
using EducationSystem.Domain.Entities;
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

        builder.Property(e => e.Id).ValueGeneratedNever(); // never create value as it created in the domain model

        builder.Property(e => e.CreatedAt)
                .HasDefaultValueSql("getutcdate()")
                .ValueGeneratedOnAdd()
                .IsRequired();

        builder.Property(e => e.CreatedBy);

        builder.Property(e => e.ModifiedAt);

        builder.Property(e => e.LastModifiedBy);
    }
}