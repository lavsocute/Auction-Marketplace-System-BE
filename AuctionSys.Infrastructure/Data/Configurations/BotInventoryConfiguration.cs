using AuctionSys.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AuctionSys.Infrastructure.Data.Configurations;

public class BotInventoryConfiguration : IEntityTypeConfiguration<BotInventory>
{
    public void Configure(EntityTypeBuilder<BotInventory> builder)
    {
        builder.ToTable("BotInventories");

        builder.HasKey(bi => bi.Id);

        builder.HasOne(bi => bi.Bot)
            .WithMany(b => b.Inventories)
            .HasForeignKey(bi => bi.BotId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(bi => bi.Item)
            .WithMany(i => i.BotInventories)
            .HasForeignKey(bi => bi.ItemId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(bi => bi.TradeLockedUntil); // Optimization for the background worker
        
        builder.Property(bi => bi.CreatedAt)
            .HasDefaultValueSql("timezone('utc', now())");
    }
}
