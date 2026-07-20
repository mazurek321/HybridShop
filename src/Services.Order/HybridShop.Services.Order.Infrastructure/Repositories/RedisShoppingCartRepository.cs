using System.Text.Json;
using HybridShop.Services.Order.Core.Interfaces;
using HybridShop.Services.Order.Core.Models.ShoppingCart;
using Microsoft.Extensions.Caching.Distributed;

namespace HybridShop.Services.Order.Infrastructure.Repositories;

public class RedisShoppingCartRepository : IShoppingCartRepository
{
    private readonly IDistributedCache _cache;

    private static readonly DistributedCacheEntryOptions CacheOptions = new()
    {
        AbsoluteExpirationRelativeToNow = TimeSpan.FromDays(7)
    };

    public RedisShoppingCartRepository(IDistributedCache cache)
    {
        _cache = cache;
    }

    public async Task<ShoppingCart?> GetCartAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var data = await _cache.GetStringAsync(userId.ToString(), cancellationToken);
        return string.IsNullOrEmpty(data) 
            ? null 
            : JsonSerializer.Deserialize<ShoppingCart>(data);
    }

    public async Task<ShoppingCart> UpdateCartAsync(ShoppingCart cart, CancellationToken cancellationToken = default)
    {
        var data = JsonSerializer.Serialize(cart);
        await _cache.SetStringAsync(cart.UserId.ToString(), data, CacheOptions, cancellationToken);
        return cart;
    }

    public async Task DeleteCartAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        await _cache.RemoveAsync(userId.ToString(), cancellationToken);
    }
}