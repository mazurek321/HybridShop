using HybridShop.Services.Auth.Application.Interfaces;
using HybridShop.Services.Auth.Application.Services;
using Microsoft.Extensions.DependencyInjection;

namespace HybridShop.Services.Auth.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<TokenService>();
        services.AddScoped<AuthService>();
        services.AddScoped<IUserContext, UserContext>();
        services.AddScoped<UserService>();
        return services;
    }
}