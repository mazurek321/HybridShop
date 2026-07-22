using HybridShop.Services.Product.Core.Interfaces;
using HybridShop.Services.Product.Core.Product;
using MongoDB.Driver;

namespace HybridShop.Services.Product.Infrastructure;

public class ProductRepository : IProductRepository
{
    private readonly IMongoCollection<Core.Product.Product> _dbContext;

    public ProductRepository(IMongoDatabase dbContext)
    {
        _dbContext = dbContext.GetCollection<Core.Product.Product>("products");
    }

    public async Task AddAsync(Core.Product.Product product, CancellationToken cancellationToken = default)
    {
        await _dbContext.InsertOneAsync(product, cancellationToken: cancellationToken);
    }

    public async Task<Core.Product.Product?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var builder = Builders<Core.Product.Product>.Filter;
        var filter = builder.Eq(p => p.Id, id) & builder.Eq(p => p.IsDeleted, false);

        return await _dbContext.Find(filter).FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<IEnumerable<Core.Product.Product>> GetByIdsAsync(
        IEnumerable<Guid> ids, 
        CancellationToken cancellationToken = default)
    {
        var distinctIds = ids.Distinct().ToList();
        if (!distinctIds.Any())
            return Enumerable.Empty<Core.Product.Product>();

        var builder = Builders<Core.Product.Product>.Filter;
        var filter = builder.In(p => p.Id, distinctIds) & builder.Eq(p => p.IsDeleted, false);

        return await _dbContext.Find(filter).ToListAsync(cancellationToken);
    }

    public async Task<Core.Product.Product?> GetBySkuIdAsync(Guid skuId, CancellationToken cancellationToken = default)
    {
        var builder = Builders<Core.Product.Product>.Filter;
        var filter = builder.Eq(p => p.IsDeleted, false) & 
                     builder.ElemMatch(p => p.Variants, v => v.SkuId == skuId);

        return await _dbContext.Find(filter).FirstOrDefaultAsync(cancellationToken);
    }
    

    public async Task<IEnumerable<Core.Product.Product>> BrowseProductsAsync(
        int skip, 
        int take, 
        PCategory? category = null, 
        decimal? priceFrom = null, 
        decimal? priceTo = null, 
        string? search = null,
        bool ignoreQueryFilters = false,
        CancellationToken cancellationToken = default)
    {
        var builder = Builders<Core.Product.Product>.Filter;
        var filter = builder.Empty;

        if (!ignoreQueryFilters)
            filter &= builder.Eq(p => p.IsDeleted, false);

        if (category.HasValue)
            filter &= builder.Eq(p => p.Category.Value, category.Value);

        if (priceFrom.HasValue)
            filter &= builder.Gte(p => p.Price.Value, priceFrom.Value);

        if (priceTo.HasValue)
            filter &= builder.Lte(p => p.Price.Value, priceTo.Value);

        if (!string.IsNullOrWhiteSpace(search))
            filter &= builder.Regex(p => p.Title, new MongoDB.Bson.BsonRegularExpression(search, "i"));

        return await _dbContext.Find(filter)
            .Skip(skip)
            .Limit(take)
            .ToListAsync(cancellationToken);
    }

    public async Task UpdateAsync(Core.Product.Product product, CancellationToken cancellationToken = default)
    {
        var filter = Builders<Core.Product.Product>.Filter.Eq(p => p.Id, product.Id);
        await _dbContext.ReplaceOneAsync(filter, product, cancellationToken: cancellationToken);
    }

    public async Task DeleteAsync(Core.Product.Product product, CancellationToken cancellationToken = default)
    {
        var filter = Builders<Core.Product.Product>.Filter.Eq(p => p.Id, product.Id);
        await _dbContext.DeleteOneAsync(filter, cancellationToken);
    }
}