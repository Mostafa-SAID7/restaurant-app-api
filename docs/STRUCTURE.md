# 🏗️ Project Structure

## Overview

**Restaurant API** follows a clean architecture pattern with clear separation of concerns.

## Backend Structure (.NET 8 Web API)

```
RestuarantAPI/
├── api/                     # .NET 8 Web API Source
│   ├── Configurations/      # DI and service setup
│   ├── Controllers/         # API endpoints
│   ├── Data/                # EF Core context
│   ├── DTOs/                # Data Transfer Objects
│   ├── Extensions/          # Extension methods
│   ├── Filters/             # Middleware and filters
│   ├── Helpers/             # Utilities
│   ├── Mapping/             # AutoMapper profiles
│   ├── Migrations/          # DB migrations
│   ├── Models/              # Domain entities
│   ├── Repositories/        # Data access layer
│   ├── Services/            # Business logic layer
│   ├── wwwroot/             # Static UI files
│   ├── appsettings.json     # Configuration
│   ├── RestuarantAPI.csproj # Project file
│   └── Program.cs           # Entry point
├── docs/                    # Detailed documentation
├── screenshots/             # UI previews
├── RestuarantAPI.sln        # Solution file
└── docker-compose.yml       # Docker orchestration
```
```

## Architecture Patterns

### Backend Patterns

1. **Repository Pattern**: Abstracts data access logic
2. **Unit of Work**: Manages transactions across multiple repositories
3. **Service Layer**: Contains business logic and orchestrates operations
4. **Dependency Injection**: Promotes loose coupling and testability
5. **DTO Pattern**: Separates internal models from API contracts

### Frontend Patterns

1. **Component-Based Architecture**: Modular and reusable UI components
2. **Service Layer**: Centralized business logic and HTTP communication
3. **Reactive Programming**: RxJS for handling asynchronous operations
4. **Feature Modules**: Organized by business functionality

## Key Design Principles

- **Separation of Concerns**: Each layer has a specific responsibility
- **Single Responsibility**: Classes and methods have one reason to change
- **Dependency Inversion**: High-level modules don't depend on low-level modules
- **Open/Closed Principle**: Open for extension, closed for modification
- **Interface Segregation**: Clients shouldn't depend on unused interfaces

---

[⬅️ Back to Main README](../README.md)