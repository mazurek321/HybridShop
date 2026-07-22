using HybridShop.Services.Product.Application.Services;
using HybridShop.Services.Product.Core.Interfaces;
using HybridShop.Services.Product.Infrastructure.Services;
using Microsoft.Extensions.DependencyInjection;


namespace HybridShop.Services.Product.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<ProductService>();
        services.AddScoped<IFileStorageService, MinioStorageService>();
        return services;
    }
}