# Immediate Next Steps (This Week)

**Goal:** Prepare Phase 2.10 testing + align on Phase A roadmap

---

## Phase 2 Wrap-Up (Complete This Week)

### 2.10: Test JWT Endpoints + Verify RBAC

**Endpoint Testing Checklist:**

#### Public Endpoints (No Auth Required)
- [ ] `GET /api/restaurants` → 200 OK, returns list
- [ ] `GET /api/restaurants/{id}` → 200 OK, returns restaurant
- [ ] `GET /api/restaurants/{id}/menu` → 200 OK, returns menu items
- [ ] `GET /api/restaurants/items/all` → 200 OK, returns all items
- [ ] `GET /api/auth/...` (public ones if any) → 200 OK

#### Admin-Only Endpoints
- [ ] `POST /api/restaurants/create` with Admin JWT → 201 OK
- [ ] `POST /api/restaurants/create` with Customer JWT → 403 Forbidden
- [ ] `POST /api/restaurants/create` without auth → 401 Unauthorized

#### Authenticated Endpoints (Any Role)
- [ ] `POST /api/auth/login` → 200 OK, returns JWT + refresh token
- [ ] `POST /api/auth/refresh` with valid refresh token → 200 OK, new JWT
- [ ] `POST /api/auth/logout` with valid JWT → 200 OK
- [ ] `POST /api/auth/change-password` with valid JWT → 200 OK

#### Customer Endpoints (Cart + Orders)
- [ ] `GET /api/cart` with Customer JWT → 200 OK, returns items
- [ ] `POST /api/cart` with Customer JWT → 201 OK, item added
- [ ] `GET /api/cart/summary` with Customer JWT → 200 OK
- [ ] `DELETE /api/cart` with Customer JWT → 200 OK
- [ ] `POST /api/orders/{restaurantId}/create` with Customer JWT → 201 OK
- [ ] `GET /api/orders` with Customer JWT → 200 OK, returns user's orders
- [ ] `DELETE /api/orders/{orderId}` with Customer JWT → 200 OK

#### RestaurantOwner Endpoints
- [ ] `POST /api/restaurants/{id}/items` with Owner JWT → 201 OK
- [ ] `POST /api/restaurants/{id}/items` with Customer JWT → 403 Forbidden (CanManageRestaurant policy)

#### RBAC Verification
- [ ] Admin can create restaurants ✓
- [ ] Customer cannot create restaurants (403) ✓
- [ ] RestaurantOwner can add items to own restaurant ✓
- [ ] RestaurantOwner cannot add items to other restaurants (403) ✓
- [ ] Customer can create orders ✓
- [ ] Unauthenticated user gets 401 on protected endpoints ✓

#### Rate Limiting (New GetRequestIdentifier)
- [ ] Authenticated users rate limited by userId ✓
- [ ] Anonymous users rate limited by IP ✓
- [ ] Rate limit headers present (Retry-After) ✓

#### Error Handling
- [ ] Invalid JWT → 401 Unauthorized ✓
- [ ] Expired JWT → 401 Unauthorized ✓
- [ ] Malformed JWT → 401 Unauthorized ✓
- [ ] Missing Authorization header on protected endpoint → 401 ✓
- [ ] Insufficient permissions → 403 Forbidden ✓

### 2.11: Final Phase 2 Commit

**Before committing, ensure:**
- [ ] Build passes: `dotnet build api/RestaurantAPI.csproj`
- [ ] No compilation errors (55 warnings is OK)
- [ ] All endpoint tests pass
- [ ] RBAC tests pass
- [ ] Git status clean: `git status`
- [ ] Create detailed commit message (see template below)

