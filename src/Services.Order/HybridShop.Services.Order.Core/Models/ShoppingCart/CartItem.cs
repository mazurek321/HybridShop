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
        Guid? skuId,
        Quantity quantity,
        decimal price,
        Guid sellerId
    )
    {
        ProductId = productId;
        SkuId = skuId;
        Quantity = quantity;
        Price = price;
        SellerId = sellerId;
    }

    [JsonPropertyName("productId")]
    public Guid ProductId { get; private set; }

    [JsonPropertyName("skuId")]
    public Guid? SkuId { get; private set; }

    [JsonPropertyName("quantity")]
    public Quantity Quantity { get; private set; }

    [JsonPropertyName("price")]
    public decimal Price { get; private set; }
    
    [JsonPropertyName("sellerId")]
    public Guid SellerId { get; private set; }

    public static CartItem NewCartItem(
        Guid productId,
        Guid? skuId,
        Quantity quantity,
        decimal price,
        Guid sellerId
    )
    {
        return new CartItem(productId, skuId, quantity, price, sellerId);
    }

    public void UpdateQuantity(Quantity newQuantity)
    {
        Quantity = newQuantity;
    }
}