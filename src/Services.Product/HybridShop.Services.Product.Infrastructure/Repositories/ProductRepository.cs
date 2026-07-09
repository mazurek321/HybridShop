using HybridShop.Services.Product.Core.Interface;
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
}