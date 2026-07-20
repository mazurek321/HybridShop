using MassTransit;
using HybridShop.BuildingBlocks.EventBus.Events;

namespace HybridShop.Services.Notification.Consumers;

public class UserRegisteredConsumer : IConsumer<UserRegisteredEvent>
{
    public async Task Consume(ConsumeContext<UserRegisteredEvent> context)
    {
        var message = context.Message;
        await Task.CompletedTask;
    }
}