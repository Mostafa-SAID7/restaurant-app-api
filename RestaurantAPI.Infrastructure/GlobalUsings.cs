// GlobalUsings.cs — Infrastructure project
// These global usings resolve:
// 1. All auto-generated EF Core migration files that reference AppDbContext using the
//    old "RestaurantAPI.Data" namespace (now RestaurantAPI.Infrastructure.Persistence).
// 2. Seeds and services that use ILogger<>, IServiceCollection without explicit usings.
global using RestaurantAPI.Infrastructure.Persistence;
global using Microsoft.Extensions.Logging;
global using Microsoft.Extensions.DependencyInjection;
global using Microsoft.AspNetCore.Http;
global using Microsoft.AspNetCore.Builder;
