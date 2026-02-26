using AuctionSys.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AuctionSys.Infrastructure.Data.Configurations;

public class ReviewConfiguration : IEntityTypeConfiguration<Review>
{
    public void Configure(EntityTypeBuilder<Review> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Comment).HasMaxLength(1000);

        builder.HasOne(x => x.Order).WithMany().HasForeignKey(x => x.OrderId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(x => x.Reviewer).WithMany(u => u.ReviewsGiven).HasForeignKey(x => x.ReviewerId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(x => x.Seller).WithMany(u => u.ReviewsReceived).HasForeignKey(x => x.SellerId).OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(x => x.OrderId).IsUnique();
        builder.HasIndex(x => x.SellerId);
    }
}

public class WishlistConfiguration : IEntityTypeConfiguration<Wishlist>
{
    public void Configure(EntityTypeBuilder<Wishlist> builder)
    {
        builder.HasKey(x => x.Id);

        builder.HasOne(x => x.User).WithMany(u => u.Wishlists).HasForeignKey(x => x.UserId).OnDelete(DeleteBehavior.Cascade);
        builder.HasOne(x => x.Item).WithMany(i => i.Wishlists).HasForeignKey(x => x.ItemId).OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(x => new { x.UserId, x.ItemId }).IsUnique();
    }
}

public class NotificationConfiguration : IEntityTypeConfiguration<Notification>
{
    public void Configure(EntityTypeBuilder<Notification> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Type).HasConversion<string>();
        builder.Property(x => x.Title).IsRequired().HasMaxLength(200);
        builder.Property(x => x.Message).HasMaxLength(1000);

        builder.HasOne(x => x.User).WithMany(u => u.Notifications).HasForeignKey(x => x.UserId).OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(x => x.UserId);
        builder.HasIndex(x => x.IsRead);
    }
}

public class ReportConfiguration : IEntityTypeConfiguration<Report>
{
    public void Configure(EntityTypeBuilder<Report> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.TargetType).HasConversion<string>();
        builder.Property(x => x.Status).HasConversion<string>();
        builder.Property(x => x.Reason).IsRequired().HasMaxLength(500);
        builder.Property(x => x.Description).HasMaxLength(2000);

        builder.HasOne(x => x.Reporter).WithMany(u => u.Reports).HasForeignKey(x => x.ReporterId).OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(x => x.Status);
    }
}

public class AuctionWatcherConfiguration : IEntityTypeConfiguration<AuctionWatcher>
{
    public void Configure(EntityTypeBuilder<AuctionWatcher> builder)
    {
        builder.HasKey(x => x.Id);

        builder.HasOne(x => x.User).WithMany(u => u.AuctionWatches).HasForeignKey(x => x.UserId).OnDelete(DeleteBehavior.Cascade);
        builder.HasOne(x => x.Auction).WithMany().HasForeignKey(x => x.AuctionId).OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(x => new { x.UserId, x.AuctionId }).IsUnique();
    }
}
