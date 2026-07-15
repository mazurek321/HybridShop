namespace HybridShop.Services.Product.Core.Exceptions;
public class InvalidProductStatusException : Exception
{
    public InvalidProductStatusException() 
        : base($"Invalid product status.")
    {}
}