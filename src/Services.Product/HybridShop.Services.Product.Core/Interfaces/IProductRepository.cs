namespace HybridShop.Services.Product.Core.Interfaces;
public interface IProductRepository
{   
    Task AddAsync(Core.Product.Product product);
    Task<Core.Product.Product?> GetByIdAsync(Guid id);
    Task<IEnumerable<Core.Product.Product>> BrowseProductsAsync(int skip, int take);
    Task UpdateAsync(Core.Product.Product product);
    Task DeleteAsync(Core.Product.Product product);
}