using HybridShop.Services.Order.Core.Models.ShoppingCart;

namespace HybridShop.Services.Order.Core.Interfaces;

public interface IShoppingCartRepository
{
    Task<ShoppingCart?> GetCartAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<ShoppingCart> UpdateCartAsync(ShoppingCart cart, CancellationToken cancellationToken = default);
    Task DeleteCartAsync(Guid userId, CancellationToken cancellationToken = default);
}