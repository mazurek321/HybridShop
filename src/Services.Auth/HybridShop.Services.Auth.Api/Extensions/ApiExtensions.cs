using Microsoft.OpenApi;

namespace HybridShop.Services.Auth.Api.Extensions;

public static class ApiExtensions
{
    public static IServiceCollection AddApiServices(this IServiceCollection services)
    {
        services.AddEndpointsApiExplorer();

        services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "HybridShop Auth API",
                Version = "v1"
            });

            options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Name = "Authorization",
                Description = "Wpisz token w formacie: Bearer {twój_token}",
                In = ParameterLocation.Header,
                Type = SecuritySchemeType.ApiKey,
                Scheme = "Bearer"
            });

            var requirement = new OpenApiSecurityRequirement();
            var schemeReference = new OpenApiSecuritySchemeReference("Bearer");
            requirement.Add(schemeReference, new List<string>());

            options.AddSecurityRequirement(document => requirement);
        });

        return services;
    }
}