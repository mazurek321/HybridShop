FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY src/ApiGateway/HybridShop.ApiGateway.csproj src/ApiGateway/
COPY src/Services.Auth/HybridShop.Services.Auth.csproj src/Services.Auth/
COPY src/Services.Product/HybridShop.Services.Product.csproj src/Services.Product/
COPY src/Services.Order/HybridShop.Services.Order.csproj src/Services.Order/
COPY src/Services.Notification/HybridShop.Services.Notification.csproj src/Services.Notification/
COPY BuildingBlocks/EventBus/HybridShop.BuildingBlocks.EventBus.csproj BuildingBlocks/EventBus/

RUN dotnet restore "src/Services.Notification/HybridShop.Services.Notification.csproj"
COPY . .

WORKDIR "/src/src/Services.Notification"
RUN dotnet build "HybridShop.Services.Notification.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "HybridShop.Services.Notification.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "HybridShop.Services.Notification.dll"]