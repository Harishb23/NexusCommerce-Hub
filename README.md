# NexusCommerce-Hub
a production-grade full-stack platform that centralizes multi-channel data from Amazon and Noon into a unified dashboard. Built with ASP.NET Core 8, React 19, and Docker, it features resilient data syncing via Polly, real-time inventory alerts, and a Clean Architecture to ensure high-performance, containerized commerce management.


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
