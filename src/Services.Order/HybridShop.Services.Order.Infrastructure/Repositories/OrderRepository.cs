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

    public async Task AddAsync(OrderAggregate order)
    {
        _context.Orders.Add(order);
        await _context.SaveChangesAsync();
    }

    public async Task<OrderAggregate?> GetByIdAsync(Guid id)
    {
        return await _context.Orders
            .Include(o => o.Items)
            .FirstOrDefaultAsync(o => o.Id == id);
    }

    public async Task<IEnumerable<OrderAggregate>> GetByBuyerIdAsync(Guid buyerId)
    {
        return await _context.Orders
            .Include(o => o.Items)
            .Where(o => o.BuyerId == buyerId)
            .ToListAsync();
    }

    public async Task<IEnumerable<OrderAggregate>> GetBySellerIdAsync(Guid sellerId)
    {
        return await _context.Orders
            .Include(o => o.Items)
            .Where(o => o.SellerId == sellerId)
            .ToListAsync();
    }

    public async Task UpdateAsync(OrderAggregate order)
    {
        _context.Orders.Update(order);
        await _context.SaveChangesAsync();
    }
}