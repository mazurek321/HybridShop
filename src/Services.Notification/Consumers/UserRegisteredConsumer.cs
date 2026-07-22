using MassTransit;
using HybridShop.BuildingBlocks.EventBus.Events;
using HybridShop.Services.Notification.Services;

namespace HybridShop.Services.Notification.Consumers;

public class UserRegisteredConsumer : IConsumer<UserRegisteredEvent>
{
    private readonly IEmailService _emailService;

    public UserRegisteredConsumer(IEmailService emailService)
    {
        _emailService = emailService;
    }

    public async Task Consume(ConsumeContext<UserRegisteredEvent> context)
    {
        var message = context.Message;

        if (!string.IsNullOrEmpty(message.Email))
        {
            var subject = "Witaj w HybridShop!";
            var body = $"""
                <div style="font-family: Arial, sans-serif; padding: 20px;">
                    <h2>Cześć {message.Name}!</h2>
                    <p>Dziękujemy za dołączenie do <strong>HybridShop</strong>.</p>
                    <p>Twoje konto z adresem <strong>{message.Email}</strong> zostało pomyślnie utworzone.</p>
                    <br/>
                    <p>Życzymy udanych zakupów!</p>
                </div>
            """;

            await _emailService.SendEmailAsync(message.Email, subject, body, context.CancellationToken);
        }
    }
}