FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY src/Services.Product/HybridShop.Services.Product.Api/HybridShop.Services.Product.Api.csproj src/Services.Product/HybridShop.Services.Product.Api/
COPY src/Services.Product/HybridShop.Services.Product.Application/HybridShop.Services.Product.Application.csproj src/Services.Product/HybridShop.Services.Product.Application/
COPY src/Services.Product/HybridShop.Services.Product.Core/HybridShop.Services.Product.Core.csproj src/Services.Product/HybridShop.Services.Product.Core/
COPY src/Services.Product/HybridShop.Services.Product.Infrastructure/HybridShop.Services.Product.Infrastructure.csproj src/Services.Product/HybridShop.Services.Product.Infrastructure/
COPY BuildingBlocks/EventBus/HybridShop.BuildingBlocks.EventBus.csproj BuildingBlocks/EventBus/
COPY BuildingBlocks/OpenApi/OpenApi.csproj BuildingBlocks/OpenApi/

RUN dotnet restore "src/Services.Product/HybridShop.Services.Product.Api/HybridShop.Services.Product.Api.csproj"
COPY . .

WORKDIR "/src/src/Services.Product/HybridShop.Services.Product.Api"
RUN dotnet build "HybridShop.Services.Product.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "HybridShop.Services.Product.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "HybridShop.Services.Product.Api.dll"]