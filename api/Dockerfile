# Build Stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy csproj and restore as distinct layers
COPY ["api/RestaurantApi.csproj", "api/"]
RUN dotnet restore "api/RestaurantApi.csproj"

# Copy everything else and build
COPY . .
WORKDIR "/src/api"
RUN dotnet build "RestaurantApi.csproj" -c Release -o /app/build

# Publish Stage
FROM build AS publish
RUN dotnet publish "RestaurantApi.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Final Runtime Image
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=publish /app/publish .

# Expose port 8080 (standard for .NET 8 in Docker)
EXPOSE 8080

ENTRYPOINT ["dotnet", "RestaurantApi.dll"]
