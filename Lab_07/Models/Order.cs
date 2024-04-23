namespace Lab_07.Models;

public class Order
{
    public Order()
    {
    }

    public Order(int productId, int amount, DateTime createdAt)
    {
        ProductId = productId;
        Amount = amount;
        CreatedAt = createdAt;
    }

    public int Id { get; set; }
    public int ProductId { get; set; }
    public int Amount { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? FulfilledAt { get; set; }
}