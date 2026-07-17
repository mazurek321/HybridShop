namespace HybridShop.Services.Order.Core.Interfaces;

public interface IOrderRepository
{
    Task AddAsync(Models.Order.Order order);
    Task<Models.Order.Order?> GetByIdAsync(Guid id);
    Task<IEnumerable<Models.Order.Order>> GetByBuyerIdAsync(Guid buyerId);
    Task<IEnumerable<Models.Order.Order>> GetBySellerIdAsync(Guid sellerId);
    Task UpdateAsync(Models.Order.Order order);
}