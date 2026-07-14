using HybridShop.Services.Product.Infrastructure;
using HybridShop.BuildingBlocks.OpenApi;
using HybridShop.Services.Product.Application;
using HybridShop.BuildingBlocks.OpenApi.Auth;
using dotenv.net;

DotEnv.Load();

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddEnvironmentVariables();

builder.Services.AddSharedOpenApi();
builder.Services.AddAuthServices(builder.Configuration);

builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new System.Text.Json.Serialization.JsonStringEnumConverter());
    });
    
var app = builder.Build();

app.MapOpenApi("/api/product/openapi/{documentName}.json");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();