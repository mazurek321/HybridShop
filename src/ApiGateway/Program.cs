using Microsoft.AspNetCore.RateLimiting;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(options =>
{
    options.AddPolicy("DefaultCorsPolicy", policy =>
    {
        policy.WithOrigins("http://localhost:3000", "http://localhost:5173") 
              .AllowAnyMethod()
              .AllowAnyHeader()
              .AllowCredentials();
    });
});

builder.Services.AddReverseProxy()
    .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"));

builder.Services.AddRateLimiter(options =>
{
    options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;

    options.AddFixedWindowLimiter("auth-policy", opt =>
    {
        opt.Window = TimeSpan.FromMinutes(1);
        opt.PermitLimit = 5; 
        opt.QueueLimit = 0;
    });

    options.AddFixedWindowLimiter("general-policy", opt =>
    {
        opt.Window = TimeSpan.FromSeconds(10);
        opt.PermitLimit = 100;
        opt.QueueLimit = 10;
    });

    options.AddFixedWindowLimiter("order-policy", opt =>
    {
        opt.Window = TimeSpan.FromMinutes(1);
        opt.PermitLimit = 15; 
        opt.QueueLimit = 2;
    });

    options.AddFixedWindowLimiter("notification-policy", opt =>
    {
        opt.Window = TimeSpan.FromMinutes(1);
        opt.PermitLimit = 5; 
        opt.QueueLimit = 0;
    });
});

var app = builder.Build();

app.UseCors("DefaultCorsPolicy");

app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint("/api/auth/openapi/v1.json", "Auth Service API");
    options.SwaggerEndpoint("/api/product/openapi/v1.json", "Product Service API");
    options.SwaggerEndpoint("/api/order/openapi/v1.json", "Order Service API");
    
    options.RoutePrefix = "swagger";
});

app.UseRateLimiter();

app.MapReverseProxy();

app.Run();