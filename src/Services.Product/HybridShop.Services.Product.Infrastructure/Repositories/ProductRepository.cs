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

    public async Task AddAsync(Core.Product.Product product)
    {
        await _dbContext.InsertOneAsync(product);
    }

    public async Task<Core.Product.Product?> GetByIdAsync(Guid id)
    {
        var filter = Builders<Core.Product.Product>.Filter.Eq(p => p.Id, id);

        return await _dbContext.Find(filter).FirstOrDefaultAsync();
    }

    public async Task<IEnumerable<Core.Product.Product>> BrowseProductsAsync(
        int skip, 
        int take, 
        PCategory? category = null, 
        decimal? priceFrom = null, 
        decimal? priceTo = null, 
        string? search = null,
        bool ignoreQueryFilters = false)
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
            .ToListAsync();
    }

    public async Task UpdateAsync(Core.Product.Product product){
        var filter = Builders<Core.Product.Product>.Filter.Eq(p => p.Id, product.Id);
        await _dbContext.ReplaceOneAsync(filter, product);
    }
    public async Task DeleteAsync(Core.Product.Product product){
        var filter = Builders<Core.Product.Product>.Filter.Eq(p => p.Id, product.Id);
        await _dbContext.DeleteOneAsync(filter);
    }
}