using HybridShop.Services.Order.Application.Services;
using Microsoft.Extensions.DependencyInjection;

namespace HybridShop.Services.Order.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<ShoppingCartService>();
        return services;
    }
}