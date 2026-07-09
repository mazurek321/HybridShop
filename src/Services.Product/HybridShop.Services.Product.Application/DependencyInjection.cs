using HybridShop.Services.Product.Application.Services;
using Microsoft.Extensions.DependencyInjection;

namespace HybridShop.Services.Product.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<ProductService>();
        return services;
    }
}