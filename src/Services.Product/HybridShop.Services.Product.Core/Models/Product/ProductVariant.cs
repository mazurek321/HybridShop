namespace HybridShop.Services.Product.Core.Product;

public class ProductVariant
{
    public ProductVariant()
    {
        Price = null!;
        Quantity = null!;
        Attributes = new Dictionary<string, object>();
    }

    public ProductVariant(
        Guid skuId,
        Price price,
        Quantity quantity,
        Dictionary<string, object> attributes
    )
    {
        SkuId = skuId;
        Price = price;
        Quantity = quantity;
        Attributes = attributes;
    }

    public Guid SkuId { get; private set; }
    public Price Price { get; private set; }
    public Quantity Quantity { get; private set; }
    public Dictionary<string, object> Attributes { get; private set; }
}