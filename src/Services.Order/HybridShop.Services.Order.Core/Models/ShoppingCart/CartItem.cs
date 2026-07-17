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
        Quantity quantity,
        decimal price
    )
    {
        ProductId = productId;
        Quantity = quantity;
        Price = price;
    }

    [JsonPropertyName("productId")]
    public Guid ProductId { get; private set; }

    [JsonPropertyName("quantity")]
    public Quantity Quantity { get; private set; }

    [JsonPropertyName("price")]
    public decimal Price { get; private set; }

    public static CartItem NewCartItem(
        Guid productId,
        Quantity quantity,
        decimal price
    )
    {
        return new CartItem(productId, quantity, price);
    }

    public void UpdateQuantity(Quantity newQuantity)
    {
        Quantity = newQuantity;
    }
}