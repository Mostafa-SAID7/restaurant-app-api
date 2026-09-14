# Clean Architecture Refactor Roadmap

**Current State:** One assembly, JWT auth foundation in place, auth services well-structured but Architecture mismatch: docs say Clean Architecture; code is classic N-layer in one project.

**Goal:** Real multi-project Clean Architecture with SOLID principles and zero unnecessary duplication.

---

## Critical Issues (Priority Order)

### 🔴 HIGHEST: Dual Authentication Systems (must eliminate)

**Current State:**
- Old path: Usercode as API key → `UserService.GetUserCodeAsync`
- New path: JWT + RefreshToken → `AuthService + ITokenService`
- Result: Two parallel security models, two ways to identify a user

**Impact:** Confusion, security surface, code duplication, testing nightmare

**Action Required:**
1. All protected endpoints use `[Authorize]` + JWT
2. Inject `ICurrentUserService` instead of passing `apiKey` parameter
3. Remove API-key filter usage from business controllers (CartController, OrderController, UserController)
4. Soft-deprecate Usercode-as-key endpoints for business operations
5. **After Phase 2.10 testing:** Kill old API-key path completely

**Files to update (Phase 3):**
- `Controllers/CartController.cs` - still takes apiKey in some paths?
- `Controllers/OrderController.cs` - still takes apiKey in some paths?
- `Controllers/UserController.cs` - old controller marked [Obsolete]
- `Services/UserService.cs` - remove API-key generation/validation
- `Repositories/UserRepository.cs` - remove API-key lookups

---

### 🔴 HIGHEST: Dependency Inversion Violations

**Current Issues:**

```csharp
// BAD: Infrastructure in Application
AuthService → AppDbContext directly
UserRepository → DbContext mixed with PasswordHasher
```

**Fix Required:**

```csharp
// GOOD: Application depends on abstractions
AuthService(IUserRepository, IRefreshTokenRepository, IPasswordService)
UserRepository implements IUserRepository (no hashing logic)
PasswordService implements IPasswordService (isolated)
```

**Why:** AuthService must be testable without EF; services should not know they're using EF.

**Action:**
- [ ] Remove `AppDbContext` from `AuthService` constructor
- [ ] Inject `IUserRepository`, `IRefreshTokenRepository`, `IPasswordService` instead
- [ ] Move password verification from `UserRepository` to `IPasswordService`

---

### 🟠 HIGH: Dual Password Handling

**Current State:**
- `UserRepository` uses `PasswordHasher<User>`
- `Auth/Services/PasswordService` also hashes

**Result:** Two sources of truth for password policy

**Fix:**
- [ ] Remove all password logic from `UserRepository` and `IUserRepository`
- [ ] All hashing/verification goes through `IPasswordService` only
- [ ] Repository is data access only

---

### 🟠 HIGH: Single Responsibility Violations

**RestaurantService violates SRP:**
```csharp
// Should be split into:
IRestaurantService // CreateRestaurant, GetRestaurants, etc.
IMenuService       // GetMenu, AddItemToMenu, etc.
IItemService       // GetAllItems, etc.
```

**UserService mixed concerns:**
```csharp
// Auth concerns belong in Auth module
GetUserCodeAsync()        → Remove (JWT replaces this)
ValidateUserAsync()       → Move to IAuthService
UpdateUserPasswordAsync() → Auth responsibility
GenerateUserCodeAsync()   → Remove (Usercode deprecated)
```

**Action:**
- [ ] Create `IMenuService` / `MenuService` (GetMenu, GetMenuByRestaurantId, AddItem)
- [ ] Create `IItemService` / `ItemService` (GetAllItems, GetByFilters)
- [ ] Reduce `RestaurantService` to restaurants only
- [ ] Move auth/registration concerns from `UserService` to `AuthService`
- [ ] Keep `UserService` for profile management only (GetUser, UpdateProfile, DeleteAccount)

---

### 🟠 HIGH: Entity Leakage Across Layers

**Current Problem:**
```csharp
// Controllers and API contracts depend on Models.*
public ActionResult GetRestaurants() 
    → returns IEnumerable<Restaurant> (entity)

public ActionResult GetMenu(int restaurantId)
    → returns IEnumerable<GetItemsDTO> (good) but some return entities
```

