using HybridShop.Grpc;
using HybridShop.Services.Order.Core.Interfaces;
using HybridShop.Services.Order.Infrastructure.Clients;
using HybridShop.Services.Order.Infrastructure.Repositories;
using HybridShop.Services.Product.Grpc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

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

        var productGrpcUrl = configuration["InternalServices:ProductGrpcUrl"] 
                             ?? "http://product-service:8081";

        services.AddGrpcClient<ProductGrpcService.ProductGrpcServiceClient>(options =>
        {
            options.Address = new Uri(productGrpcUrl);
        }).AddStandardResilienceHandler(options =>
        {
            options.Retry.MaxRetryAttempts = 2;
            options.Retry.Delay = TimeSpan.FromMilliseconds(200);
            options.Retry.BackoffType = Polly.DelayBackoffType.Constant;
            options.AttemptTimeout.Timeout = TimeSpan.FromSeconds(10);
            options.TotalRequestTimeout.Timeout = TimeSpan.FromSeconds(15);
            options.CircuitBreaker.SamplingDuration = TimeSpan.FromSeconds(30);
            options.CircuitBreaker.FailureRatio = 0.9;
        });

        var authGrpcUrl = configuration["InternalServices:AuthGrpcUrl"] 
                          ?? "http://auth-service:8081";

        services.AddGrpcClient<UserGrpcService.UserGrpcServiceClient>(options =>
        {
            options.Address = new Uri(authGrpcUrl);
        }).AddStandardResilienceHandler(options =>
        {
            options.Retry.MaxRetryAttempts = 2;
            options.Retry.Delay = TimeSpan.FromMilliseconds(200);
            options.Retry.BackoffType = Polly.DelayBackoffType.Constant;
            options.AttemptTimeout.Timeout = TimeSpan.FromSeconds(10);
            options.TotalRequestTimeout.Timeout = TimeSpan.FromSeconds(15);
        });

        services.AddScoped<IShoppingCartRepository, RedisShoppingCartRepository>();
        services.AddScoped<IProductServiceClient, ProductServiceClient>();
        services.AddScoped<IOrderRepository, OrderRepository>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        return services;
    }
}