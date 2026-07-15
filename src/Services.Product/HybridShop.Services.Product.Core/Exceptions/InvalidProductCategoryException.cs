namespace HybridShop.Services.Product.Core.Exceptions;
public class InvalidProductException : Exception
{
    public InvalidProductException() 
        : base($"Invalid product category.")
    {}
}