using HybridShop.Services.Order.Core.Interfaces;
using HybridShop.Services.Order.Core.Models.Outbox;
using Microsoft.EntityFrameworkCore;

namespace HybridShop.Services.Order.Infrastructure.Repositories;

public class OutboxRepository : IOutboxRepository
{
    private readonly OrderDbContext _dbContext;

    public OutboxRepository(OrderDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task AddAsync(OutboxMessage message, CancellationToken cancellationToken = default)
    {
        await _dbContext.OutboxMessages.AddAsync(message, cancellationToken);
    }

    public async Task<List<OutboxMessage>> GetUnprocessedMessagesAsync(int batchSize, CancellationToken cancellationToken = default)
    {
        return await _dbContext.OutboxMessages
            .Where(m => m.ProcessedAt == null)
            .OrderBy(m => m.CreatedAt)
            .Take(batchSize)
            .ToListAsync(cancellationToken);
    }
}