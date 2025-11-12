namespace App.Domain.Entities;

public sealed class Item
{
    public Guid Id { get; private set; }

    public string Name { get; private set; } = null!;

    public decimal Price { get; private set; }

    public DateTime CreatedAt { get; private set; }

    private Item()
    {
    }

    public Item(Guid id, string name, decimal price, DateTime createdAt)
    {
        Id = id;
        Name = name;
        Price = price;
        CreatedAt = createdAt;
    }

    public void Update(string name, decimal price)
    {
        Name = name;
        Price = price;
    }
}
