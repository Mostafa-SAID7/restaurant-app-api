# 🍽️ Restaurant API

> **Premium .NET 8 Web API** | Clean Architecture | CQRS Pattern | Production-Ready

A modern, scalable restaurant management API built with cutting-edge .NET architecture and best practices.

---

## ⚡ Quick Start

```bash
# Clone & navigate
git clone https://github.com/Mostafa-SAID7/restaurant-app-api.git
cd restaurant-app-api

# Run with Docker
docker-compose up -d

# Or run locally
dotnet restore
dotnet run --project RestaurantAPI.API
```

**API Access:**
- 🌐 **Swagger UI:** http://localhost:5124/swagger
- 📚 **API Documentation:** http://localhost:5124/Docs.html

---

## 🏗️ Architecture

```
┌─────────────────────────────────────────────┐
│           RestaurantAPI.API                 │  Presentation Layer
│  (Controllers, Middleware, Filters)         │  • Request/Response handling
└────────────────┬────────────────────────────┘  • Validation & routing
                 │
┌────────────────▼────────────────────────────┐
│      RestaurantAPI.Application              │  Application Layer
│  (CQRS Commands, Queries, Handlers)         │  • Business logic
└────────────────┬────────────────────────────┘  • Use case coordination
                 │
┌────────────────▼────────────────────────────┐
│      RestaurantAPI.Infrastructure           │  Infrastructure Layer
│  (Repositories, UnitOfWork, DbContext)      │  • Data persistence
└────────────────┬────────────────────────────┘  • External services
                 │
┌────────────────▼────────────────────────────┐
│       RestaurantAPI.Domain                  │  Domain Layer
│  (Entities, Interfaces, Business Rules)     │  • Pure business logic
└─────────────────────────────────────────────┘  • Zero dependencies
```

**Design Patterns:**
- ✅ Clean Architecture (4 layers)
- ✅ CQRS (Command Query Responsibility Segregation)
- ✅ Repository Pattern (9 repositories)
- ✅ Dependency Injection (full IoC)
- ✅ MediatR for command/query handling
- ✅ AutoMapper for DTO mapping
- ✅ Unit of Work for transaction management

---

## 📦 Core Features

### **Restaurants**
- Create, read, update, delete restaurants
- Menu management with items
- Item availability tracking

### **Orders**
- Order creation with line items
- Order history per user
- Transaction atomicity (all-or-nothing)
- Order authorization (users can only access their orders)

### **Cart**
- Add/remove items from cart
- Cart persistence per user
- Cart summarization with totals

### **Authentication & Authorization**
- JWT token-based auth
- Refresh token rotation
- Role-based access control (RBAC)
- Custom authorization policies

### **Users**
- User registration & login
- Password hashing (bcrypt)
- User profile management
- Admin capabilities

---

## 🛠️ Tech Stack

| Layer | Technology |
|-------|-----------|
| **Runtime** | .NET 8.0 |
| **Database** | SQL Server / EF Core 9 |
| **API** | ASP.NET Core 8 |
| **Architecture** | Clean Architecture + CQRS |
| **Command Handler** | MediatR |
| **Mapping** | AutoMapper |
| **Validation** | FluentValidation |
| **Testing** | xUnit, Moq, FluentAssertions |
| **DevOps** | Docker, Docker Compose |
| **CI/CD** | GitHub Actions |

---

## 📊 Project Structure

```
RestaurantAPI.Domain/
├── Entities/              # Core business models
├── Interfaces/            # Repository contracts
└── Exceptions/            # Domain exceptions

RestaurantAPI.Infrastructure/
├── Persistence/
│   ├── AppDbContext.cs
│   ├── Repositories/      # 9 data access repositories
│   ├── Configurations/    # EF entity mappings
│   └── Migrations/        # Database versioning
└── UnitOfWork.cs          # Transaction coordination

RestaurantAPI.Application/
├── Features/              # Feature-organized CQRS handlers
│   ├── Orders/
│   ├── Cart/
│   ├── Restaurants/
│   ├── Users/
│   └── Auth/
├── Common/
│   ├── DTOs/              # Response data transfer objects
│   ├── Mappings/          # AutoMapper profiles
│   └── Abstractions/      # Service interfaces
└── DependencyInjection.cs

RestaurantAPI.API/
├── Controllers/           # 5 REST endpoints
├── Middleware/            # Request pipeline
├── Filters/               # Validation & exception handling
├── Policies/              # Authorization requirements
└── Program.cs             # Startup configuration
```

---

## 🔐 Security

- **Password Security:** bcrypt hashing (PBKDF2)
- **JWT Tokens:** Signed with RS256
- **Refresh Tokens:** Database-stored, rotation on use
- **Authorization:** Policy-based (custom requirements)
- **CORS:** Configurable per environment
- **Input Validation:** All endpoints validated
- **SQL Injection Protection:** Parameterized queries (EF Core)

