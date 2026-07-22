namespace HybridShop.Services.Notification.Hubs;

public record OrderNotificationDto(
    Guid OrderId,
    string BuyerEmail,
    decimal Total,
    DateTime CreatedAt
);