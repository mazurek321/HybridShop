using HybridShop.Services.Order.Core.Interfaces;
using Microsoft.EntityFrameworkCore;
using OrderAggregate = HybridShop.Services.Order.Core.Models.Order.Order;

namespace HybridShop.Services.Order.Infrastructure.Repositories;

public class OrderRepository : IOrderRepository
{
    private readonly OrderDbContext _context;

    public OrderRepository(OrderDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(OrderAggregate order, CancellationToken cancellationToken = default)
    {
        _context.Orders.Add(order);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task<OrderAggregate?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Orders
            .Include(o => o.Items)
            .FirstOrDefaultAsync(o => o.Id == id, cancellationToken);
    }

    public async Task<IEnumerable<OrderAggregate>> GetByBuyerIdAsync(Guid buyerId, CancellationToken cancellationToken = default)
    {
        return await _context.Orders
            .Include(o => o.Items)
            .Where(o => o.BuyerId == buyerId)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<OrderAggregate>> GetBySellerIdAsync(Guid sellerId, CancellationToken cancellationToken = default)
    {
        return await _context.Orders
            .Include(o => o.Items)
            .Where(o => o.SellerId == sellerId)
            .ToListAsync(cancellationToken);
    }

    public async Task UpdateAsync(OrderAggregate order, CancellationToken cancellationToken = default)
    {
        _context.Orders.Update(order);
        await _context.SaveChangesAsync(cancellationToken);
    }
}