---

## 📋 API Endpoints

### **Restaurants**
```
POST   /api/restaurants              # Create restaurant
GET    /api/restaurants              # List all
GET    /api/restaurants/{id}         # Get by ID
PUT    /api/restaurants/{id}         # Update
DELETE /api/restaurants/{id}         # Delete
GET    /api/restaurants/{id}/menu    # Get menu
GET    /api/restaurants/items        # List all items
```

### **Orders**
```
POST   /api/orders/{restaurantId}/create    # Create order
GET    /api/orders                          # User's orders
GET    /api/orders/{masterId}               # Order details
DELETE /api/orders/{orderId}                # Delete line item
DELETE /api/orders/{masterId}               # Delete entire order
```

### **Cart**
```
POST   /api/cart/add                # Add to cart
GET    /api/cart                    # Get cart
PUT    /api/cart/{cartId}           # Update quantity
DELETE /api/cart/{cartId}           # Remove from cart
DELETE /api/cart                    # Clear cart
```

### **Authentication**
```
POST   /auth/register               # Register user
POST   /auth/login                  # Login
POST   /auth/refresh                # Refresh token
PUT    /auth/change-password        # Change password
```

### **Users**
```
GET    /api/users                   # List all (admin)
GET    /api/users/{id}              # Get profile
DELETE /api/users/{id}              # Delete user (admin)
```

---

## 🧪 Testing

```bash
# Run all tests
dotnet test

# With coverage
dotnet test /p:CollectCoverage=true

# Specific test file
dotnet test --filter "ClassName=OrderServiceTests"
```

**Test Structure:**
- Unit tests for CQRS handlers
- Integration tests for API endpoints
- Mock repositories for isolation

---

## 🚀 Deployment

### Docker
```bash
# Build image
docker build -f RestaurantAPI.API/Dockerfile -t restaurant-api:latest .

# Run container
docker run -p 5124:8080 restaurant-api:latest
```

### Docker Compose
```bash
docker-compose up -d           # Start services
docker-compose down            # Stop services
docker-compose logs -f         # View logs
```

### Environment Variables
```env
ConnectionStrings__DefaultConnection=Server=localhost;Database=RestaurantAPI;...
JWT_SECRET=your-secret-key-here
JWT_EXPIRATION_MINUTES=15
JWT_REFRESH_EXPIRATION_DAYS=7
ASPNETCORE_ENVIRONMENT=Production
```

---

## 📈 Performance & Scalability

- **CQRS Separation:** Independent read/write optimization
- **Unit of Work:** Batch database operations
- **Async/Await:** Non-blocking I/O throughout
- **Connection Pooling:** EF Core default
- **Pagination:** List endpoints support limit/offset
- **Caching:** Ready for distributed cache integration

---

## 📝 Database Schema

**Core Entities:**
- `Users` - User accounts with auth info
- `Restaurants` - Restaurant details
- `Items` - Menu items with pricing
- `Carts` - User shopping carts
- `MasterOrders` - Order summaries
- `Orders` - Order line items (references MasterOrder)
- `RefreshTokens` - Token rotation storage

**Key Relationships:**
- User → Many Orders
- User → Many MasterOrders
- MasterOrder → Many Orders (1:N)
- Restaurant → Many Items
- Item → Many Orders (snapshots)

---

## 🔄 CI/CD Pipeline

**GitHub Actions Workflows:**
- ✅ Build & Test on PR
- ✅ CodeQL Security Scan
- ✅ SonarQube Analysis (optional)
- ✅ Docker Image Build & Push
- ✅ Auto-merge dependencies

---

## 📚 Resources

| Resource | Link |
|----------|------|
| .NET Docs | https://docs.microsoft.com/dotnet |
| Clean Architecture | https://blog.cleancoder.com/uncle-bob/2012/08/13/the-clean-architecture.html |
| CQRS Pattern | https://martinfowler.com/bliki/CQRS.html |
| EF Core | https://docs.microsoft.com/ef/core |
| MediatR | https://github.com/jbogard/MediatR |

---

## 📄 License

[MIT License](LICENSE) © 2025 Mostafa SAID

---

## 🤝 Contributing

Contributions welcome! Please follow these steps:

1. Fork the repository
2. Create feature branch: `git checkout -b feature/your-feature`
3. Commit changes: `git commit -m "feat: add your feature"`
4. Push to branch: `git push origin feature/your-feature`
5. Open pull request

---

## 📧 Contact & Support

- **GitHub Issues:** [Report bugs](https://github.com/Mostafa-SAID7/restaurant-app-api/issues)
- **Discussions:** [Ask questions](https://github.com/Mostafa-SAID7/restaurant-app-api/discussions)
- **Email:** contact@example.com

---

**Built with ❤️ using .NET 8 | Clean Architecture | Modern Patterns**
