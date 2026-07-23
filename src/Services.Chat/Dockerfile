FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY src/Services.Chat/HybridShop.Services.Chat.Api/HybridShop.Services.Chat.Api.csproj src/Services.Chat/HybridShop.Services.Chat.Api/
COPY src/Services.Chat/HybridShop.Services.Chat.Application/HybridShop.Services.Chat.Application.csproj src/Services.Chat/HybridShop.Services.Chat.Application/
COPY src/Services.Chat/HybridShop.Services.Chat.Core/HybridShop.Services.Chat.Core.csproj src/Services.Chat/HybridShop.Services.Chat.Core/
COPY src/Services.Chat/HybridShop.Services.Chat.Infrastructure/HybridShop.Services.Chat.Infrastructure.csproj src/Services.Chat/HybridShop.Services.Chat.Infrastructure/
COPY BuildingBlocks/EventBus/EventBus.csproj BuildingBlocks/EventBus/

RUN dotnet restore "src/Services.Chat/HybridShop.Services.Chat.Api/HybridShop.Services.Chat.Api.csproj"
COPY . .

WORKDIR "/src/src/Services.Chat/HybridShop.Services.Chat.Api"
RUN dotnet build "HybridShop.Services.Chat.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "HybridShop.Services.Chat.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "HybridShop.Services.Chat.Api.dll"]