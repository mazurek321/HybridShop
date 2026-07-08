using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using HybridShop.Services.Auth.Core.Interfaces;
using HybridShop.Services.Auth.Infrastructure.Data;
using HybridShop.Services.Auth.Infrastructure.Repositories;

namespace HybridShop.Services.Auth.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<AuthDbContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString("DefaultConnection")));

        services.AddScoped<IUserRepository, UserRepository>();
        
        return services;
    }
}