namespace AuctionSys.Domain.Entities;

public class Category
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? IconUrl { get; set; }

    public ICollection<Item> Items { get; set; } = new List<Item>();

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
