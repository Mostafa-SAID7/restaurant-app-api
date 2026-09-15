# Phase 7: Application Layer Cleanup & Finalization

**Status:** Blocked - Requires focused migration session
**Blocker:** Mixing of old service classes with new CQRS handlers causing compilation errors

---

## Executive Summary

The migration is **90% complete** but there's a critical pre-existing architectural issue: the Application layer still contains **old service classes alongside new CQRS handlers**, creating namespace conflicts and duplicate logic.

### The Problem

**Old code (to remove):**
- `RestaurantAPI.Application/Services/` - Old service implementations
- `RestaurantAPI.Application/Common/Abstractions/` - Old interfaces
- Old DTOs used by old services

**New code (being added):**
- `RestaurantAPI.Application/Features/{Feature}/Commands/` - CQRS handlers
- `RestaurantAPI.Application/Features/{Feature}/Queries/` - CQRS handlers
- New DTOs for responses

**Result:** Build errors like:
```
error CS0246: The type or namespace name 'AppDbContext' could not be found
error CS0246: The type or namespace name 'CartService' could not be found
```

---

## Root Cause Analysis

During the **feature/jwt-endpoint-migration → main** merge:
1. ✅ New CQRS handlers were created correctly
2. ✅ 30+ handlers cover Restaurants, Cart, Orders, Users, Auth
3. ❌ **BUT:** Old service classes were NOT removed from Application layer
4. ❌ Old service interfaces still referenced in DI wiring
5. ❌ Test files still use old service namespaces

---

## What Needs To Happen (Phase 7 Tasks)

### Step 1: Remove Old Service Classes (CRITICAL)
**Delete these directories:**
- `RestaurantAPI.Application/Services/` (CartService, OrderService, etc.)
- `RestaurantAPI.Application/Common/Abstractions/` (old interfaces)
- `RestaurantAPI.Application/Mapping/` (old mapping profile)
- Keep only: `IAuthService`, `IPasswordService`, `ITokenService`, `ICurrentUserService`

### Step 2: Clean Up Old DTOs
**Review and keep only needed DTOs:**
- **Remove:** Old DTO classes that duplicate CQRS response types
- **Keep:** DTOs unique to external auth/responses (like `TokenResponseDto`, `LoginRequestDto`)
- **Move:** Feature-specific DTOs into `RestaurantAPI.Application/Features/{Feature}/DTOs/`

**Current DTO locations to reorganize:**
```
RestaurantAPI.Application/
├── Common/DTOs/
│   ├── ApiResponse.cs            ✅ Keep (response wrapper)
│   ├── CartDTOs.cs               ⚠️ Review (conflicts with Cart/DTOs/)
│   ├── ItemDTOs.cs               ⚠️ Review (conflicts with Restaurants/DTOs/)
│   ├── OrderDTOs.cs              ⚠️ Review (conflicts with Orders/DTOs/)
│   ├── RestaurantDTOs.cs         ⚠️ Review (conflicts with Restaurants/DTOs/)
│   └── ...
├── Features/Auth/DTOs/
│   ├── ChangePasswordDto.cs       ✅ Keep
│   ├── LoginRequestDto.cs         ✅ Keep
│   ├── RegisterRequestDto.cs      ✅ Keep
│   ├── RefreshTokenRequestDto.cs  ✅ Keep
│   └── TokenResponseDto.cs        ✅ Keep
└── Features/{Feature}/DTOs/       ✅ Responses for queries/commands
```

### Step 3: Fix DI Wiring (Program.cs)
**Current issue:**
```csharp
// OLD - pointing to deleted services
builder.Services.AddScoped<ICartService, CartService>();  
builder.Services.AddScoped<IOrderService, OrderService>();
```

**What to do:**
```csharp
// NEW - CQRS handlers registered via MediatR
builder.Services.AddMediatR(config => 
    config.RegisterServicesFromAssembly(typeof(ApplicationAssemblyMarker).Assembly)
);

// Keep ONLY infrastructure/auth services
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IPasswordService, PasswordService>();
builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddScoped<ICurrentUserService, CurrentUserService>();
```

### Step 4: Update Test Files
**Fix broken namespaces:**
- `RestaurantAPI.UnitTests/Services/` → Delete these (no service tests needed, test handlers instead)
- `RestaurantAPI.IntegrationTests/` → Update to use real API endpoints, not old services

