using Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Configuration;

internal sealed class ActorConfiguration : IEntityTypeConfiguration<Actor>
{
    public void Configure(EntityTypeBuilder<Actor> builder)
    {
        builder.ToTable("Actors");

        builder.UseTptMappingStrategy()
            .HasKey(a => a.InternalId);

        builder.Property(a => a.PublicId)
            .IsRequired();

        builder.HasIndex(a => a.PublicId)
            .IsUnique();
    }
}
