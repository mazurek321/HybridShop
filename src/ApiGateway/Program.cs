var builder = WebApplication.CreateBuilder(args);

builder.Services.AddReverseProxy()
    .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"));

var app = builder.Build();

app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint("/api/auth/openapi/v1.json", "Auth Service API");
    options.SwaggerEndpoint("/api/products/openapi/v1.json", "Product Service API");
    
    options.RoutePrefix = "swagger";
});

app.MapReverseProxy();

app.Run();