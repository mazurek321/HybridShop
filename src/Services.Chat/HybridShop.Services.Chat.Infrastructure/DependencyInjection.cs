using HybridShop.Grpc;
using HybridShop.Services.Chat.Core.Interfaces;
using HybridShop.Services.Chat.Infrastructure.Clients;
using HybridShop.Services.Chat.Infrastructure.Repositories;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;

namespace HybridShop.Services.Chat.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSingleton<IMongoClient>(sp =>
            new MongoClient(configuration.GetConnectionString("Mongo") ?? configuration["MONGO_CONNECTION_STRING"]));

        services.AddScoped<IMongoDatabase>(sp =>
            sp.GetRequiredService<IMongoClient>().GetDatabase("HybridShop_Chat"));

        services.AddGrpcClient<UserGrpcService.UserGrpcServiceClient>(options =>
        {
            var authUrl = configuration["InternalServices:AuthGrpcUrl"] 
                          ?? configuration["Services:AuthUrl"] 
                          ?? "http://auth-service:8081";

            options.Address = new Uri(authUrl);
        });

        services.AddScoped<IChatRepository, ChatRepository>();
        services.AddScoped<IUserGrpcClient, UserGrpcClient>();

        return services;
    }
}