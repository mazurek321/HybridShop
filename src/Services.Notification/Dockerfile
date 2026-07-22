FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY src/Services.Notification/HybridShop.Services.Notification.csproj src/Services.Notification/
COPY BuildingBlocks/EventBus/EventBus.csproj BuildingBlocks/EventBus/
COPY BuildingBlocks/OpenApi/OpenApi.csproj BuildingBlocks/OpenApi/  

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