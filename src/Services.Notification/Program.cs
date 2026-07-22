using MassTransit;
using HybridShop.BuildingBlocks.EventBus;
using HybridShop.Services.Notification.Consumers;
using HybridShop.Grpc;
using HybridShop.BuildingBlocks.OpenApi;
using HybridShop.BuildingBlocks.OpenApi.Auth;
using HybridShop.Services.Notification.Hubs;
using HybridShop.Services.Notification.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(options =>
{
    options.AddPolicy("SignalRCors", policy =>
    {
        policy.SetIsOriginAllowed(_ => true) 
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});

builder.Services.AddAuthServices(builder.Configuration);

builder.Services.Configure<JwtBearerOptions>(JwtBearerDefaults.AuthenticationScheme, options =>
{
    options.Events = new JwtBearerEvents
    {
        OnMessageReceived = context =>
        {
            var accessToken = context.Request.Query["access_token"];
            var path = context.HttpContext.Request.Path;

            if (!string.IsNullOrEmpty(accessToken) && path.StartsWithSegments("/hubs/notifications"))
            {
                context.Token = accessToken;
            }

            return Task.CompletedTask;
        }
    };
});

builder.Services.AddSignalR();
builder.Services.AddTransient<IEmailService, EmailService>();

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

app.UseCors("SignalRCors");

app.UseAuthentication(); 
app.UseAuthorization();

app.MapHub<NotificationHub>("/hubs/notifications");

app.Run();