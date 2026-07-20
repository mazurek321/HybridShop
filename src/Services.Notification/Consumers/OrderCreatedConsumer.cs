using MassTransit;
using HybridShop.BuildingBlocks.EventBus.Events;

namespace HybridShop.Services.Notification.Consumers;

public class OrderCreatedConsumer : IConsumer<OrderCreatedEvent>
{
    public async Task Consume(ConsumeContext<OrderCreatedEvent> context)
    {
        var message = context.Message;
        await Task.CompletedTask;
    }
}