**Why it's bad:**
- Swagger documents entity shapes
- Changes to entity structure break API contract
- Clients depend on internal implementation

**Fix:**
- [ ] **Rule:** Services return `Result<T>` or `DTO`, never entities
- [ ] All controller responses use response DTOs
- [ ] Controllers never see `Models.*` for output
- [ ] Swagger documents DTOs, not entities

**Action:**
- [ ] Create response DTOs: `RestaurantResponse`, `ItemResponse`, `OrderResponse`, etc.
- [ ] Update all services to return DTOs instead of entities
- [ ] Remove `[SwaggerResponse(200, typeof(Restaurant))]` → replace with `RestaurantResponse`

---

### 🟠 HIGH: Inconsistent Response Contract

**Current Problem:**
```csharp
// Most endpoints use ResponseHelper
ResponseHelper.Success(data) / NotFound() / Error()

// But Auth uses anonymous objects
new { success, message, errors, timestamp }
```

**Result:** Inconsistent API contract, hard to standardize client parsing

**Fix:**
- [ ] One unified `ApiResponse<T>` type used everywhere
- [ ] Or standardize ResponseHelper to all auth endpoints too
- [ ] Consistent error shape with problem details (RFC 7807 optional)

**Action:**
- [ ] Define `ApiResponse<T>` class (or enhance ResponseHelper)
- [ ] Update `AuthController` to use consistent response shape
- [ ] Document in Swagger/OpenAPI

---

### 🟡 MEDIUM: Services Return Entities

**Pattern to remove:**
```csharp
// BAD
Task<Restaurant> CreateRestaurantAsync(RestaurantDTO dto)
Task<Item> AddItemToMenuAsync(int restaurantId, ItemDTO dto)
Task<Cart> GetCartAsync(string userId)

// GOOD
Task<Result<RestaurantResponse>> CreateRestaurantAsync(RestaurantDTO dto)
Task<Result<ItemResponse>> AddItemToMenuAsync(int restaurantId, ItemDTO dto)
Task<Result<CartResponse>> GetCartAsync(string userId)
```

**Action:**
- [ ] Wrap service returns in `Result<T>` or return DTO directly
- [ ] Never return entities from services

---

### 🟡 MEDIUM: Fat Unit of Work

**Current:**
```csharp
IUnitOfWork exposes all repos + transactions + raw SQL methods
// Makes testing single repo harder, tempts inline SQL
```

**Better option:**
1. Keep UoW but resolve repos from DI (not `new`)
2. Or: Use MediatR + transaction scope for use cases
3. Or: Lightweight per-use-case transaction abstraction

**Action (Phase B):**
- [ ] Modify `UoW.Repository` properties to use `serviceProvider.GetRequiredService<T>()`
- [ ] Or: Plan MediatR migration for Phase C

---

### 🟡 MEDIUM: Naming & Style Inconsistency

**Issues:**
- Method names: `getrestaurants` vs `GetRestaurantsAsync` (inconsistent casing)
- DTO names: `GetItems`, `SetCart`, `menuDTO` vs `ItemResponse`, `AddToCartRequest`
- Routes: `/api/Restaurant` vs standard REST `/api/restaurants` (lowercase plural)
- Some endpoints return entities; others return DTOs

**Action:**
- [ ] Normalize to PascalCase methods: `GetRestaurantsAsync`, `CreateRestaurantAsync`
- [ ] DTO naming: `*Request` for input, `*Response` for output, `*Dto` for data transfer only
- [ ] Routes: lowercase plural resource names (`/api/restaurants`, `/api/orders`, `/api/cart`)
- [ ] Consistent: all outputs are DTOs, never entities

---

## Phased Refactor Plan

### Phase A: Unify & Clean (1–2 weeks, before multi-project split)

**Goal:** Fix critical duplication and SOLID violations while staying in one assembly.

#### A.1: Kill Dual Auth System
- [ ] Make sure Phase 2.10 tests confirm JWT works for all endpoints
- [ ] Remove all API-key parameters from CartController, OrderController
- [ ] Remove Usercode-as-key from UserService
- [ ] Delete old `UserController` endpoints (marked [Obsolete])
- [ ] Delete `ApiKeyAuthorizationFilter` tests (already deleted filter)
- **Commit:** "Phase A.1: Unify auth - JWT only, remove API-key fallback"