**Create new test structure:**
```
RestaurantAPI.UnitTests/
├── Features/Orders/
│   ├── CreateOrderCommandHandlerTests.cs
│   ├── DeleteOrderCommandHandlerTests.cs
│   └── GetUserOrdersQueryHandlerTests.cs
├── Features/Cart/
│   ├── AddToCartCommandHandlerTests.cs
│   └── GetCartQueryHandlerTests.cs
└── ...
```

### Step 5: Verify Build & Run Tests
```bash
dotnet build                      # Should compile cleanly
dotnet test                       # All tests pass
dotnet run --project RestaurantAPI.API  # Server starts
```

---

## Why This Blocked the UserID Refactoring

When attempting to remove `UserID` from Order entity:
1. Modified Order.cs, OrderConfiguration.cs, handler, mapping ✅
2. Ran `dotnet build` ❌
3. **Result:** 68+ compilation errors from old service code trying to reference old namespaces

The **old services** were importing from old namespaces that no longer exist:
```csharp
using RestaurantAPI.Models;        // ❌ Doesn't exist (moved to Domain.Entities)
using RestaurantAPI.Repositories;  // ❌ Doesn't exist (moved to Infrastructure)
using RestaurantAPI.Data;          // ❌ Doesn't exist (moved to Infrastructure)
```

---

## Impact: Why This Matters

| Item | Current | Needed |
|------|---------|--------|
| **Architecture Clarity** | Mixed old/new ❌ | Pure CQRS ✅ |
| **Build Status** | Fails ❌ | Should pass ✅ |
| **Code Duplication** | High (services + handlers) ❌ | Zero ✅ |
| **Testing** | Old service tests ❌ | Handler tests ✅ |
| **Maintenance** | Confusing ❌ | Clear ✅ |

---

## Estimated Effort

- **Time:** 3-4 hours (focused work)
- **Files to delete:** ~20
- **Files to modify:** ~15
- **Breaking changes:** None (internal refactoring only)
- **Risk:** Low (no schema changes, pure code cleanup)

---

## Dependencies & Assumptions

✅ **Already done:**
- Domain layer is pure (no EF attributes)
- Infrastructure layer is complete (repos, UnitOfWork, EF configs)
- CQRS handlers exist and are wired (30+ handlers)
- Auth services still work (TokenService, PasswordService, CurrentUserService)

❌ **Blockers removed:**
- Old `api/` folder deleted from main ✅
- .sln file updated to reference new projects ✅

---

## Recommended Approach

**Option A: Surgical Cleanup (Recommended)**
1. Delete old service classes (Services/ directory)
2. Delete old mapping/abstractions
3. Update DI wiring
4. Fix test imports
5. Build and verify

**Option B: Verify Coverage First**
1. Audit which CQRS handlers replace each service
2. Ensure no missing handlers
3. Then execute Option A

---

## Success Criteria

- ✅ `dotnet build` runs with 0 errors
- ✅ `dotnet test` runs successfully (all tests pass)
- ✅ `dotnet run` starts the API server
- ✅ All endpoints work (test with Postman/curl)
- ✅ No warnings in build output
- ✅ UserID can be removed from Order (Phase 6 refactoring completes)

---

## Files Affected (Summary)

### To Delete
```
RestaurantAPI.Application/Services/Implementation/
  - CartService.cs
  - OrderService.cs
  - RestaurantService.cs
  - UserService.cs

RestaurantAPI.Application/Services/Interfaces/
  - ICartService.cs
  - IOrderService.cs
  - IRestaurantService.cs
  - IUserService.cs
  - IImageService.cs

RestaurantAPI.Application/Mapping/
  - MappingProfile.cs (old one)

tests/RestaurantAPI.UnitTests/Services/
  - CartServiceTests.cs
  - OrderServiceTests.cs
  - RestaurantServiceTests.cs
  - UserServiceTests.cs
  - (all service tests)
```

### To Modify
```
RestaurantAPI.API/Program.cs
  - Remove ICartService registration
  - Remove IOrderService registration
  - Remove IRestaurantService registration
  - Remove IUserService registration

tests/RestaurantAPI.UnitTests/
  - Fix namespace imports
  - Delete old service test directory

tests/RestaurantAPI.IntegrationTests/
  - Update to use API endpoints
  - Fix namespace imports
```

---

## Next Steps

1. **Create new task list** for Phase 7 cleanup
2. **Execute deletion** of old service classes
3. **Update DI wiring** in Program.cs
4. **Fix test files** with new namespaces
5. **Verify build & tests**
6. **Commit:** `refactor: remove legacy service layer - complete CQRS migration`
7. **Then:** UserID refactoring (Phase 6) can proceed cleanly

