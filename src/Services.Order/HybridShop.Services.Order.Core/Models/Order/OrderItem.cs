using System.Net;
using System.Text.Json.Serialization;

namespace HybridShop.Services.Order.Core.Models.Order;

public class OrderItem
{
    public OrderItem() {}

    private OrderItem(Guid productId, string title, int quantity, decimal price)
    {
        ProductId = productId;
        Title = title;
        Quantity = quantity;
        Price = price;
    }

    public Guid ProductId { get; private set; }
    public string Title { get; private set; } = string.Empty;
    public int Quantity { get; private set; }
    public decimal Price { get; private set; }

    public static OrderItem AddOrderItem(
        Guid productId, string title, int quantity, decimal price
    )
    {
        return new OrderItem(productId, title, quantity, price);
    }
}