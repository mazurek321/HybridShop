using HybridShop.Services.Product.Core.Interfaces;
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
        return await _dbContext
            .Find(p => p.Id == id && !p.IsDeleted)
            .FirstOrDefaultAsync();
    }

    public async Task<IEnumerable<Core.Product.Product>> BrowseProductsAsync(int skip, int take)
    {
        return await _dbContext.Find(FilterDefinition<Core.Product.Product>.Empty)
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