using AuctionSys.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AuctionSys.Infrastructure.Data.Configurations;

public class AuctionConfiguration : IEntityTypeConfiguration<Auction>
{
    public void Configure(EntityTypeBuilder<Auction> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.StartPrice).HasPrecision(18, 2);
        builder.Property(x => x.CurrentPrice).HasPrecision(18, 2);
        builder.Property(x => x.Status).HasConversion<string>();

        builder.HasOne(x => x.Item).WithOne(i => i.Auction).HasForeignKey<Auction>(x => x.ItemId).OnDelete(DeleteBehavior.Cascade);
        builder.HasOne(x => x.Winner).WithMany().HasForeignKey(x => x.WinnerId).OnDelete(DeleteBehavior.SetNull);

        builder.HasIndex(x => x.Status);
        builder.HasIndex(x => x.EndTime);
    }
}

public class BidConfiguration : IEntityTypeConfiguration<Bid>
{
    public void Configure(EntityTypeBuilder<Bid> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Amount).HasPrecision(18, 2);

        builder.HasOne(x => x.Auction).WithMany(a => a.Bids).HasForeignKey(x => x.AuctionId).OnDelete(DeleteBehavior.Cascade);
        builder.HasOne(x => x.Bidder).WithMany(u => u.Bids).HasForeignKey(x => x.BidderId).OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(x => x.AuctionId);
    }
}
