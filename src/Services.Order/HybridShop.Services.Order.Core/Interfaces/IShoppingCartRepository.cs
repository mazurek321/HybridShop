using HybridShop.Services.Order.Core.Models.ShoppingCart;

namespace HybridShop.Services.Order.Core.Interfaces;
public interface IShoppingCartRepository
{
    Task<ShoppingCart?> GetCartAsync(Guid userId);
    Task<ShoppingCart> UpdateCartAsync(ShoppingCart cart);
    Task DeleteCartAsync(Guid userId);
}