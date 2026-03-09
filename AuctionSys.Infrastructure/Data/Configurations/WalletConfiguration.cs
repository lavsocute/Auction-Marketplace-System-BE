using AuctionSys.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AuctionSys.Infrastructure.Data.Configurations;

public class WalletConfiguration : IEntityTypeConfiguration<Wallet>
{
    public void Configure(EntityTypeBuilder<Wallet> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Balance).HasPrecision(18, 2);
        builder.Property(x => x.FrozenBalance).HasPrecision(18, 2);
        builder.Property(x => x.Signature).IsRequired().HasMaxLength(150);

        builder.HasOne(x => x.User).WithOne(u => u.Wallet).HasForeignKey<Wallet>(x => x.UserId).OnDelete(DeleteBehavior.Cascade);
        builder.HasIndex(x => x.UserId).IsUnique();
    }
}

public class WalletTransactionConfiguration : IEntityTypeConfiguration<WalletTransaction>
{
    public void Configure(EntityTypeBuilder<WalletTransaction> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Amount).HasPrecision(18, 2);
        builder.Property(x => x.Type).HasConversion<string>();
        builder.Property(x => x.Description).HasMaxLength(500);

        builder.HasOne(x => x.Wallet).WithMany(w => w.Transactions).HasForeignKey(x => x.WalletId).OnDelete(DeleteBehavior.Cascade);
        builder.HasIndex(x => x.WalletId);
        builder.HasIndex(x => x.CreatedAt);
    }
}
