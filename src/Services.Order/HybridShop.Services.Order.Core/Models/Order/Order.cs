namespace HybridShop.Services.Order.Core.Models.Order;

public class Order
{
    public Order() {}

    private Order(
        Guid id,
        Guid buyerId,
        Guid sellerId,
        List<OrderItem> items,
        decimal deliveryPrice,
        string deliveryName,
        decimal total,
        string shippingAddress,
        OrderStatus status,
        DateTime createdAt
    )
    {
        Id = id;
        BuyerId = buyerId;
        SellerId = sellerId;
        Items = items;
        DeliveryPrice = deliveryPrice;
        DeliveryName = deliveryName;
        Total = total;
        ShippingAddress = shippingAddress;
        Status = status;
        CreatedAt = createdAt;
    }

    public Guid Id { get; private set; }
    public Guid BuyerId { get; private set; }
    public Guid SellerId { get; private set; }
    public List<OrderItem> Items { get; private set; } = new();
    public decimal DeliveryPrice { get; private set; }
    public string DeliveryName { get; private set; } = string.Empty;
    public decimal Total { get; private set; }
    public string ShippingAddress { get; private set; } = string.Empty;
    public OrderStatus Status { get; private set; }
    public DateTime CreatedAt { get; private set; }

    public static Order Create(
        Guid buyerId,
        Guid sellerId,
        List<OrderItem> items,
        decimal deliveryPrice,
        string deliveryName,
        string shippingAddress
    )
    {
        var total = items.Sum(i => i.Price * i.Quantity) + deliveryPrice;
        return new Order(Guid.NewGuid(), buyerId, sellerId, items, deliveryPrice, deliveryName, total, shippingAddress, OrderStatus.Placed, DateTime.UtcNow);
    }

    public void UpdateStatus(OrderStatus status)
    {
        Status = status;
    }
}