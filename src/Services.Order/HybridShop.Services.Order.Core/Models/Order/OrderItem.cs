using System.Text.Json.Serialization;

namespace HybridShop.Services.Order.Core.Models.Order;

public class OrderItem
{
    public OrderItem() {}

    private OrderItem(Guid id, Guid productId, string title, int quantity, decimal price, Guid sellerId, OrderStatus status)
    {
        Id = id;
        ProductId = productId;
        Title = title;
        Quantity = quantity;
        Price = price;
        SellerId = sellerId;
        Status = status;
    }

    public Guid Id { get; private set; }
    public Guid ProductId { get; private set; }
    public string Title { get; private set; } = string.Empty;
    public int Quantity { get; private set; }
    public decimal Price { get; private set; }
    public Guid SellerId { get; private set; }
    public OrderStatus Status { get; private set; }

    public static OrderItem AddOrderItem(
        Guid productId, string title, int quantity, decimal price, Guid sellerId
    )
    {
        return new OrderItem(Guid.NewGuid(), productId, title, quantity, price, sellerId, OrderStatus.Placed);
    }

    public void UpdateStatus(OrderStatus status)
    {
        Status = status;
    }
}