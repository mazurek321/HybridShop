using HybridShop.Services.Product.Core.Product;

namespace HybridShop.Services.Product.Core.Interfaces;

public interface IProductRepository
{   
    Task AddAsync(Core.Product.Product product, CancellationToken cancellationToken = default);
    Task<Core.Product.Product?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Core.Product.Product?> GetBySkuIdAsync(Guid skuId, CancellationToken cancellationToken = default);
    Task<IEnumerable<Core.Product.Product>> BrowseProductsAsync(
        int skip, 
        int take, 
        PCategory? category = null, 
        decimal? priceFrom = null, 
        decimal? priceTo = null, 
        string? search = null,
        bool ignoreQueryFilters = false,
        CancellationToken cancellationToken = default
    );
    Task UpdateAsync(Core.Product.Product product, CancellationToken cancellationToken = default);
    Task DeleteAsync(Core.Product.Product product, CancellationToken cancellationToken = default);
}