using HybridShop.Services.Order.Core.Models.Outbox;

namespace HybridShop.Services.Order.Core.Interfaces;

public interface IOutboxRepository
{
    Task AddAsync(OutboxMessage message, CancellationToken cancellationToken = default);
    Task<List<OutboxMessage>> GetUnprocessedMessagesAsync(int batchSize, CancellationToken cancellationToken = default);
}