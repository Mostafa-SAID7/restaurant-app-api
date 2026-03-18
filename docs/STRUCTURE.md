# Project Structure

## Overview

The NooR Restaurant Management System follows a clean architecture pattern with clear separation of concerns between the backend API and frontend application.

## Backend Structure (.NET 8 Web API)

```
backend/api/
├── Configurations/          # Dependency injection and service configurations
│   ├── ApiConfiguration.cs
│   ├── CorsConfiguration.cs
│   ├── DatabaseConfiguration.cs
│   ├── FilterConfiguration.cs
│   ├── MiddlewareConfiguration.cs
│   ├── ServiceConfiguration.cs
│   └── SwaggerConfiguration.cs
├── Controllers/             # API endpoints and HTTP request handling
│   ├── CartController.cs
│   ├── OrderController.cs
│   ├── RestaurantController.cs
│   └── UserController.cs
├── Data/                    # Database context and configuration
│   └── AppDbContext.cs
├── DTOs/                    # Data Transfer Objects for API contracts
│   ├── CartDTO.cs
│   ├── GetItemsDTO.cs
│   ├── ImageRequest.cs
│   ├── ItemDTO.cs
│   ├── MenuDTO.cs
│   ├── OrderDTO.cs
│   ├── RestaurantCategory.cs
│   ├── RestaurantDTO.cs
│   └── UserDTO.cs
├── Extensions/              # Extension methods for common operations
│   ├── DateTimeExtensions.cs
│   ├── DecimalExtensions.cs
│   ├── EnumerableExtensions.cs
│   ├── HttpContextExtensions.cs
│   └── StringExtensions.cs
├── Filters/                 # Action filters and middleware
│   ├── ApiKeyAuthorizationFilter.cs
│   ├── ExceptionFilter.cs
│   ├── FileUploadOperationFilter.cs
│   ├── LoggingFilter.cs
│   ├── RateLimitingFilter.cs
│   └── ValidationFilter.cs
├── Helpers/                 # Utility classes and helper methods
│   ├── FileHelper.cs
│   ├── ResponseHelper.cs
│   └── ValidationHelper.cs
├── Mapping/                 # AutoMapper profiles
│   └── MappingProfile.cs
├── Migrations/              # Entity Framework database migrations
├── Models/                  # Domain entities and data models
│   ├── Cart.cs
│   ├── Item.cs
│   ├── MasterOrder.cs
│   ├── Order.cs
│   ├── Restaurant.cs
│   └── User.cs
├── Repositories/            # Data access layer with Repository pattern
│   ├── Interfaces/          # Repository contracts
│   │   ├── IBaseRepository.cs
│   │   ├── ICartRepository.cs
│   │   ├── IItemRepository.cs
│   │   ├── IMasterOrderRepository.cs
│   │   ├── IOrderRepository.cs
│   │   ├── IRestaurantRepository.cs
│   │   ├── IUnitOfWork.cs
│   │   └── IUserRepository.cs
│   └── Implementation/      # Repository implementations
│       ├── BaseRepository.cs
│       ├── CartRepository.cs
│       ├── ItemRepository.cs
│       ├── MasterOrderRepository.cs
│       ├── OrderRepository.cs
│       ├── RestaurantRepository.cs
│       ├── UnitOfWork.cs
│       └── UserRepository.cs
├── Services/                # Business logic layer
│   ├── Interfaces/          # Service contracts
│   │   ├── ICartService.cs
│   │   ├── IImageService.cs
│   │   ├── IOrderService.cs
│   │   ├── IRestaurantService.cs
│   │   └── IUserService.cs
│   └── Implementation/      # Service implementations
│       ├── CartService.cs
│       ├── ImageService.cs
│       ├── OrderService.cs
│       ├── RestaurantService.cs
│       └── UserService.cs
├── wwwroot/                 # Static files and uploaded images
├── appsettings.json         # Application configuration
├── appsettings.Development.json
├── FakeRestuarantAPI.csproj # Project file
└── Program.cs               # Application entry point
```

## Frontend Structure (Angular 19)

```
frontend/restaurant-app/
├── src/
│   ├── app/
│   │   ├── core/                    # Core functionality (singleton services)
│   │   │   ├── models/              # TypeScript interfaces and models
│   │   │   │   ├── menu-item.model.ts
│   │   │   │   ├── order.model.ts
│   │   │   │   ├── reservation.model.ts
│   │   │   │   └── review.model.ts
│   │   │   └── services/            # Core services
│   │   │       ├── cart.service.ts
│   │   │       ├── menu.service.ts
│   │   │       ├── reservation.service.ts
│   │   │       └── review.service.ts
│   │   ├── features/                # Feature modules
│   │   │   ├── about/
│   │   │   ├── checkout/
│   │   │   ├── home/
│   │   │   ├── menu/
│   │   │   ├── privacy/
│   │   │   ├── reservations/
│   │   │   └── terms/
│   │   ├── layout/                  # Layout components
│   │   │   ├── footer/
│   │   │   └── header/
│   │   ├── shared/                  # Shared components and utilities
│   │   │   └── components/
│   │   │       ├── custom-calendar.component.ts
│   │   │       ├── custom-select.component.ts
│   │   │       └── icon.component.ts
│   │   ├── app.component.html
│   │   ├── app.component.scss
│   │   ├── app.component.ts
│   │   ├── app.config.ts
│   │   └── app.routes.ts
│   ├── assets/                      # Static assets
│   ├── environments/                # Environment configurations
│   ├── index.html
│   ├── main.ts
│   └── styles.scss
├── angular.json                     # Angular CLI configuration
├── package.json                     # Dependencies and scripts
└── tsconfig.json                    # TypeScript configuration
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