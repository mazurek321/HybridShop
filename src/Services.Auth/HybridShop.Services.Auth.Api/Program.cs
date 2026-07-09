using HybridShop.Services.Auth.Api.Extensions;
using HybridShop.Services.Auth.Application;
using HybridShop.Services.Auth.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddApiServices();
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

app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint("/api/auth/openapi/v1.json", "HybridShop Auth API v1");
    options.RoutePrefix = "swagger"; 
});

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();