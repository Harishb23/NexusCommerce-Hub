# NexusCommerce-Hub
a production-grade full-stack platform that centralizes multi-channel data from Amazon and Noon into a unified dashboard. Built with ASP.NET Core 8, React 19, and Docker, it features resilient data syncing via Polly, real-time inventory alerts, and a Clean Architecture to ensure high-performance, containerized commerce management.


# Commerce Operations Command Center 🚀

A production-grade, full-stack commerce operations platform designed to centralize and scale multi-channel management. It aggregates data from **Amazon** and **Noon** into a unified dashboard for real-time order management, inventory health, and resilient data syncing.

---

## 🏗️ Architecture & Tech Stack

Built using **Clean Architecture** patterns to ensure business logic remains independent of external API volatility.

| Layer | Technology |
|---|---|
| **Frontend** | React 19, Vite, TypeScript, Tailwind CSS, TanStack Query v5 |
| **Backend** | ASP.NET Core 8 Web API |
| **Data Layer** | Entity Framework Core 8, PostgreSQL 16 |
| **Resilience** | Polly (Circuit Breaker, Retry Policies) |
| **Infrastructure** | Docker, Docker Compose |
| **Observability** | Serilog (Structured Logging) |

---

## ⚡ Quick Start

### 1. Prerequisites
- Docker Desktop (with Compose v2)
- .NET 8 SDK (optional, for local development)

### 2. Run with Docker
```bash
docker compose up --build
```
Once the containers are healthy, open:
- **Frontend**: [http://localhost:3000](http://localhost:3000)
- **Backend API**: [http://localhost:8080](http://localhost:8080)
- **Swagger UI**: [http://localhost:8080/swagger](http://localhost:8080/swagger)

---

## ✨ Key Features

- **Omni-Channel Synchronization**: Aggregates real-time data from diverse providers (DummyJSON and FakeStore API).
- **Operational Resilience**: Uses **Polly** to prevent cascading failures. If one channel is down, the rest of the dashboard remains functional.
- **Inventory Intelligence**: Automated monitoring with real-time "Low Stock" alerts across all channels.
- **Integration Health**: A dedicated monitoring service that tracks the response times and failure rates of third-party APIs.

---

## 🧠 Engineering Judgment (Stage 5)

### Trade-offs Made
1. **Property-based Records vs. Primary Constructors**: I transitioned from primary constructors to explicit `{ get; init; }` properties in DTOs to ensure 100% compatibility with Reflection-based serializers and AutoMapper, prioritizing reliability over syntax sugar.
2. **Single-Project Clean Architecture**: For this scale, I implemented layer separation via folders rather than multiple projects to reduce Docker image layers and build complexity, while maintaining a clear separation of concerns.
3. **Synchronous vs. Async Syncing**: The current sync is triggered via HTTP POST. For an MVP, this provides immediate feedback. In a production environment, this would be refactored into a background job (Hangfire) to support larger datasets.

### What's Next for Production?
- **Authentication**: Implementing JWT-based role-based access control.
- **Background Jobs**: Moving the sync logic out of the request/response cycle.
- **Observability**: Adding OpenTelemetry for distributed tracing across external channel calls.

---

## 🛠️ Testing Strategy
- **Unit Tests**: Coverage for core Service logic using Moq for dependency isolation.
- **Integration Tests**: (Recommended) Use Testcontainers to run API-level validation against a real PostgreSQL instance.
- **Contract Tests**: Verify interactions with external commerce APIs to prevent breaking changes from upstream providers.

---

## 📝 License
MIT License.
