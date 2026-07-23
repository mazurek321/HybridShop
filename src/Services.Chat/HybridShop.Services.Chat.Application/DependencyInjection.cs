using HybridShop.Services.Chat.Application.Services;
using Microsoft.Extensions.DependencyInjection;

namespace HybridShop.Services.Auth.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<ChatService>();
        return services;
    }
}