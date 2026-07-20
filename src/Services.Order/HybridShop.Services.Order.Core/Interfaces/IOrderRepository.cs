using HybridShop.Services.Order.Core.Models.Order;

namespace HybridShop.Services.Order.Core.Interfaces;

public interface IOrderRepository
{
    Task AddAsync(Models.Order.Order order, CancellationToken cancellationToken = default);
    Task<Models.Order.Order?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IEnumerable<Models.Order.Order>> GetByBuyerIdAsync(Guid buyerId, CancellationToken cancellationToken = default);
    Task<IEnumerable<Models.Order.Order>> GetBySellerIdAsync(Guid sellerId, CancellationToken cancellationToken = default);
    Task UpdateAsync(Models.Order.Order order, CancellationToken cancellationToken = default);
}