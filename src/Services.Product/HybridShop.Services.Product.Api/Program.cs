using HybridShop.Services.Product.Infrastructure;
using HybridShop.BuildingBlocks.OpenApi;
using HybridShop.Services.Product.Application;
using MongoDB.Driver;
using HybridShop.BuildingBlocks.OpenApi.Auth;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSharedOpenApi();
builder.Services.AddAuthServices(builder.Configuration);

MongoMapping.Register();

builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);


var mongoSection = builder.Configuration.GetSection("MongoSettings");
var connectionString = mongoSection["ConnectionString"];
var databaseName = mongoSection["DatabaseName"];

var mongoClient = new MongoClient(connectionString);
builder.Services.AddSingleton<IMongoClient>(mongoClient);
builder.Services.AddScoped<IMongoDatabase>(sp => mongoClient.GetDatabase(databaseName));

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new System.Text.Json.Serialization.JsonStringEnumConverter());
    });
    
var app = builder.Build();

app.MapOpenApi("/api/products/openapi/{documentName}.json");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();