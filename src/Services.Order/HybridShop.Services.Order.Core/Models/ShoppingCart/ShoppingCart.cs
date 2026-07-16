using System.Text.Json.Serialization;

namespace HybridShop.Services.Order.Core.Models.ShoppingCart;

public class ShoppingCart
{
    public ShoppingCart(){}

    [JsonConstructor]
    private ShoppingCart(
        Guid userId,
        List<CartItem>? items = null
    )
    {
        UserId = userId;
        Items = items ?? new List<CartItem>();
    }
    
    [JsonPropertyName("userId")]
    public Guid UserId { get; private set;}
    [JsonPropertyName("items")]
    public List<CartItem> Items { get; private set; } = new();

    public static ShoppingCart NewShoppingCart(Guid userId)
    {
        return new ShoppingCart(userId);
    }

    public void AddItem(
        Guid productId,
        Quantity quantity
    )
    {
        var existingItem = Items.FirstOrDefault(i => i.ProductId == productId);

        if(existingItem is not null)
            existingItem.UpdateQuantity(quantity);
        else
            Items.Add(CartItem.NewCartItem(productId, quantity));
    }

    public void RemoveItem(Guid productId)
    {
        Items.RemoveAll(i => i.ProductId == productId);
    }
}