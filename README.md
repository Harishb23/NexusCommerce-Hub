# Commerce Operations Command Center

A production-quality commerce operations platform for managing orders, inventory, integrations, and analytics across multiple commerce channels from a single dashboard.

## Quick Start

```bash
docker compose up --build
```

Then open:
- **Frontend**: http://localhost:3000
- **Backend API**: http://localhost:8080
- **Swagger UI**: http://localhost:8080/swagger

## Architecture Overview

```
┌─────────────────────────────────────────────────────────┐
│                      Docker Network                       │
│                                                           │
│  ┌──────────────┐    ┌──────────────┐    ┌────────────┐ │
│  │   Frontend   │───▶│   Backend    │───▶│ PostgreSQL │ │
│  │  React 19    │    │  ASP.NET 8   │    │            │ │
│  │  Port: 3000  │    │  Port: 8080  │    │ Port: 5432 │ │
│  └──────────────┘    └──────────────┘    └────────────┘ │
│         │                   │                            │
│         │ nginx proxy       │ Polly retry                │
│         ▼                   ▼                            │
│   /api/* → backend   DummyJSON API                       │
│                       FakeStore API                      │
└─────────────────────────────────────────────────────────┘
```

### Backend Structure (Clean Architecture)

```
backend/src/CommerceOps.API/
├── Controllers/          # HTTP layer — no business logic
├── Services/             # Business logic
├── Interfaces/           # Contracts (DI boundaries)
├── Repositories/         # Data access layer
├── Models/               # EF Core entities
├── DTOs/                 # Request/response contracts
├── Mappings/             # AutoMapper profiles
├── Data/                 # DbContext + Migrations
├── Middleware/           # Cross-cutting concerns
└── Configurations/       # External config
```

### Frontend Structure

```
frontend/src/
├── pages/                # Route-level components
├── components/           # Reusable UI primitives
├── layouts/              # Shell/navigation
├── hooks/                # React Query data hooks
├── services/             # Axios API client
└── types/                # TypeScript interfaces
```

## Prerequisites

- Docker Desktop (with compose v2)
- Internet access (for external API calls to DummyJSON and FakeStore)

## Running Without Docker

### Backend

```bash
cd backend/src/CommerceOps.API
dotnet restore
dotnet run
```

Requires PostgreSQL running locally. Update `appsettings.Development.json` with your connection string.

### Frontend

```bash
cd frontend
npm install
npm run dev
```

Proxies `/api/*` to `http://localhost:8080`.

## Environment Variables

| Variable | Default | Description |
|---|---|---|
| `ConnectionStrings__DefaultConnection` | (see appsettings.json) | PostgreSQL connection string |
| `AllowedOrigins` | `http://localhost:3000` | CORS allowed origins |
| `Inventory__LowStockThreshold` | `10` | Units below which stock is "low" |
| `ASPNETCORE_ENVIRONMENT` | `Docker` | ASP.NET environment |
| `VITE_API_URL` | `/api` | Frontend API base URL |

## API Endpoints

### Dashboard
| Method | Endpoint | Description |
|---|---|---|
| GET | `/api/dashboard` | Aggregate stats |

### Orders
| Method | Endpoint | Description |
|---|---|---|
| GET | `/api/orders` | List orders (paginated, filterable) |
| GET | `/api/orders/{id}` | Single order |
| PUT | `/api/orders/{id}/status` | Update order status |
| GET | `/api/orders/search?q=` | Search orders |

### Inventory
| Method | Endpoint | Description |
|---|---|---|
| GET | `/api/inventory` | All products |
| GET | `/api/inventory/low-stock` | Products below threshold |
| GET | `/api/inventory/stats` | Inventory statistics |

### Integrations
| Method | Endpoint | Description |
|---|---|---|
| GET | `/api/integrations/health` | Integration health status |

### Sync
| Method | Endpoint | Description |
|---|---|---|
| POST | `/api/sync` | Trigger sync (all or specific channel) |
| GET | `/api/sync/logs` | Sync history |
| GET | `/api/sync/failed` | Failed syncs only |
| POST | `/api/sync/retry/{id}` | Retry a failed sync |

### Analytics
| Method | Endpoint | Description |
|---|---|---|
| GET | `/api/analytics/orders` | Orders by channel + status |
| GET | `/api/analytics/revenue` | Revenue by channel |
| GET | `/api/analytics/inventory-health` | Inventory health breakdown |

### Health
| Method | Endpoint | Description |
|---|---|---|
| GET | `/health` | API + DB health check |

## Data Sources

| Channel | Source | Data Type |
|---|---|---|
| Amazon | `https://dummyjson.com/carts` | Orders |
| Noon | `https://fakestoreapi.com/products` | Products/Inventory |

## Running Tests

```bash
cd backend
dotnet test tests/CommerceOps.Tests/CommerceOps.Tests.csproj
```

## Tech Stack

| Layer | Technology |
|---|---|
| Frontend | React 19, Vite, TypeScript, Tailwind CSS |
| State Management | TanStack Query v5 |
| Charts | Chart.js + react-chartjs-2 |
| Backend | ASP.NET Core 8 Web API |
| ORM | Entity Framework Core 8 |
| Database | PostgreSQL 16 |
| HTTP Resilience | Polly (retry + circuit breaker) |
| Logging | Serilog |
| Mapping | AutoMapper |
| Testing | xUnit, Moq, FluentAssertions |
| Container | Docker + Docker Compose |