#### A.2: Single Password Service
- [ ] Remove all password methods from `UserRepository` / `IUserRepository`
- [ ] Move password verification to `IPasswordService.VerifyPasswordAsync(hash, plaintext)`
- [ ] Update `AuthService` to use `IPasswordService` for all verification
- [ ] Update `UserService` (if it had password methods) to use `IPasswordService`
- **Commit:** "Phase A.2: Unify password - IPasswordService only, remove repo hashing"

#### A.3: Split Services for SRP
- [ ] Create `IMenuService` interface and `MenuService` implementation
- [ ] Move `GetMenu`, `GetMenuByRestaurantId`, `AddItemToMenu` from `RestaurantService` → `MenuService`
- [ ] Create `IItemService` interface and `ItemService` implementation
- [ ] Move `GetAllItems`, `GetByFilters` from `RestaurantService` → `ItemService`
- [ ] Reduce `RestaurantService` to: `GetRestaurants`, `GetRestaurantById`, `CreateRestaurant`, `RestaurantExists`, etc.
- [ ] Move auth/registration from `UserService` → `AuthService`
- [ ] Keep `UserService` for: `GetUser`, `UpdateProfile`, `DeleteAccount`, `ChangePassword`
- **Files:**
  - Create: `Services/Interfaces/IMenuService.cs`, `IItemService.cs`
  - Create: `Services/Implementation/MenuService.cs`, `ItemService.cs`
  - Modify: `RestaurantService.cs`, `UserService.cs`, `AuthService.cs`
  - Update: `ServiceConfiguration.cs` (register new services)
  - Update: Controllers to inject new services
- **Commit:** "Phase A.3: SRP - Split menu/item/auth concerns into separate services"

#### A.4: Unified Response Contract
- [ ] Create or enhance `ApiResponse<T>` class:
  ```csharp
  public class ApiResponse<T>
  {
      public bool Success { get; set; }
      public T? Data { get; set; }
      public string? Message { get; set; }
      public List<string>? Errors { get; set; }
      public DateTime Timestamp { get; set; }
  }
  ```
- [ ] Update `ResponseHelper` to return `ApiResponse<T>`
- [ ] Update `AuthController` to use same response shape
- [ ] Update all controllers for consistency
- **Commit:** "Phase A.4: Unified response contract - consistent ApiResponse<T> everywhere"

#### A.5: Services Return DTOs, Never Entities
- [ ] Create response DTOs for all main entities:
  - `RestaurantResponse`, `MenuResponse`, `ItemResponse`, `OrderResponse`, `CartResponse`, `UserResponse`, etc.
- [ ] Update all `IService` interfaces to return DTO types or `Result<DtoType>`
- [ ] Update all service implementations
- [ ] Update all controllers (they should not see entity types)
- [ ] Update `MappingProfile` to include entity-to-DTO maps
- [ ] Update Swagger `[SwaggerResponse]` attributes to use DTO types
- **Commit:** "Phase A.5: DTO boundary - services return DTOs, never entities"

#### A.6: Fix DIP in AuthService
- [ ] Remove `AppDbContext` from `AuthService` constructor
- [ ] Inject `IUserRepository`, `IRefreshTokenRepository`, `IPasswordService`
- [ ] Update AuthService implementation to use repositories
- [ ] All DbContext access now through repositories
- **Commit:** "Phase A.6: DIP - AuthService depends on abstractions, not DbContext"

#### A.7: Repository Cleanup
- [ ] Remove `PasswordHasher` usage from `UserRepository`
- [ ] Remove any password verification logic from repos
- [ ] Remove API-key lookup methods from `UserRepository`
- [ ] Ensure repositories are pure data access (no business logic)
- **Commit:** "Phase A.7: Repository SRP - data access only, no business logic"

#### A.8: Naming Normalization
- [ ] Standardize method names: PascalCase, async suffixes
- [ ] Standardize DTO names: `*Request`, `*Response`, `*Dto`
- [ ] Normalize routes to lowercase plurals: `/api/restaurants`, `/api/orders`, `/api/auth`, `/api/cart`
- [ ] Audit all class/method/property names for consistency
- **Commit:** "Phase A.8: Naming - consistent PascalCase, clear DTO suffixes, REST routes"

**Milestone after Phase A:** Single assembly with SOLID applied, unified auth/password, consistent DTOs, no entity leakage.

---

