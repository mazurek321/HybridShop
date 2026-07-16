using HybridShop.Services.Order.Core.Interfaces;
using HybridShop.Services.Order.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using HybridShop.Services.Product.Grpc;
using HybridShop.Services.Order.Infrastructure.Services;


namespace HybridShop.Services.Order.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<OrderDbContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString("DefaultConnection")));

        services.AddStackExchangeRedisCache(options =>
        {
            options.Configuration = configuration.GetConnectionString("Redis");
        });

        var authGrpcUrl = configuration["InternalServices:ProductGrpcUrl"];
        services.AddGrpcClient<ProductGrpcService.ProductGrpcServiceClient>(options =>
        {
            options.Address = new Uri(authGrpcUrl!);
        });

        services.AddScoped<IShoppingCartRepository, RedisShoppingCartRepository>();
        services.AddScoped<IProductServiceClient, ProductServiceClient>();
        
        return services;
    }
}