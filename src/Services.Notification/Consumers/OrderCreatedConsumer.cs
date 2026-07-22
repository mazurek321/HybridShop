using MassTransit;
using HybridShop.BuildingBlocks.EventBus.Events;
using HybridShop.Services.Notification.Hubs;
using Microsoft.AspNetCore.SignalR;

namespace HybridShop.Services.Notification.Consumers;

public class OrderCreatedConsumer : IConsumer<OrderCreatedEvent>
{
    private readonly IHubContext<NotificationHub, INotificationClient> _hubContext;

    public OrderCreatedConsumer(IHubContext<NotificationHub, INotificationClient> hubContext)
    {
        _hubContext = hubContext;
    }

    public async Task Consume(ConsumeContext<OrderCreatedEvent> context)
    {
        var message = context.Message;

        var notification = new OrderNotificationDto(
            message.OrderId,
            message.BuyerEmail,
            message.TotalAmount,
            DateTime.UtcNow
        );

        await _hubContext.Clients
            .User(message.SellerId.ToString())
            .ReceiveOrderNotification(notification);
    }
}