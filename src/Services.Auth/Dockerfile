FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY src/Services.Auth/HybridShop.Services.Auth.Api/HybridShop.Services.Auth.Api.csproj src/Services.Auth/HybridShop.Services.Auth.Api/
COPY src/Services.Auth/HybridShop.Services.Auth.Application/HybridShop.Services.Auth.Application.csproj src/Services.Auth/HybridShop.Services.Auth.Application/
COPY src/Services.Auth/HybridShop.Services.Auth.Infrastructure/HybridShop.Services.Auth.Infrastructure.csproj src/Services.Auth/HybridShop.Services.Auth.Infrastructure/
COPY src/Services.Auth/HybridShop.Services.Auth.Core/HybridShop.Services.Auth.Core.csproj src/Services.Auth/HybridShop.Services.Auth.Core/
COPY BuildingBlocks/EventBus/EventBus.csproj BuildingBlocks/EventBus/
COPY BuildingBlocks/OpenApi/OpenApi.csproj BuildingBlocks/OpenApi/

RUN dotnet restore "src/Services.Auth/HybridShop.Services.Auth.Api/HybridShop.Services.Auth.Api.csproj"
COPY . .

WORKDIR "/src/src/Services.Auth/HybridShop.Services.Auth.Api"
RUN dotnet build "HybridShop.Services.Auth.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "HybridShop.Services.Auth.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "HybridShop.Services.Auth.Api.dll"]