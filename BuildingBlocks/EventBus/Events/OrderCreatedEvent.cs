namespace HybridShop.BuildingBlocks.EventBus.Events;

public record OrderCreatedEvent(
    Guid OrderId,
    Guid BuyerId,
    string BuyerEmail,
    decimal TotalAmount
);