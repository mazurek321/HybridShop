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

// if (app.Environment.IsDevelopment())
// {
    app.UseSwagger(options =>
    {
        options.RouteTemplate = "swagger/{documentName}/swagger.json";
    });

    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("./v1/swagger.json", "HybridShop Auth API v1");
        options.RoutePrefix = "swagger"; 
    });
// }

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();