namespace HybridShop.Services.Product.Core.Product;

public class Price
{
    public Price(decimal value)
    {
        if(value < 0)
            throw new Exception("Invalid price.");
        
        Value = value;
    }
    public decimal Value { get; private set; }
}