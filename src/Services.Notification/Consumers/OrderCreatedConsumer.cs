using MassTransit;
using HybridShop.BuildingBlocks.EventBus.Events;
using HybridShop.Services.Notification.Hubs;
using Microsoft.AspNetCore.SignalR;

namespace HybridShop.Services.Notification.Consumers;

public class OrderCreatedConsumer : IConsumer<OrderCreatedEvent>
{
    private readonly IHubContext<NotificationHub> _hubContext;

    public OrderCreatedConsumer(IHubContext<NotificationHub> hubContext)
    {
        _hubContext = hubContext;
    }

    public async Task Consume(ConsumeContext<OrderCreatedEvent> context)
    {
        var message = context.Message;

        await _hubContext.Clients
            .User(message.SellerId.ToString())
            .SendAsync("ReceiveOrderNotification", new
            {
                OrderId = message.OrderId,
                BuyerEmail = message.BuyerEmail,
                Total = message.TotalAmount,
                CreatedAt = DateTime.UtcNow
            });
    }
}