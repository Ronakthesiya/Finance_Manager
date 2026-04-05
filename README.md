# 💰 Finance Manager — ASP.NET Core Backend

> A production-ready RESTful backend API for secure, multi-user financial management with fine-grained RBAC, Redis-backed session management, structured logging, rate limiting, and rich dashboard analytics.

**Zorvyn Backend Developer Intern Assignment** · Built by **Ronak Thesiya** (Ref: `TE55EQAO`)

---

## 🛠️ Tech Stack

![ASP.NET Core](https://img.shields.io/badge/ASP.NET_Core-8.0-512BD4?logo=dotnet)
![MySQL](https://img.shields.io/badge/MySQL-8.0-4479A1?logo=mysql&logoColor=white)
![Redis](https://img.shields.io/badge/Redis-StackExchange-DC382D?logo=redis&logoColor=white)
![Serilog](https://img.shields.io/badge/Serilog-Structured_Logging-informational)
![JWT](https://img.shields.io/badge/JWT-Bearer_Auth-F7B731?logo=jsonwebtokens&logoColor=white)

| Technology | Purpose |
|---|---|
| ASP.NET Core 8 | Web framework & middleware pipeline |
| MySQL | Primary relational database |
| Redis (StackExchange) | Session store & permission cache |
| ServiceStack OrmLite | Lightweight micro-ORM |
| Serilog | Structured multi-sink logging |
| JWT Bearer | Stateless authentication |
| BCrypt.Net | Password hashing |
| Swashbuckle | Swagger / OpenAPI UI |

---

## ✨ Features

| Feature | Description |
|---|---|
| **JWT Authentication** | Short-lived access tokens (15 min) + Redis refresh tokens (24 h) |
| **RBAC Authorization** | Role → Permission slugs enforced via a custom `IAsyncActionFilter` |
| **Rate Limiting** | Token Bucket per IP (global 100/min) + tighter `AuthPolicy` on login (5/min) |
| **Pagination & Filtering** | Generic `PagedData<T>`, `PaginationQuery`, `TransactionFilterQuery` |
| **Structured Logging** | Serilog → Console, rolling Files, and MySQL simultaneously |
| **Global Exception Handling** | Middleware catches all unhandled exceptions, maps to HTTP codes |
| **API Versioning** | URL-based versioning, ready for v2 without breaking v1 clients |
| **Custom DTO↔POCO Mapper** | Reflection-based mapper with enum ↔ string and type coercion |
| **Redis Caching** | Permissions cached per user (10 min), refresh sessions (24 h) |
| **Soft Deletes** | `IsDeleted` flag on all entities — no records permanently removed |

---

## 🏗️ Solution Architecture

The solution follows a **clean layered architecture** split into 5 focused projects:

```
Finance_Manager (Solution)
├── Finance_Manager_API        → Presentation Layer (Controllers, Filters, Middleware)
├── Finance_Manager_BAL        → Business Access Layer (Handlers, PermissionService)
├── Finance_Manager_DAL        → Data Access Layer (OrmLite Contexts, Interfaces)
├── Finance_Manager_Core       → Shared Infrastructure (JWT, Redis, Mapper, Middleware)
└── Finance_Manager_MAL        → Model Access Layer (DTOs, POCOs, Enums)
```

### Layer Dependency Rule

```
API ──► BAL (IBL* interfaces)
         │
         └──► DAL (IDL* interfaces)
                │
                └──► MySQL (via OrmLite)

All layers   ──► MAL (DTOs, POCOs, Enums)
API + BAL    ──► Core (JwtService, RedisService, Middleware)
```

Each layer depends only on the **interface abstraction** of the layer below — never on a concrete class. This makes every layer independently testable and swappable.

---

## 📁 Folder Structure

<details>
<summary>Click to expand full folder tree</summary>

```
Finance_Manager (Solution)
├── Finance_Manager_API/
│   ├── Controllers/
│   │   ├── CLAuthController.cs
│   │   ├── CLCategoryController.cs
│   │   ├── CLDashboardController.cs
│   │   ├── CLPermissionController.cs
│   │   ├── CLRoleController.cs
│   │   ├── CLTransactionController.cs
│   │   └── CLUserController.cs
│   ├── Filters/
│   │   └── PermissionAttribute.cs     ← IAsyncActionFilter (RBAC)
│   ├── Program.cs                     ← DI, Middleware, JWT, Rate Limiting
│   └── appsettings.json
│
├── Finance_Manager_BAL/
│   ├── Handler/
│   │   ├── BLAuthHandler.cs
│   │   ├── BLCategoryHandler.cs
│   │   ├── BLDashboardHandler.cs
│   │   ├── BLPermissionHandler.cs
│   │   ├── BLRoleHandler.cs
│   │   ├── BLTransactionHandler.cs
│   │   └── BLUserHandler.cs
│   ├── Interfaces/
│   │   └── IBL*.cs                    ← One interface per handler
│   └── PermissionService.cs           ← RBAC validation + Redis session check
│
├── Finance_Manager_DAL/
│   ├── Context/
│   │   └── DL*.cs                     ← One context per feature
│   ├── Interfaces/
│   │   └── IDL*.cs
│   └── Models/
│       ├── RoleWithPermissions.cs
│       └── UserWithRoles.cs
│
├── Finance_Manager_Core/
│   ├── Services/
│   │   ├── JwtService.cs
│   │   ├── RedisService.cs
│   │   └── DTOPOCOMapper.cs
│   ├── Interface/
│   │   └── IDTOPOCOMapper.cs
│   └── Middleware/
│       └── ExceptionMiddleware.cs
│
└── Finance_Manager_MAL/
    ├── DTO/
    │   ├── Core/          ← ApiResponse<T>, PagedData<T>, PaginationQuery
    │   ├── Auth/          ← DTOLoginRequest, DTOSignupRequest, DTOAuthResponse
    │   ├── Transaction/   ← DTOTransaction, TransactionFilterQuery
    │   ├── Role/          ← DTORole, DTORoleCreate, DTORoleUpdate
    │   ├── User/          ← DTOUser, DTOUserCreate, DTOUserUpdate
    │   ├── Category/
    │   ├── Permission/
    │   └── Dashboard/     ← DTODashboardSummary, DTOMonthlyStats
    ├── POCO/              ← User, Role, Permission, Transaction, Category
    └── Enums/             ← TransactionType, TransactionStatus, CategoryType
```
</details>

---

## 🔄 HTTP Request Pipeline

Every inbound request flows through this exact middleware chain:

```
Client (HTTP request)
    │
    ▼
Rate Limiter          → 429 if exceeded
    │
    ▼
Exception Middleware  → wraps ALL layers below (catch → log → JSON error)
    │
    ▼
JWT Authentication    → validates signature + lifetime
    │
    ▼
Controller [Authorize]
    │
    ▼
[Permission("x:y")] Filter  → 403 if permission check fails
    │
    ▼
BAL Handler           → business logic
    │
    ▼
Redis Cache           → HIT: return fast | MISS: continue to DAL
    │
    ▼
DAL Context (OrmLite) → typed queries → MySQL
    │
    ▼
HTTP Response         ← ApiResponse<T> JSON envelope
```

---

## 🔐 Authentication & Session Management

### JWT Auth Flow

| Step | Action | Detail |
|---|---|---|
| 1 | `POST /auth/login` | Client sends email + password |
| 2 | Password verify | `BCrypt.Verify(plaintext, storedHash)` |
| 3 | Generate tokens | JWT (15 min access) + GUID refresh token |
| 4 | Store session | Redis key `session:{userId}:{refreshToken}` → 24h TTL |
| 5 | Response | Returns `{ accessToken, refreshToken }` |
| 6 | Subsequent calls | `Authorization: Bearer <accessToken>` |
| 7 | `POST /auth/refresh` | Validates refresh in Redis → rotates both tokens |
| 8 | `POST /auth/logout` | Deletes Redis session key → invalidates refresh token |

### Redis Key Patterns

| Key Pattern | TTL | Purpose |
|---|---|---|
| `session:{userId}:{refreshToken}` | 24 hours | Refresh token session store |
| `permissions:{userId}` | 10 minutes | Cached RBAC permission slug list |

### Token Rotation — Replay Attack Protection

When a refresh token is used, the old session key is **immediately deleted** (DEL) before a new one is created (SET). If an attacker reuses a stolen refresh token, the Redis SCAN returns no match → `401`.

---

## 🛡️ Permission-Based Authorization (RBAC)

Users hold **Roles**; Roles hold **Permissions**. Each permission is identified by a `resource:action` slug enforced via a custom `IAsyncActionFilter`.

### Permission Slugs

| Slug | Meaning |
|---|---|
| `transaction:view` | Read transaction list and details |
| `transaction:create` | Create a new transaction |
| `transaction:update` | Edit or approve/reject transactions |
| `transaction:delete` | Soft delete transactions |
| `category:view / create / update / delete` | Category management |
| `role:view / create / update / delete` | Role management |
| `user:view / create / update / delete` | User management |
| `dashboard:view` | Access all dashboard analytics endpoints |

### Usage

```csharp
// Per-action (granular)
[HttpGet]
[Permission("transaction:view")]
public async Task<IActionResult> GetAll(...) { }

// Per-controller (all actions share same permission)
[Permission("dashboard:view")]
public class CLDashboardController : ControllerBase { }
```

### Validation Steps (PermissionService)

1. Extract `userId` from JWT claims
2. Verify active Redis session exists (`session:{userId}:*`)
3. Load permission list from Redis cache (`permissions:{userId}`)
4. On cache miss → query DB → cache for 600s
5. Check if required permission slug is in the list

---

## 🚦 Rate Limiting

Built on .NET 8's native `System.Threading.RateLimiting` — no third-party library.

| Policy | Limit | Target |
|---|---|---|
| Global Token Bucket | 100 req/min per IP | All endpoints |
| `AuthPolicy` | 5 req/min per IP | `POST /auth/login` |

- Exceeding the limit returns `429 Too Many Requests` with a `Retry-After: 60` header
- Rate limit events are logged via Serilog with client IP, path, and method

---

## 📊 Dashboard Endpoints

All dashboard endpoints require `dashboard:view` permission.

| Endpoint | Description |
|---|---|
| `GET /dashboard/summary` | Total income, expense, net balance |
| `GET /dashboard/monthly` | Monthly income vs expense (chart data) |
| `GET /dashboard/category-expense` | Expense breakdown by category |
| `GET /dashboard/recent-transactions` | N most recent transactions |
| `GET /dashboard/daily` | Daily income/expense (date range filterable) |
| `GET /dashboard/top-expenses` | Top N spending transactions |
| `GET /dashboard/pending-summary` | Count and total of pending transactions |

---

## 📄 Pagination & Filtering

All list endpoints return a `PagedData<T>` envelope. Page size is hard-capped at **100**.

### Example Request

```
GET /api/v1/transactions?pageNumber=2&pageSize=10&startDate=2025-01-01&type=Expense&searchTerm=rent
```

### Example Response

```json
{
  "success": true,
  "message": null,
  "data": {
    "items": [{ "id": 42, "title": "Office Rent", "amount": 25000.00 }],
    "totalCount": 245,
    "pageNumber": 2,
    "pageSize": 10,
    "totalPages": 25
  }
}
```

### Available Transaction Filters

| Filter | Type | Description |
|---|---|---|
| `startDate` / `endDate` | DateTime | Date range filter |
| `categoryId` | long | Filter by category |
| `type` | `Income` / `Expense` | Transaction type |
| `status` | `Pending` / `Approved` / `Rejected` | Transaction status |
| `searchTerm` | string | Searches `Title` + `Note` fields |

---

## 📝 Logging (Serilog)

Three sinks run simultaneously on every log event:

| Sink | Used For | Detail |
|---|---|---|
| Console | Development | Immediate real-time feedback |
| File (daily rolling) | Production ops | 7-day retention at `logs/log-YYYYMMDD.txt` |
| MySQL (`log` table) | Audit & compliance | Queryable — filter by user, IP, timeframe |

HTTP request logging produces entries like:
```
2025-04-05 12:00:01 [INF] HTTP GET /api/v1/transactions responded 200 in 45ms
```

---

## ⚠️ Global Exception Handling

`ExceptionMiddleware` wraps the entire pipeline. Internal error details are **never** exposed to clients.

| Exception Type | HTTP Status | Client Message |
|---|---|---|
| `ArgumentException` | 400 Bad Request | Validation message (safe to expose) |
| `KeyNotFoundException` | 404 Not Found | Not found message (safe to expose) |
| `UnauthorizedAccessException` | 401 Unauthorized | Generic message |
| Any other | 500 Internal Server Error | Generic message (full stack trace logged only) |

---

## 🗄️ Database Entities

| Entity | Key Fields |
|---|---|
| `User` | Id, Name, Email, PasswordHash, IsActive, IsDeleted, CreatedAt, UpdatedAt |
| `Role` | Id, Name, Slug, Description, IsActive, IsDeleted |
| `Permission` | Id, Name, Slug |
| `UserRole` | UserId, RoleId (junction) |
| `RolePermission` | RoleId, PermissionId (junction) |
| `Category` | Id, Name, Type, IsDeleted, CreatedAt, UpdatedAt |
| `Transaction` | Id, Title, Amount, Type, Status, TransactionDate, Note, CategoryId, CreatedBy, ApprovedBy, IsDeleted |

**Soft Deletes:** All entities use an `IsDeleted` flag. Hard deletes are never performed, preserving full audit history.

---

## 🔌 Complete API Reference

**Base URL:** `https://localhost:{port}/api/v1`  
**Auth header:** `Authorization: Bearer <token>`

<details>
<summary>Auth — /auth</summary>

| Method | Endpoint | JWT | Rate Limit | Description |
|---|---|---|---|---|
| POST | `/auth/signup` | No | — | Register new user |
| POST | `/auth/login` | No | 5/min | Login → access + refresh tokens |
| POST | `/auth/refresh` | No | — | Rotate tokens |
| POST | `/auth/logout` | No | — | Invalidate Redis session |

</details>

<details>
<summary>Transactions — /transactions</summary>

| Method | Endpoint | Permission | Description |
|---|---|---|---|
| GET | `/transactions` | `transaction:view` | Paginated + filtered list |
| GET | `/transactions/{id}` | `transaction:view` | Get by ID |
| POST | `/transactions` | `transaction:create` | Create (status = Pending) |
| PUT | `/transactions/{id}` | `transaction:update` | Update details |
| PATCH | `/transactions/{id}/status` | `transaction:update` | Approve or Reject |
| DELETE | `/transactions/{id}` | `transaction:delete` | Soft delete |

</details>

<details>
<summary>Categories, Roles, Users, Permissions, Dashboard</summary>

**Categories** — CRUD at `/categories` (permissions: `category:*`)  
**Roles** — CRUD + permission assignment at `/roles` (permissions: `role:*`)  
**Users** — CRUD at `/users` (permissions: `user:*`)  
**Permissions** — Read-only at `/permissions` (permissions: `permission:view`)  
**Dashboard** — 7 analytics endpoints at `/dashboard` (permission: `dashboard:view`)

</details>

---

## 📦 NuGet Packages

| Package | Version | Purpose |
|---|---|---|
| `Asp.Versioning.Mvc` | 8.1.1 | URL-based API versioning |
| `Microsoft.AspNetCore.Authentication.JwtBearer` | 8.0.25 | JWT Bearer middleware |
| `ServiceStack.OrmLite.MySql` | 10.0.6 | MySQL dialect for OrmLite |
| `StackExchange.Redis` | transitive | High-performance Redis client |
| `Serilog.AspNetCore` | 10.0.0 | Serilog + ASP.NET Core integration |
| `Serilog.Sinks.MySQL` | 5.0.0 | MySQL logging sink |
| `Swashbuckle.AspNetCore` | 6.4.0 | Swagger / OpenAPI UI |
| `Newtonsoft.Json` | 13.0.5 | JSON serialization (Redis) |
| `BCrypt.Net-Next` | transitive | Password hashing |
| `System.Threading.RateLimiting` | Built-in | Token Bucket rate limiter (.NET 8) |

---

## 🧠 Design Decisions

- **Interface-Driven Architecture** — Every layer exposes an interface (`IBL*`, `IDL*`). Concrete classes are registered only in `Program.cs`, keeping all layers independently testable.
- **Redis for Two Concerns** — Handles both refresh token sessions (24h TTL) and permission caching (10min TTL). Pattern-based key lookup (`session:{uid}:*`) enables token rotation without storing userId separately.
- **Filter vs Middleware for Auth** — JWT uses built-in middleware (broad, stateless). RBAC uses `IAsyncActionFilter` (fine-grained, per-action) so different endpoints can declare different permission requirements as simple attributes.
- **Dual-Purpose Rate Limiting** — Global limiter protects all endpoints at IP level. Named `AuthPolicy` specifically limits login attempts as a simple, effective brute-force defence.
- **Soft Deletes Everywhere** — `IsDeleted = true` instead of `DELETE SQL` preserves full audit history. The Serilog MySQL sink further captures system events.
- **Custom Mapper vs AutoMapper** — The reflection-based `DTOPOCOMapper` avoids an external dependency, handles enum↔string conversion needed by OrmLite, and keeps the codebase self-contained.
- **Dashboard SQL Aggregations** — Monthly stats, category breakdowns, and daily trends use single aggregate queries with `DATE_FORMAT + CASE WHEN SUM(...)` — efficient with no in-memory grouping.

---

## 🚀 Getting Started

### Prerequisites

- .NET 8 SDK
- MySQL 8.0+
- Redis server

### Configuration

Update `appsettings.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=financemanager;User=root;Password=yourpassword;",
    "Redis": "localhost:6379"
  },
  "Jwt": {
    "Key": "your-secret-key-min-32-chars",
    "Issuer": "FinanceManager",
    "Audience": "FinanceManagerClient",
    "ExpiryMinutes": "15"
  }
}
```

### Run

```bash
# Restore dependencies
dotnet restore

# Run the API project
dotnet run --project Finance_Manager_API
```

Swagger UI will be available at `https://localhost:{port}/swagger`.

---

## 📋 Standardized API Response

Every endpoint returns a consistent `ApiResponse<T>` envelope:

```json
// Success with data
{ "success": true, "message": null, "data": { ... } }

// Success with message
{ "success": true, "message": "Operation completed.", "data": null }

// Error
{ "success": false, "message": "Access Denied", "data": null }
```

---

*Finance Manager · ASP.NET Core 8 · Ronak Thesiya · Zorvyn Backend Developer Intern Assignment*
