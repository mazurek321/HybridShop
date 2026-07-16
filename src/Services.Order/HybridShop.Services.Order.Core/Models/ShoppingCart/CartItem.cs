using System.Text.Json.Serialization;

namespace HybridShop.Services.Order.Core.Models.ShoppingCart;

public class CartItem
{
    public CartItem()
    {
        Quantity = null!;
    }

    [JsonConstructor]
    private CartItem(
        Guid productId,
        Quantity quantity
    )
    {
        ProductId = productId;
        Quantity = quantity;
    }

    [JsonPropertyName("productId")]
    public Guid ProductId { get; private set; }

    [JsonPropertyName("quantity")]
    public Quantity Quantity { get; private set; }
    public static CartItem NewCartItem(
        Guid productId,
        Quantity quantity
    )
    {
        return new CartItem(productId, quantity);
    }

    public void UpdateQuantity(Quantity newQuantity)
    {
        Quantity = newQuantity;
    }
}