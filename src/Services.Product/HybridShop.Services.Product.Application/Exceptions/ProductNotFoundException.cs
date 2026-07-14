namespace HybridShop.Services.Product.Application.Exceptions;
public class ProductNotFoundException : Exception
{
    public ProductNotFoundException() 
        : base($"This product cannot be found.")
    {}
}