# Engineering Decisions

## Trade-offs Made

### 1. Single Project vs. Multi-Project Solution

**Decision**: Used a single ASP.NET project with folder-based separation rather than multiple projects (e.g., Core, Infrastructure, API).

**Trade-off**: Reduces build complexity and Docker layer count. Faster iteration in a small-team context.  
**Cost**: Weaker compile-time enforcement of layer boundaries — a controller could accidentally import a repository directly. In production, this would use separate projects to enforce isolation.

**Production improvement**: Split into `CommerceOps.Core` (entities, interfaces, DTOs), `CommerceOps.Infrastructure` (EF Core, external services), and `CommerceOps.API` (controllers, middleware).

---

### 2. Polly v7 over Polly v8

**Decision**: Used Polly v7 (`WaitAndRetryAsync` / `CircuitBreakerAsync`) rather than Polly v8's `ResiliencePipeline`.

**Trade-off**: v7's fluent API is more readable and widely documented. v8 is the future but has a steeper learning curve for contributors.  
**Cost**: v7 is in maintenance mode. New resilience strategies (rate limiter, timeout, hedging) are v8-only.

**Production improvement**: Migrate to Polly v8 `ResiliencePipelineBuilder` with named pipelines per channel, including hedging for read-heavy operations and per-channel rate limiter.

---

### 3. Random Stock Values for Noon Products

**Decision**: FakeStore API only provides product data, not inventory. Stock levels are randomly generated during sync.

**Trade-off**: Allows the full inventory monitoring feature to be demonstrated without a real inventory source.  
**Cost**: Stock values are not meaningful — they reset on each full sync.

**Production improvement**: Maintain a separate inventory ledger table with append-only stock movement records. Sync creates/updates the base product; stock is managed independently via a dedicated inventory adjustment API.

---

## Production Improvements

1. **Authentication & Authorization**: Add JWT auth with role-based access (admin, viewer). Use ASP.NET Core Identity or an external IdP (Auth0/Keycloak).

2. **Background Sync Jobs**: Replace manual sync triggers with Hangfire or Quartz.NET scheduled jobs per channel. Configurable per-channel cron intervals via admin UI.

3. **Webhook Support**: Instead of polling, expose webhook endpoints so channels can push updates in real-time.

4. **Caching Layer**: Add Redis for dashboard stats (cache 30s), integration health (cache 10s), and analytics (cache 5min). Reduces DB load at high query frequency.

5. **Rate Limiting**: Per-channel per-minute rate limits using ASP.NET Core RateLimiter middleware. Protects against external API throttling cascading into internal failures.

6. **Database Migrations on Startup**: Production should use a pre-deployment migration step (CI/CD) rather than `Migrate()` in startup code, which causes downtime risk during rolling deploys.

7. **Observability**: Add OpenTelemetry with distributed tracing across HTTP calls. Export spans to Jaeger or Datadog. Add custom metrics (sync duration, order sync rate, failure rate) as Prometheus counters.

8. **Multi-tenancy**: Each merchant gets an isolated tenant. Data model gets `TenantId` on all tables. Row-level security in PostgreSQL.

9. **Event-driven Architecture**: Publish domain events (OrderStatusChanged, SyncCompleted) to a message bus (RabbitMQ/Kafka) for downstream consumers (notifications, audit log, analytics pipeline).

10. **Frontend**: Add React Error Boundaries, skeleton loading states, toast notifications for sync results, and a real-time integration health polling indicator using SSE or WebSocket.

---

## Known Limitations

1. **No authentication** — any user can access all data and trigger syncs.
2. **Sync is blocking** — large datasets will delay the HTTP response. Should be async via background jobs.
3. **Stock is random** — Noon channel stock values are not real inventory data.
4. **No pagination on analytics** — could be slow with large datasets without time-range filtering.
5. **Single-region** — no geographic redundancy or read replicas.
6. **Migration runs on startup** — `db.Database.Migrate()` in `Program.cs` is safe for single-instance deployments but risky during rolling deploys.
7. **No audit trail** — order status changes are not recorded with timestamps or actor information.

---

## Testing Strategy

### Unit Tests (implemented)
- Service layer tests using Moq for all repository/service dependencies
- Covers: happy path, error cases, validation, deduplication logic
- `OrderServiceTests`: status validation, not-found handling, pagination
- `SyncServiceTests`: channel success/failure, deduplication, unknown channel

### Integration Tests (recommended)
- Use `Microsoft.EntityFrameworkCore.InMemory` or a real Postgres test container (`Testcontainers.PostgreSql`) for repository-layer integration tests
- API-level tests using `WebApplicationFactory<Program>` with seeded test data
- Tests for: sync idempotency, order filter combinations, health endpoint behavior

### Contract Tests (recommended)
- Record DummyJSON and FakeStore API responses, replay in tests to avoid external API dependency
- Use WireMock.Net to simulate external API failures (timeout, 429, 500)

### Load Tests (recommended)
- k6 or NBomber scripts for sync endpoint under concurrent load
- Validate Polly circuit breaker activates correctly under sustained failures
