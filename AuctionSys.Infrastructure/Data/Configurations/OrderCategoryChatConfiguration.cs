using AuctionSys.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AuctionSys.Infrastructure.Data.Configurations;

public class OrderConfiguration : IEntityTypeConfiguration<Order>
{
    public void Configure(EntityTypeBuilder<Order> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.TotalPrice).HasPrecision(18, 2);
        builder.Property(x => x.Status).HasConversion<string>();

        builder.HasOne(x => x.Buyer).WithMany(u => u.Orders).HasForeignKey(x => x.BuyerId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(x => x.Item).WithMany(i => i.Orders).HasForeignKey(x => x.ItemId).OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(x => x.BuyerId);
    }
}

public class CategoryConfiguration : IEntityTypeConfiguration<Category>
{
    public void Configure(EntityTypeBuilder<Category> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Name).IsRequired().HasMaxLength(100);
        builder.HasIndex(x => x.Name).IsUnique();
    }
}

public class ChatMessageConfiguration : IEntityTypeConfiguration<ChatMessage>
{
    public void Configure(EntityTypeBuilder<ChatMessage> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Content).IsRequired().HasMaxLength(2000);

        builder.HasOne(x => x.Sender).WithMany(u => u.SentMessages).HasForeignKey(x => x.SenderId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(x => x.Receiver).WithMany(u => u.ReceivedMessages).HasForeignKey(x => x.ReceiverId).OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(x => new { x.SenderId, x.ReceiverId });
        builder.HasIndex(x => x.SentAt);
    }
}
