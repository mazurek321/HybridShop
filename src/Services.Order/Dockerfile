FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY src/Services.Order/HybridShop.Services.Order.Api/HybridShop.Services.Order.Api.csproj src/Services.Order/HybridShop.Services.Order.Api/
COPY src/Services.Order/HybridShop.Services.Order.Application/HybridShop.Services.Order.Application.csproj src/Services.Order/HybridShop.Services.Order.Application/
COPY src/Services.Order/HybridShop.Services.Order.Core/HybridShop.Services.Order.Core.csproj src/Services.Order/HybridShop.Services.Order.Core/
COPY src/Services.Order/HybridShop.Services.Order.Infrastructure/HybridShop.Services.Order.Infrastructure.csproj src/Services.Order/HybridShop.Services.Order.Infrastructure/
COPY BuildingBlocks/EventBus/EventBus.csproj BuildingBlocks/EventBus/

RUN dotnet restore "src/Services.Order/HybridShop.Services.Order.Api/HybridShop.Services.Order.Api.csproj"
COPY . .

WORKDIR "/src/src/Services.Order/HybridShop.Services.Order.Api"
RUN dotnet build "HybridShop.Services.Order.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "HybridShop.Services.Order.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "HybridShop.Services.Order.Api.dll"]