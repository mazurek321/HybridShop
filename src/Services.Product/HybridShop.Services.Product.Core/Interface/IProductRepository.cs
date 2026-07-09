namespace HybridShop.Services.Product.Core.Interface;
public interface IProductRepository
{   
    Task AddAsync(Core.Product.Product product);
    Task<Core.Product.Product?> GetByIdAsync(Guid id);
}