### Phase B: Multi-Project Clean Architecture (1–2 weeks)

**Goal:** Split into 4 projects following Clean Architecture.

#### B.1: Project Structure
```
RestaurantAPI.Domain/
├── Entities/                    ← pure POCOs, no EF annotations
├── ValueObjects/                ← Money, Email, Quantity, etc.
├── DomainEvents/
└── Interfaces/ (optional)       ← only if needed by app layer

RestaurantAPI.Application/
├── UseCases/
│   ├── Orders/
│   │   ├── CreateOrderRequest
│   │   ├── CreateOrderResponse
│   │   └── CreateOrderHandler
│   ├── Cart/
│   ├── Auth/
│   └── ...
├── DTOs/
├── Interfaces/                  ← IRepository, IUnitOfWork abstractions
├── Validators/                  ← FluentValidation
├── Exceptions/
└── DependencyInjection.cs       ← register all app services

RestaurantAPI.Infrastructure/
├── Data/
│   ├── AppDbContext.cs
│   ├── Configurations/
│   └── Seeders/
├── Repositories/                ← concrete implementations
├── Services/                     ← external integrations (email, storage, JWT)
├── Identity/                     ← PasswordService, TokenService
└── DependencyInjection.cs       ← register infra services

RestaurantAPI.API/
├── Controllers/
├── Middleware/
├── Filters/
├── Program.cs                   ← DI composition root
└── Extensions/

RestaurantAPI.Tests/
└── Application/                 ← test handlers/use cases without EF
```

#### B.2: Move Domain
- [ ] Remove all EF annotations from entities
- [ ] Pure POCOs or records
- [ ] Move to `RestaurantAPI.Domain` project
- [ ] No `using` statements for `Microsoft.EntityFrameworkCore`

#### B.3: Application Layer (Use Cases / MediatR optional)
- [ ] Create `IRepositoryX` interfaces (or keep in Application)
- [ ] Create use case handlers or service classes
- [ ] All DTOs here
- [ ] Validators via FluentValidation
- [ ] Exception types
- [ ] No EF references

#### B.4: Move Infrastructure
- [ ] `AppDbContext` → `Infrastructure`
- [ ] `EF Configurations` → `Infrastructure/Data/Configurations/`
- [ ] `Repositories` (concrete) → `Infrastructure/Repositories/`
- [ ] `PasswordService`, `TokenService` → `Infrastructure/Identity/`
- [ ] File storage, email → `Infrastructure/Services/`

#### B.5: Update API Layer
- [ ] Controllers remain thin
- [ ] No business logic in controllers
- [ ] Inject application services / handlers
- [ ] `Program.cs` registers all layers

#### B.6: Dependency Injection
- [ ] `DomainDependencyInjection.cs` (if needed)
- [ ] `ApplicationDependencyInjection.cs` (register all use cases, validators)
- [ ] `InfrastructureDependencyInjection.cs` (register repos, services, EF)
- [ ] `Program.cs` calls all three

**Milestone after Phase B:** Real multi-project Clean Architecture, clear layer separation, easy to test.

---

### Phase C: Hardening & Advanced (1–2 weeks)

#### C.1: Result / Error Pattern
- [ ] Replace exceptions for control flow with `Result<T> { IsSuccess, Value, Error }`
- [ ] Consistent error handling
- [ ] Type-safe return values

#### C.2: Value Objects
- [ ] `Email` value object (validation in constructor)
- [ ] `Money` value object (amounts, currency)
- [ ] `Quantity` value object (non-negative, max validations)
- [ ] Used in domain entities and application DTOs

#### C.3: Domain Invariants
- [ ] Move validation logic into entities where possible
- [ ] E.g., `Order.AddItem()` enforces total >= 0, max items, etc.

#### C.4: Lightweight Unit of Work
- [ ] Refactor UoW to resolve repos from DI instead of `new`
- [ ] Or: Replace with `ITransactionScope` + per-use-case transaction handling
- [ ] Or: MediatR pipeline with transaction decorator

#### C.5: Integration Tests
- [ ] Test full use cases against real EF
- [ ] Test repositories, auth, business logic
- [ ] No mocks for EF tests; no in-memory (too permissive)

---

## Critical Path (Do These First)

