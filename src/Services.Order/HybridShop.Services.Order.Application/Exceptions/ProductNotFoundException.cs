namespace HybridShop.Services.Order.Application.Exceptions;

public class ProductNotFoundException : Exception
{
    public ProductNotFoundException(Guid productId)  
        : base($"Product with id {productId} not found.")
    {}
}