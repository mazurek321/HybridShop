FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY src/Services.Order/HybridShop.Services.Order.csproj src/Services.Order/
COPY BuildingBlocks/EventBus/HybridShop.BuildingBlocks.EventBus.csproj BuildingBlocks/EventBus/

RUN dotnet restore "src/Services.Order/HybridShop.Services.Order.csproj"
COPY . .

WORKDIR "/src/src/Services.Order"
RUN dotnet build "HybridShop.Services.Order.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "HybridShop.Services.Order.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "HybridShop.Services.Order.dll"]