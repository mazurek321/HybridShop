using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.OpenApi;

namespace HybridShop.Services.Auth.Api.Extensions;

public static class ApiExtensions
{
    public static IServiceCollection AddApiServices(this IServiceCollection services)
    {
        services.AddEndpointsApiExplorer();

        services.AddOpenApi(options =>
        {
            options.AddDocumentTransformer((document, context, cancellationToken) =>
            {
                document.Servers?.Clear();

                var scheme = new OpenApiSecurityScheme
                {
                    Type = SecuritySchemeType.Http,
                    Scheme = JwtBearerDefaults.AuthenticationScheme,
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header
                };

                document.Components ??= new OpenApiComponents();
                document.Components.SecuritySchemes ??= new Dictionary<string, IOpenApiSecurityScheme>();
                document.Components.SecuritySchemes[JwtBearerDefaults.AuthenticationScheme] = scheme;

                var schemeReference = new OpenApiSecuritySchemeReference(JwtBearerDefaults.AuthenticationScheme, document);

                var requirements = new OpenApiSecurityRequirement
                {
                    [schemeReference] = new List<string>()
                };

                document.Security ??= new List<OpenApiSecurityRequirement>();
                document.Security.Add(requirements);
                
                return Task.CompletedTask;
            });
        });

        return services;
    }
}