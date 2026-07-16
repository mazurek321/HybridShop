using HybridShop.Services.Product.Infrastructure;
using HybridShop.BuildingBlocks.OpenApi;
using HybridShop.Services.Product.Application;
using HybridShop.BuildingBlocks.OpenApi.Auth;
using dotenv.net;
using HybridShop.Services.Product.Api.Grpc;

DotEnv.Load();

var builder = WebApplication.CreateBuilder(args);

builder.WebHost.ConfigureKestrel(options =>
{
    options.ListenAnyIP(8080, o => o.Protocols = Microsoft.AspNetCore.Server.Kestrel.Core.HttpProtocols.Http1);
    options.ListenAnyIP(8081, o => o.Protocols = Microsoft.AspNetCore.Server.Kestrel.Core.HttpProtocols.Http2);
});

builder.Configuration.AddEnvironmentVariables();

builder.Services.AddGrpc();

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

app.MapGrpcService<ProductGrpcServer>();

app.MapControllers();

app.Run();