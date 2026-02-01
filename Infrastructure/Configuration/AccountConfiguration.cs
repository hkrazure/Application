using Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Configuration;

internal sealed class AccountConfiguration : IEntityTypeConfiguration<Account>
{
    public void Configure(EntityTypeBuilder<Account> builder)
    {
        builder.ToTable("Accounts");

        builder.HasKey(a => a.InternalId);

        builder.Property(a => a.PublicId)
           .IsRequired();

        builder.Property(a => a.CurrencyType)
            .IsRequired();

        builder.Property(a => a.Balance)
            .IsRequired();

        builder.HasIndex(a => a.PublicId)
            .IsUnique();

        builder.OwnsOne(a => a.AccountNumber, builder =>
        {
            builder.Property(an => an.Value)
                .HasColumnName("AccountNumber")
                .IsRequired();

            builder.HasIndex(an => an.Value)
            .IsUnique();
        });

        builder.HasOne(a => a.Actor)
            .WithMany()
            .HasForeignKey(a => a.ActorId)
            .IsRequired();
    }
}
