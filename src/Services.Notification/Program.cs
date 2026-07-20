using MassTransit;
using HybridShop.BuildingBlocks.EventBus;
using HybridShop.Services.Notification.Consumers;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEventBus(builder.Configuration, x =>
{
    x.AddConsumer<UserRegisteredConsumer>();
    x.AddConsumer<OrderCreatedConsumer>();
});

var app = builder.Build();
app.Run();