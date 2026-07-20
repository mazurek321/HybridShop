namespace HybridShop.BuildingBlocks.EventBus.Events;

public record UserRegisteredEvent(
    Guid UserId,
    string Email,
    string Name,
    string Lastname
);