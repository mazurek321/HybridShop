using Microsoft.AspNetCore.OpenApi;
using HybridShop.BuildingBlocks.OpenApi;


var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSharedOpenApi();

var app = builder.Build();

app.MapOpenApi("/api/products/openapi/{documentName}.json");


app.MapGet("/test", () => Results.Ok("Działa z Product Service przez Gateway!"));

app.Run();