| Step | Task | Impact | Est. Time |
|------|------|--------|-----------|
| **1** | A.1: Kill dual auth (JWT only) | Eliminates biggest confusion | 2 days |
| **2** | A.2: Single password service | DIP, testability | 1 day |
| **3** | A.3: Split services for SRP | Maintainability | 3 days |
| **4** | A.4: Unified response contract | API consistency | 1 day |
| **5** | A.5: Services return DTOs | Layer cleanliness | 3 days |
| **6** | A.6: AuthService DIP | Foundation for Phase B | 1 day |
| **7** | A.7: Repository cleanup | Repository SRP | 1 day |
| **8** | A.8: Naming normalization | Consistency | 2 days |
| **B.1-B.6** | Multi-project split | Real Clean Architecture | 5–7 days |
| **C.1-C.5** | Hardening | Production ready | 3–5 days |

**Total estimated:** 3–4 weeks for full Clean Architecture.

---

## Immediate Next Actions (This Week)

1. **Commit Phase 2.10 testing** - Verify JWT works for all endpoints
2. **Decision:** Approve Phase A roadmap
3. **Start A.1:** Remove API-key paths from CartController, OrderController, UserService
4. **Start A.2:** Move password logic to `IPasswordService` only
5. **Create architecture decision record (ADR):** "Why we're moving to Clean Architecture multi-project"

---

## Files Most Affected (Priority Order)

### Phase A.1 (Auth Unification)
- `Controllers/CartController.cs`, `OrderController.cs` - remove apiKey params
- `Services/UserService.cs` - remove API-key logic
- `Repositories/UserRepository.cs` - remove API-key lookups

### Phase A.2 (Password Unification)
- `Services/Auth/PasswordService.cs` - **source of truth**
- `Repositories/UserRepository.cs` - **remove password methods**
- `Services/UserService.cs` - update to use PasswordService

### Phase A.3 (SRP Services)
- Create: `Services/Interfaces/IMenuService.cs`, `IItemService.cs`
- Create: `Services/Implementation/MenuService.cs`, `ItemService.cs`
- Modify: `Services/RestaurantService.cs`, `UserService.cs`, `AuthService.cs`

### Phase A.5 (DTOs)
- Create many response DTOs
- Update all services
- Update all controllers

---

## Success Criteria

After Phase A:
- ✓ No dual auth or password systems
- ✓ All services return DTOs, never entities
- ✓ SOLID principles visibly applied in code
- ✓ Consistent naming and style
- ✓ Unified API response contract
- ✓ Build passes, tests pass

After Phase B:
- ✓ Four clean projects (Domain, Application, Infrastructure, API)
- ✓ Layer boundaries enforced
- ✓ Pure domain with no EF
- ✓ Application layer testable without EF
- ✓ Clear dependency flow: API → Application → Domain; Infrastructure ⟂ Domain/Application

After Phase C:
- ✓ Result<T> / error pattern used throughout
- ✓ Value objects for Money, Email, Quantity
- ✓ Domain invariants enforced
- ✓ Integration tests for full flows
- ✓ Production-ready Clean Architecture

---

## Decision Points

1. **MediatR or traditional service layer?**
   - MediatR: More flexible for commands/queries, pipeline decorators (auth, logging, validation)
   - Services: Simpler, more explicit, less magic
   - **Recommendation:** Traditional services first (Phase A-B), consider MediatR in Phase C if team wants it

2. **Result<T> or exceptions?**
   - Result<T>: Type-safe, no surprises
   - Exceptions: Familiar, simpler for unexpected errors
   - **Recommendation:** Result<T> for known business failures (validation, not found); exceptions for bugs

3. **Value objects everywhere or just Money?**
   - Full: Email, Quantity, Price, all with validation
   - Minimal: Money only, keep others as primitives
   - **Recommendation:** Start with Money; add Email, Quantity as needed (Phase C)

4. **Tests in separate project or alongside code?**
   - Separate: Better organization, easy to exclude from package
   - **Recommendation:** One `RestaurantAPI.Tests` project with subfolders: Application/, Infrastructure/, Controllers/

---

## Notes

- This roadmap assumes team agreement that Clean Architecture is the goal
- Phase A can be done in one assembly without disrupting main; Phase B is the big move
- Each phase has clear commit messages for traceability
- Testing should increase as you go: Phase A (some), Phase B (good coverage), Phase C (high coverage)
- After Phase C, the codebase will be maintainable, testable, and scalable

