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
        DeliveryMethod? delivery = null
    )
    {
        UserId = userId;
        Items = items ?? new List<CartItem>();
        Delivery = delivery;
        Total = total;
    }
    
    [JsonPropertyName("userId")]
    public Guid UserId { get; private set;}

    [JsonPropertyName("items")]
    public List<CartItem> Items { get; private set; } = new();

    [JsonPropertyName("delivery")]
    public DeliveryMethod? Delivery { get; private set; }

    [JsonPropertyName("total")]
    public decimal Total { get; private set; }

    public static ShoppingCart NewShoppingCart(Guid userId)
    {
        return new ShoppingCart(userId);
    }

    public void AddItem(Guid productId, Quantity quantity, decimal price)
    {
        var existingItem = Items.FirstOrDefault(i => i.ProductId == productId);

        if(existingItem is not null)
            existingItem.UpdateQuantity(quantity);
        else
            Items.Add(CartItem.NewCartItem(productId, quantity, price));

        RecalculateTotal();
    }

    public void RemoveItem(Guid productId)
    {
        Items.RemoveAll(i => i.ProductId == productId);
        RecalculateTotal();
    }

    public void SetDeliveryMethod(DeliveryMethod delivery)
    {
        Delivery = delivery;
        RecalculateTotal();
    }

    private void RecalculateTotal()
    {
        var itemsTotal = Items.Sum(i => i.Price * i.Quantity.Value);
        var deliveryCost = Delivery?.Price ?? 0m;

        Total = itemsTotal + deliveryCost;
    }
}