using EducationSystem.Domain.Entities;
using EducationSystem.Infrastructure.Persistence.Configurations.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class EmailConfirmationTokenConfig
    : BaseAuditableEntityConfiguration<EmailConfirmationToken>
{
    public override void Configure(
        EntityTypeBuilder<EmailConfirmationToken> builder)
    {
        base.Configure(builder);

        builder.ToTable("EmailConfirmationTokens");

        builder.Property(x => x.TokenHash)
            .IsRequired()
            .HasMaxLength(450);

        builder.HasIndex(x => x.TokenHash)
            .IsUnique();

        builder.HasOne(x => x.User)
            .WithMany(u => u.EmailConfirmationTokens)
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}