using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Caching.Distributed;
using MongoDB.Driver;
using HybridShop.Services.Product.Core.Interfaces;
using HybridShop.Services.Product.Infrastructure.Clients;
using HybridShop.Grpc;
using HybridShop.Services.Product.Infrastructure.Repositories;

namespace HybridShop.Services.Product.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        MongoMapping.Register();

        var mongoSection = configuration.GetSection("MongoSettings");
        var connectionString = mongoSection["ConnectionString"];
        var databaseName = mongoSection["DatabaseName"];

        var mongoClient = new MongoClient(connectionString);
        services.AddSingleton<IMongoClient>(mongoClient);
        services.AddScoped<IMongoDatabase>(sp => mongoClient.GetDatabase(databaseName));

        services.AddStackExchangeRedisCache(options =>
        {
            options.Configuration = configuration.GetConnectionString("Redis");
            options.InstanceName = "HybridShop_Product_";
        });

        AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", true);
        var authGrpcUrl = configuration["InternalServices:AuthGrpcUrl"];

        services.AddGrpcClient<UserGrpcService.UserGrpcServiceClient>(options =>
        {
            options.Address = new Uri(authGrpcUrl!);
        }).AddStandardResilienceHandler(options =>
        {
            options.Retry.MaxRetryAttempts = 3;
            options.Retry.Delay = TimeSpan.FromMilliseconds(500);
            options.Retry.BackoffType = Polly.DelayBackoffType.Exponential;

            options.AttemptTimeout.Timeout = TimeSpan.FromSeconds(3);

            options.CircuitBreaker.SamplingDuration = TimeSpan.FromSeconds(10);
            options.CircuitBreaker.FailureRatio = 0.5;
        });

        services.AddScoped<ProductRepository>();
        // services.AddScoped<IProductRepository>(sp =>
        //     new CachedProductRepository(
        //         sp.GetRequiredService<ProductRepository>(),
        //         sp.GetRequiredService<IDistributedCache>()
        //     ));
        services.AddScoped<IProductRepository, ProductRepository>();

        services.AddScoped<IUserServiceClient, UserServiceClient>();

        return services;
    }
}