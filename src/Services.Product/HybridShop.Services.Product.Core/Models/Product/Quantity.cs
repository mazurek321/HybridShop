namespace HybridShop.Services.Product.Core.Product;
public class Quantity
{
    public Quantity(int value)
    {
        if(value < 0) 
            throw new Exception("Invalid quantity of items.");
        Value = value;
    }
    public int Value { get; private set; }
}