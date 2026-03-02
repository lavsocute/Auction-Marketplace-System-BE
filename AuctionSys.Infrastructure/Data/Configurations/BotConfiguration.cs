using AuctionSys.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AuctionSys.Infrastructure.Data.Configurations;

public class BotConfiguration : IEntityTypeConfiguration<Bot>
{
    public void Configure(EntityTypeBuilder<Bot> builder)
    {
        builder.ToTable("Bots");

        builder.HasKey(b => b.Id);

        builder.Property(b => b.SteamId)
            .IsRequired()
            .HasMaxLength(64);

        builder.Property(b => b.Name)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(b => b.TradeUrl)
            .HasMaxLength(500);

        builder.HasIndex(b => b.SteamId).IsUnique();
        
        builder.Property(b => b.CreatedAt)
            .HasDefaultValueSql("timezone('utc', now())");
    }
}
