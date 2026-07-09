using HybridShop.Services.Auth.Api.Extensions;
using HybridShop.Services.Auth.Application;
using HybridShop.Services.Auth.Infrastructure;
using HybridShop.BuildingBlocks.OpenApi;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSharedOpenApi();
builder.Services.AddAuthServices(builder.Configuration);
builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

builder.Services.AddHttpContextAccessor();
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new System.Text.Json.Serialization.JsonStringEnumConverter());
    });

var app = builder.Build();

app.MapOpenApi("/api/auth/openapi/{documentName}.json");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();