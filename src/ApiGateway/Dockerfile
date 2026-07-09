FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY src/ApiGateway/HybridShop.ApiGateway.csproj src/ApiGateway/

RUN dotnet restore "src/ApiGateway/HybridShop.ApiGateway.csproj"
COPY . .

WORKDIR "/src/src/ApiGateway"
RUN dotnet build "HybridShop.ApiGateway.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "HybridShop.ApiGateway.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "HybridShop.ApiGateway.dll"]