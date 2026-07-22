namespace HybridShop.BuildingBlocks.EventBus.Events;

public record OrderCreatedEvent(
    Guid OrderId,
    Guid BuyerId,
    Guid SellerId,
    string BuyerEmail,
    decimal TotalAmount
);