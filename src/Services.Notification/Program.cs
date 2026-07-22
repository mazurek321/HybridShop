using MassTransit;
using HybridShop.BuildingBlocks.EventBus;
using HybridShop.Services.Notification.Consumers;
using HybridShop.Grpc;
using HybridShop.BuildingBlocks.OpenApi;
using HybridShop.BuildingBlocks.OpenApi.Auth;
using HybridShop.Services.Notification.Hubs;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddAuthServices(builder.Configuration);
builder.Services.AddSignalR();

builder.Services.AddEventBus(builder.Configuration, x =>
{
    x.AddConsumer<UserRegisteredConsumer>();
    x.AddConsumer<OrderCreatedConsumer>();
});

builder.Services.AddGrpcClient<UserGrpcService.UserGrpcServiceClient>(options =>
{
    var authUrl = builder.Configuration["InternalServices:AuthGrpcUrl"] 
                  ?? builder.Configuration["Services:AuthUrl"] 
                  ?? "http://auth-service:8081";

    options.Address = new Uri(authUrl);
});

var app = builder.Build();

app.UseAuthentication(); 
app.UseAuthorization();

app.MapHub<NotificationHub>("/hubs/notifications");

app.Run();