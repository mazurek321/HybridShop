using System.Text.Json;
using System.Text.Json.Serialization;
using HybridShop.Services.Product.Core.Interfaces;
using HybridShop.Services.Product.Core.Product;
using Microsoft.Extensions.Caching.Distributed;

namespace HybridShop.Services.Product.Infrastructure.Repositories;

public class CachedProductRepository : IProductRepository
{
    private readonly IProductRepository _decorated;
    private readonly IDistributedCache _cache;
    private static readonly JsonSerializerOptions SerializerOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        IncludeFields = true
    };

    public CachedProductRepository(IProductRepository decorated, IDistributedCache cache)
    {
        _decorated = decorated;
        _cache = cache;
    }

    public async Task AddAsync(Core.Product.Product product, CancellationToken cancellationToken = default)
    {
        await _decorated.AddAsync(product, cancellationToken);
    }

    public async Task<Core.Product.Product?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        string cacheKey = $"products:id:{id}";
        string? cachedProduct = await _cache.GetStringAsync(cacheKey, cancellationToken);

        if (!string.IsNullOrEmpty(cachedProduct))
        {
            try
            {
                var deserialized = JsonSerializer.Deserialize<Core.Product.Product>(cachedProduct, SerializerOptions);
                if (deserialized is not null) return deserialized;
            }
            catch
            {
                await _cache.RemoveAsync(cacheKey, cancellationToken);
            }
        }

        var product = await _decorated.GetByIdAsync(id, cancellationToken);

        if (product is not null)
        {
            var options = new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(15)
            };

            await _cache.SetStringAsync(cacheKey, JsonSerializer.Serialize(product, SerializerOptions), options, cancellationToken);
        }

        return product;
    }

    public async Task<IEnumerable<Core.Product.Product>> GetByIdsAsync(
        IEnumerable<Guid> ids, 
        CancellationToken cancellationToken = default)
    {
        var distinctIds = ids.Distinct().ToList();
        var results = new List<Core.Product.Product>();
        var missingIds = new List<Guid>();

        foreach (var id in distinctIds)
        {
            var cached = await GetByIdAsync(id, cancellationToken);
            if (cached is not null)
                results.Add(cached);
            else
                missingIds.Add(id);
        }

        if (missingIds.Any())
        {
            var fetched = await _decorated.GetByIdsAsync(missingIds, cancellationToken);
            results.AddRange(fetched);
        }

        return results;
    }

    public async Task<Core.Product.Product?> GetBySkuIdAsync(Guid skuId, CancellationToken cancellationToken = default)
    {
        string cacheKey = $"products:sku:{skuId}";
        string? cachedProduct = await _cache.GetStringAsync(cacheKey, cancellationToken);

        if (!string.IsNullOrEmpty(cachedProduct))
        {
            try
            {
                var deserialized = JsonSerializer.Deserialize<Core.Product.Product>(cachedProduct, SerializerOptions);
                if (deserialized is not null) return deserialized;
            }
            catch
            {
                await _cache.RemoveAsync(cacheKey, cancellationToken);
            }
        }

        var product = await _decorated.GetBySkuIdAsync(skuId, cancellationToken);

        if (product is not null)
        {
            var options = new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(15)
            };

            await _cache.SetStringAsync(cacheKey, JsonSerializer.Serialize(product, SerializerOptions), options, cancellationToken);
        }

        return product;
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
        return await _decorated.BrowseProductsAsync(skip, take, category, priceFrom, priceTo, search, ignoreQueryFilters, cancellationToken);
    }

    public async Task UpdateAsync(Core.Product.Product product, CancellationToken cancellationToken = default)
    {
        await _decorated.UpdateAsync(product, cancellationToken);
        await InvalidateCacheAsync(product, cancellationToken);
    }

    public async Task DeleteAsync(Core.Product.Product product, CancellationToken cancellationToken = default)
    {
        await _decorated.DeleteAsync(product, cancellationToken);
        await InvalidateCacheAsync(product, cancellationToken);
    }

    private async Task InvalidateCacheAsync(Core.Product.Product product, CancellationToken cancellationToken)
    {
        await _cache.RemoveAsync($"products:id:{product.Id}", cancellationToken);
        
        if (product.Variants is not null)
        {
            foreach (var variant in product.Variants)
            {
                if (variant.SkuId != Guid.Empty)
                {
                    await _cache.RemoveAsync($"products:sku:{variant.SkuId}", cancellationToken);
                }
            }
        }
    }
}