**Commit Template:**
```
Phase 2.10-2.11: JWT Endpoint Testing + RBAC Verification Complete

TESTING SUMMARY
===============
✓ All 4 controllers verified with JWT Bearer auth
✓ RBAC policies enforced: Admin > RestaurantOwner > Customer
✓ Public endpoints accessible without auth (GET /restaurants, GET /menu)
✓ Protected endpoints require valid JWT + role policies
✓ Rate limiting by authenticated userId or IP
✓ Error responses consistent: 401 (Unauthorized), 403 (Forbidden), 404 (Not Found)

ENDPOINT COVERAGE
=================
- Public: 4 endpoints (GET restaurants/menu/items)
- Admin-only: 1 endpoint (POST create restaurant)
- RestaurantOwner: 1 endpoint (POST add items to menu)
- Customer: 5 endpoints (cart + orders)
- Auth: 4 endpoints (login, refresh, logout, change-password)

RBAC VALIDATION
===============
✓ Admin role: full access (POST restaurants, view all orders)
✓ RestaurantOwner role: can manage own restaurants
✓ Customer role: can create orders, manage own cart
✓ Resource-based authorization: CanModifyOwnResource handler verified
✓ CanManageRestaurant handler: owner + admin access verified

NEXT PHASE
==========
Ready for Phase A (SOLID refactoring):
- A.1: Kill dual auth system (JWT only)
- A.2: Unify password handling (IPasswordService only)
- A.3: Split services for SRP (MenuService, ItemService)
- And 5 more Phase A tasks...

Build: ✓ Pass (55 warnings, 0 errors)
Tests: ✓ All endpoint tests pass
Ready: ✓ Merge to main after review
```

---

## Team Alignment on Phase A Roadmap

### Discussion Points

1. **Confirm Goal:** Real Clean Architecture (multi-project) or stay N-layer in one assembly?
   - Recommendation: Multi-project (Phase B) = better long-term
   - Option to stop at Phase A if team prefers

2. **Dual Auth Decision:** Kill API-key-as-Usercode immediately?
   - Recommendation: Yes, Phase A.1 (2-3 days)
   - It's the biggest technical debt

3. **Timeline Feasibility:** 4 weeks for full CA (Phase A + B + C)?
   - Phase A: 2 weeks (in parallel with features if possible)
   - Phase B: 1 week (big move, focus week)
   - Phase C: 1 week (hardening)
   - Adjust based on team capacity

4. **Testing Strategy:** How aggressive?
   - Recommendation: Add tests as you refactor (TDD-ish)
   - Existing tests should pass throughout
   - New Application layer tests in Phase B

5. **Deployment Risk:** Can we push this to production incrementally?
   - Phase A: Yes, no API changes (internal refactoring only)
   - Phase B: Major refactoring, should be merged as one PR
   - Phase C: Additive (new patterns), low risk

---

## Create Phase A Feature Branch

**After Phase 2.10 testing is approved:**

```bash
# Ensure on main and up-to-date
git checkout main
git pull origin main

# Create Phase A branch
git checkout -b feature/phase-a-solid

# All Phase A work goes here
# Merge to main after each sub-phase (A.1, A.2, A.3, etc.)
# or one big commit at the end
```

---

## Documentation Tasks

### Update Project Docs

- [ ] **README.md:** Update to reflect JWT-only auth (remove API-key mentions)
- [ ] **ARCHITECTURE.md:** Create (or update existing) with:
  - Current state: N-layer + JWT + Auth module
  - Roadmap: Phase A (SOLID), Phase B (multi-project), Phase C (hardening)
  - Layer responsibilities
  - Example: how to add a new endpoint (with DTOs, not entities)

- [ ] **AUTH.md:** JWT workflow, token refresh, RBAC hierarchy, role definitions

### Create Decision Records (Optional but Good)

**ADR-001: Why Clean Architecture Multi-Project (Phase B)**
- **Problem:** Current N-layer in one assembly makes it hard to test, maintain, and reason about boundaries
- **Decision:** Move to Domain / Application / Infrastructure / API (4 projects)
- **Consequences:** Better testability, clear dependencies, easier to scale

**ADR-002: Why Kill Dual Auth (Phase A.1)**
- **Problem:** API-key + JWT systems cause confusion, code duplication, testing complexity
- **Decision:** JWT is now the only auth method; UserCode deprecated for business operations
- **Consequences:** Simpler code, clearer identity model, easier to audit

