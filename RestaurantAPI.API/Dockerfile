# Build Stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ["RestaurantAPI.Domain/RestaurantAPI.Domain.csproj", "RestaurantAPI.Domain/"]
COPY ["RestaurantAPI.Application/RestaurantAPI.Application.csproj", "RestaurantAPI.Application/"]
COPY ["RestaurantAPI.Infrastructure/RestaurantAPI.Infrastructure.csproj", "RestaurantAPI.Infrastructure/"]
COPY ["RestaurantAPI.API/RestaurantAPI.csproj", "RestaurantAPI.API/"]
RUN dotnet restore "RestaurantAPI.API/RestaurantAPI.csproj"

COPY . .
WORKDIR "/src/RestaurantAPI.API"
RUN dotnet build "RestaurantAPI.csproj" -c Release -o /app/build

# Publish Stage
FROM build AS publish
RUN dotnet publish "RestaurantAPI.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Final Runtime Image
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app

RUN addgroup --system --gid 1001 dotnet \
    && adduser --system --uid 1001 --ingroup dotnet --shell /bin/false dotnet

RUN apt-get update && apt-get install -y curl && rm -rf /var/lib/apt/lists/*

COPY --from=publish /app/publish .

RUN chown -R dotnet:dotnet /app
USER dotnet

EXPOSE 8080

HEALTHCHECK --interval=30s --timeout=3s --start-period=60s --retries=3 \
    CMD curl -f http://localhost:8080/health || exit 1

ENTRYPOINT ["dotnet", "RestaurantAPI.dll"]
