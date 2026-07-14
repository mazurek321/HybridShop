using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;
using HybridShop.Services.Product.Core.Interfaces;
using HybridShop.Services.Product.Infrastructure.Clients;
using HybridShop.Grpc;

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


        AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", true);
        var authGrpcUrl = configuration["InternalServices:AuthGrpcUrl"];

        services.AddGrpcClient<UserGrpcService.UserGrpcServiceClient>(options =>
        {
            options.Address = new Uri(authGrpcUrl!);
        });

        services.AddScoped<IProductRepository, ProductRepository>();
        services.AddScoped<IUserServiceClient, UserServiceClient>();

        return services;
    }
}