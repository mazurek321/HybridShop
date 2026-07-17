using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Security.Claims;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using BuildingBlocks.OpenApi.Context;

namespace HybridShop.BuildingBlocks.OpenApi.Auth;

public static class AuthenticationExtensions
{
    public static IServiceCollection AddAuthServices(this IServiceCollection services, IConfiguration configuration)
    {
        var envKey = Environment.GetEnvironmentVariable("JWT_KEY") ?? configuration["JWT_KEY"];
        var envIssuer = Environment.GetEnvironmentVariable("JWT_ISSUER") ?? configuration["JWT_ISSUER"];
        var envAudience = Environment.GetEnvironmentVariable("JWT_AUDIENCE") ?? configuration["JWT_AUDIENCE"];

        if (!string.IsNullOrEmpty(envKey)) configuration["Jwt:Key"] = envKey;
        if (!string.IsNullOrEmpty(envIssuer)) configuration["Jwt:Issuer"] = envIssuer;
        if (!string.IsNullOrEmpty(envAudience)) configuration["Jwt:Audience"] = envAudience;

        var jwtKey = configuration["Jwt:Key"] 
            ?? throw new InvalidOperationException("JWT Signing Key is missing from configuration.");
            
        var issuer = configuration["Jwt:Issuer"] 
            ?? throw new InvalidOperationException("JWT Issuer is missing from configuration.");
            
        var audience = configuration["Jwt:Audience"] 
            ?? throw new InvalidOperationException("JWT Audience is missing from configuration.");

        var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));

        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = signingKey,
                
                ValidateIssuer = true,
                ValidIssuer = issuer,
                
                ValidateAudience = true,
                ValidAudience = audience,
                
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero,

                NameClaimType = ClaimTypes.NameIdentifier,
                RoleClaimType = ClaimTypes.Role
            };

            options.Events = new JwtBearerEvents
            {
                OnAuthenticationFailed = context =>
                {
                    Console.WriteLine($"[JWT ERROR] Walidacja tokenu nie powiodła się: {context.Exception.Message}");
                    return Task.CompletedTask;
                },
                OnTokenValidated = context =>
                {
                    Console.WriteLine("[JWT SUCCESS] Token poprawnie zweryfikowany przez .NET.");
                    return Task.CompletedTask;
                }
            };
        });

        services.AddHttpContextAccessor();
        services.AddScoped<IUserContext, UserContext>();

        services.AddAuthorization();
        return services;
    }
}