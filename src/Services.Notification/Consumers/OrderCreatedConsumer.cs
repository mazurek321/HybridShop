using MassTransit;
using HybridShop.BuildingBlocks.EventBus.Events;
using HybridShop.Grpc;
using HybridShop.Services.Notification.Hubs;
using HybridShop.Services.Notification.Services;
using Microsoft.AspNetCore.SignalR;

namespace HybridShop.Services.Notification.Consumers;

public class OrderCreatedConsumer : IConsumer<OrderCreatedEvent>
{
    private readonly IHubContext<NotificationHub, INotificationClient> _hubContext;
    private readonly IEmailService _emailService;
    private readonly UserGrpcService.UserGrpcServiceClient _userGrpcClient;

    public OrderCreatedConsumer(
        IHubContext<NotificationHub, INotificationClient> hubContext,
        IEmailService emailService,
        UserGrpcService.UserGrpcServiceClient userGrpcClient
    )
    {
        _hubContext = hubContext;
        _emailService = emailService;
        _userGrpcClient = userGrpcClient;
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

        if (!string.IsNullOrEmpty(message.BuyerEmail))
        {
            var buyerSubject = $"Potwierdzenie zamówienia #{message.OrderId}";
            var buyerBody = $"""
                <h2>Dziękujemy za zakupy!</h2>
                <p>Twój numer zamówienia: <strong>{message.OrderId}</strong></p>
                <p>Łączna kwota: <strong>{message.TotalAmount} PLN</strong></p>
            """;

            await _emailService.SendEmailAsync(message.BuyerEmail, buyerSubject, buyerBody, context.CancellationToken);
        }

        try
        {
            var sellerUserResponse = await _userGrpcClient.GetUserDetailsAsync(
                new UserRequest { Id = message.SellerId.ToString() },
                cancellationToken: context.CancellationToken
            );

            if (sellerUserResponse is not null && !string.IsNullOrEmpty(sellerUserResponse.Email))
            {
                var sellerSubject = $"Nowe zamówienie #{message.OrderId}";
                var sellerBody = $"""
                    <h2>Masz nowe zamówienie!</h2>
                    <p>Numer zamówienia: <strong>{message.OrderId}</strong></p>
                    <p>Kupujący: {message.BuyerEmail}</p>
                    <p>Wartość: <strong>{message.TotalAmount} PLN</strong></p>
                """;

                await _emailService.SendEmailAsync(sellerUserResponse.Email, sellerSubject, sellerBody, context.CancellationToken);
            }
        }
        catch
        {
        }
    }
}