namespace HybridShop.Services.Notification.Hubs;

public interface INotificationClient
{
    Task ReceiveOrderNotification(OrderNotificationDto notification);
}