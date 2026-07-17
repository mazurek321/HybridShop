using HybridShop.Services.Product.Infrastructure;
using HybridShop.BuildingBlocks.OpenApi;
using HybridShop.Services.Product.Application;
using HybridShop.BuildingBlocks.OpenApi.Auth;
using dotenv.net;
using HybridShop.Services.Product.Api.Grpc;

string? currentDir = Directory.GetCurrentDirectory();
string? envFilePath = null;

while (currentDir != null)
{
    var potentialPath = Path.Combine(currentDir, ".env");
    if (File.Exists(potentialPath))
    {
        envFilePath = potentialPath;
        break;
    }
    currentDir = Directory.GetParent(currentDir)?.FullName;
}

if (envFilePath != null)
{
    DotEnv.Load(options: new DotEnvOptions(envFilePaths: new[] { envFilePath }));
    Console.WriteLine($"[Sukces] Załadowano plik .env z: {envFilePath}");
}
else
{
    Console.WriteLine("[Błąd] Nie znaleziono pliku .env w żadnym z katalogów nadrzędnych!");
}

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddEnvironmentVariables();

builder.WebHost.ConfigureKestrel(options =>
{
    options.ListenAnyIP(8080, o => o.Protocols = Microsoft.AspNetCore.Server.Kestrel.Core.HttpProtocols.Http1);
    options.ListenAnyIP(8081, o => o.Protocols = Microsoft.AspNetCore.Server.Kestrel.Core.HttpProtocols.Http2);
});

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