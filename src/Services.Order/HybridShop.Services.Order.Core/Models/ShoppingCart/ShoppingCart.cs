using System.Text.Json.Serialization;
using HybridShop.Services.Order.Core.Models.Delivery;

namespace HybridShop.Services.Order.Core.Models.ShoppingCart;

public class ShoppingCart
{
    public ShoppingCart(){}

    [JsonConstructor]
    private ShoppingCart(
        Guid userId,
        List<CartItem>? items = null,
        decimal total = 0,
        DeliveryMethod? delivery = null,
        Guid version = default
    )
    {
        UserId = userId;
        Items = items ?? new List<CartItem>();
        Delivery = delivery;
        Total = total;
        Version = version == default ? Guid.NewGuid() : version;
    }
    
    [JsonPropertyName("userId")]
    public Guid UserId { get; private set;}

    [JsonPropertyName("items")]
    public List<CartItem> Items { get; private set; } = new();

    [JsonPropertyName("delivery")]
    public DeliveryMethod? Delivery { get; private set; }

    [JsonPropertyName("total")]
    public decimal Total { get; private set; }

    [JsonPropertyName("version")]
    public Guid Version { get; private set; }

    public static ShoppingCart NewShoppingCart(Guid userId)
    {
        return new ShoppingCart(userId) { Version = Guid.NewGuid() };
    }

    public void AddItem(Guid productId, Guid? skuId, Quantity quantity, decimal price, Guid sellerId)
    {
        var existingItem = Items.FirstOrDefault(i => i.ProductId == productId && i.SkuId == skuId);
        if (existingItem is not null)
        {
            existingItem.UpdateQuantity(new Quantity(existingItem.Quantity.Value + quantity.Value));
        }
        else
        {
            Items.Add(CartItem.NewCartItem(productId, skuId, quantity, price, sellerId));
        }

        Version = Guid.NewGuid();
        RecalculateTotal();
    }

    public void RemoveItem(Guid productId)
    {
        Items.RemoveAll(i => i.ProductId == productId);
        Version = Guid.NewGuid();
        RecalculateTotal();
    }

    public void SetDeliveryMethod(DeliveryMethod delivery)
    {
        Delivery = delivery;
        Version = Guid.NewGuid();
        RecalculateTotal();
    }

    private void RecalculateTotal()
    {
        var itemsTotal = Items.Sum(i => i.Price * i.Quantity.Value);
        var deliveryCost = Delivery?.Price ?? 0m;

        Total = itemsTotal + deliveryCost;
    }
}