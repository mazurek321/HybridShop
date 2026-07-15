using HybridShop.Services.Product.Core.Product;

namespace HybridShop.Services.Product.Core.Interfaces;
public interface IProductRepository
{   
    Task AddAsync(Core.Product.Product product);
    Task<Core.Product.Product?> GetByIdAsync(Guid id, bool ignoreQueryFilters = false);
    Task<IEnumerable<Core.Product.Product>> BrowseProductsAsync(
        int skip, 
        int take, 
        PCategory? category = null, 
        decimal? priceFrom = null, 
        decimal? priceTo = null, 
        string? search = null,
        bool ignoreQueryFilters = false
    );
    Task UpdateAsync(Core.Product.Product product);
    Task DeleteAsync(Core.Product.Product product);
}