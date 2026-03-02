using AuctionSys.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AuctionSys.Infrastructure.Data.Configurations;

public class SkinMetadataConfiguration : IEntityTypeConfiguration<SkinMetadata>
{
    public void Configure(EntityTypeBuilder<SkinMetadata> builder)
    {
        builder.ToTable("SkinMetadata");

        builder.HasKey(sm => sm.Id);

        // 1-to-1 mapping with Item
        builder.HasOne(sm => sm.Item)
            .WithOne(i => i.SkinMetadata)
            .HasForeignKey<SkinMetadata>(sm => sm.ItemId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Property(sm => sm.Exterior)
            .HasConversion<string>() // Store enum as string
            .HasMaxLength(50);

        builder.Property(sm => sm.FloatValue)
            .HasPrecision(10, 8); // example: 0.12345678

        builder.Property(sm => sm.NameTag)
            .HasMaxLength(100);

        // JSONB type for Postgres if supported
        builder.Property(sm => sm.StickersJson)
            .HasColumnType("jsonb");
    }
}