---

## File Checklist: Ready for Phase A

### To Review Before Phase A Starts

- [ ] Read: `CLEAN_ARCHITECTURE_ROADMAP.md` (full plan)
- [ ] Read: `ARCHITECTURE_ASSESSMENT.md` (current state + issues)
- [ ] Review: `api/Auth/Services/` (good patterns to replicate)
- [ ] Review: `api/Controllers/UserController.cs` (old, to be removed)
- [ ] Review: `api/Services/RestaurantService.cs` (SRP violation to split)
- [ ] Review: `api/Repositories/UserRepository.cs` (password hashing to move)

### To Create/Prepare for Phase A

- [ ] Create `PHASE_A_TASKS.md` (detailed task breakdown)
- [ ] Create `TESTING_STRATEGY_A.md` (how to ensure Phase A doesn't break things)
- [ ] Discuss with team: parallel feature development during Phase A?

---

## Immediate Blockers (Check These)

- [ ] Build still compiles ✓ (yes, Phase 2 compiled)
- [ ] No merge conflicts on main ✓ (yes, Phase 2 pushed to feature branch)
- [ ] Team agreement on Clean Architecture goal? (pending)
- [ ] Phase 2.10 test results documented? (pending)
- [ ] Any blocking bugs in current codebase? (check issue tracker)

---

## Success Criteria: End of This Week

✅ **By End of Friday:**
1. Phase 2.10 testing complete and documented
2. Final Phase 2 commit merged or ready to merge
3. Team has read + approved Clean Architecture roadmap
4. Phase A feature branch created (if approved)
5. Phase A.1 tasks identified and ready to start
6. Project documentation updated with new architecture plans

✅ **By End of Next Week:**
1. Phase A.1 (kill dual auth) complete + committed
2. Phase A.2 (unify password) complete + committed
3. Build passes, tests pass
4. No regressions

---

## Risks This Week

| Risk | Mitigation |
|------|-----------|
| Phase 2 testing finds major bugs | Stop, fix, re-test before proceeding to Phase A |
| Team wants to delay to Phase A | Document that dual auth debt grows weekly; act sooner |
| Someone starts a feature during refactoring | Use feature branch discipline; freeze features during Phase A.1-A.2 |

---

## Communication

### To Team
"We've completed Phase 2 (JWT endpoint migration + RBAC). Now we need to assess the architecture and plan Phase 3+. The good news: JWT is working. The challenging news: we have dual auth systems, SRP violations, and entity leakage that are increasing maintenance burden. I propose a 4-week refactor (Phase A-B-C) to build real Clean Architecture. See CLEAN_ARCHITECTURE_ROADMAP.md for details."

### To Stakeholders
"Security refactor is on track. Phase 2 (JWT) is complete. Next: internal code cleanup + architecture restructuring (4 weeks, no API changes visible to users). This makes future features faster and safer to build."

---

## Sign-Off Checklist

Before officially starting Phase A:

- [ ] Phase 2 testing complete (all endpoints verified)
- [ ] Phase 2 final commit ready
- [ ] Team read + approved CLEAN_ARCHITECTURE_ROADMAP.md
- [ ] Stakeholders informed of Phase A timeline (4 weeks)
- [ ] Feature freeze confirmed (no new features during Phase A.1-A.2)
- [ ] Phase A.1 tasks assigned / scheduled
- [ ] DI container / service registration strategy reviewed
- [ ] Test coverage plan approved

---

## Next Session Agenda

1. **Phase 2.10 Test Results:** Demo endpoint tests, RBAC verification
2. **Roadmap Q&A:** Address concerns about Phase A-C timeline
3. **Phase A.1 Kickoff:** Assign tasks, start killing dual auth
4. **Team Capacity:** How many devs on Phase A vs. feature work?
5. **Git Strategy:** Branch naming, merge frequency, code review process

---

**Owner:** You (architecture lead)  
**Status:** Pending team approval  
**Target Start:** Phase A.1 by next Monday  

