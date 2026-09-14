# Build Stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy csproj and restore as distinct layers for better caching
COPY ["api/RestaurantAPI.csproj", "api/"]
RUN dotnet restore "api/RestaurantAPI.csproj"

# Copy everything else and build
COPY . .
WORKDIR "/src/api"
RUN dotnet build "RestaurantAPI.csproj" -c Release -o /app/build

# Publish Stage
FROM build AS publish
RUN dotnet publish "RestaurantAPI.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Final Runtime Image
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app

# Create non-root user for security
RUN addgroup --system --gid 1001 dotnet \
    && adduser --system --uid 1001 --ingroup dotnet --shell /bin/false dotnet

# Install curl for health checks
RUN apt-get update && apt-get install -y curl && rm -rf /var/lib/apt/lists/*

# Copy published app
COPY --from=publish /app/publish .

# Change ownership to non-root user
RUN chown -R dotnet:dotnet /app
USER dotnet

# Expose port 8080 (standard for .NET 8 in Docker)
EXPOSE 8080

# Health check
HEALTHCHECK --interval=30s --timeout=3s --start-period=60s --retries=3 \
    CMD curl -f http://localhost:8080/health || exit 1

ENTRYPOINT ["dotnet", "RestaurantAPI.dll"]
