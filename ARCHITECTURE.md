# Architecture Diagram (draw.io compatible XML)

Save as `.drawio` file and open with https://draw.io or the draw.io VS Code extension.

```xml
<mxfile host="app.diagrams.net">
  <diagram name="Commerce Ops Architecture">
    <mxGraphModel dx="1422" dy="762" grid="1" gridSize="10" guides="1" tooltips="1" connect="1" arrows="1" fold="1" page="1" pageScale="1" pageWidth="1169" pageHeight="827" math="0" shadow="0">
      <root>
        <mxCell id="0" />
        <mxCell id="1" parent="0" />

        <!-- Docker Compose boundary -->
        <mxCell id="2" value="Docker Compose Network" style="points=[[0,0],[0.25,0],[0.5,0],[0.75,0],[1,0],[1,0.25],[1,0.5],[1,0.75],[1,1],[0.75,1],[0.5,1],[0.25,1],[0,1],[0,0.75],[0,0.5],[0,0.25]];shape=mxgraph.flowchart.start_2;fillColor=#dae8fc;strokeColor=#6c8ebf;fontStyle=1;fontSize=14;" vertex="1" parent="1">
          <mxGeometry x="40" y="40" width="900" height="600" as="geometry" />
        </mxCell>

        <!-- Frontend -->
        <mxCell id="10" value="&lt;b&gt;Frontend&lt;/b&gt;&lt;br&gt;React 19 + Vite&lt;br&gt;Tailwind CSS&lt;br&gt;TanStack Query&lt;br&gt;Chart.js&lt;br&gt;&lt;br&gt;Port: 3000" style="rounded=1;whiteSpace=wrap;fillColor=#fff2cc;strokeColor=#d6b656;" vertex="1" parent="1">
          <mxGeometry x="80" y="200" width="160" height="140" as="geometry" />
        </mxCell>

        <!-- Nginx -->
        <mxCell id="11" value="nginx" style="rounded=1;fillColor=#f5f5f5;strokeColor=#666666;" vertex="1" parent="1">
          <mxGeometry x="80" y="360" width="160" height="40" as="geometry" />
        </mxCell>

        <!-- Backend -->
        <mxCell id="20" value="&lt;b&gt;Backend API&lt;/b&gt;&lt;br&gt;ASP.NET Core 8&lt;br&gt;Clean Architecture&lt;br&gt;Repository Pattern&lt;br&gt;AutoMapper&lt;br&gt;Polly Resilience&lt;br&gt;Serilog&lt;br&gt;&lt;br&gt;Port: 8080" style="rounded=1;whiteSpace=wrap;fillColor=#d5e8d4;strokeColor=#82b366;" vertex="1" parent="1">
          <mxGeometry x="340" y="160" width="200" height="200" as="geometry" />
        </mxCell>

        <!-- PostgreSQL -->
        <mxCell id="30" value="&lt;b&gt;PostgreSQL 16&lt;/b&gt;&lt;br&gt;Orders&lt;br&gt;Products&lt;br&gt;SyncLogs&lt;br&gt;IntegrationHealth&lt;br&gt;&lt;br&gt;Port: 5432" style="shape=cylinder3;whiteSpace=wrap;fillColor=#dae8fc;strokeColor=#6c8ebf;" vertex="1" parent="1">
          <mxGeometry x="660" y="160" width="180" height="180" as="geometry" />
        </mxCell>

        <!-- External APIs -->
        <mxCell id="40" value="&lt;b&gt;DummyJSON API&lt;/b&gt;&lt;br&gt;dummyjson.com/carts&lt;br&gt;Channel: Amazon" style="rounded=1;fillColor=#ffe6cc;strokeColor=#d79b00;" vertex="1" parent="1">
          <mxGeometry x="340" y="430" width="180" height="80" as="geometry" />
        </mxCell>

        <mxCell id="41" value="&lt;b&gt;FakeStore API&lt;/b&gt;&lt;br&gt;fakestoreapi.com/products&lt;br&gt;Channel: Noon" style="rounded=1;fillColor=#ffe6cc;strokeColor=#d79b00;" vertex="1" parent="1">
          <mxGeometry x="560" y="430" width="180" height="80" as="geometry" />
        </mxCell>

        <!-- Connections -->
        <mxCell id="50" edge="1" source="10" target="20" parent="1">
          <mxGeometry relative="1" as="geometry" />
          <mxCell value="REST /api/*" style="edgeLabel;" vertex="1" connectable="0" parent="50">
            <mxGeometry relative="0.5" as="geometry" />
          </mxCell>
        </mxCell>

        <mxCell id="51" edge="1" source="20" target="30" parent="1">
          <mxGeometry relative="1" as="geometry" />
          <mxCell value="EF Core" style="edgeLabel;" vertex="1" connectable="0" parent="51">
            <mxGeometry relative="0.5" as="geometry" />
          </mxCell>
        </mxCell>

        <mxCell id="52" edge="1" source="20" target="40" parent="1">
          <mxGeometry relative="1" as="geometry" />
          <mxCell value="HttpClient + Polly" style="edgeLabel;" vertex="1" connectable="0" parent="52">
            <mxGeometry relative="0.5" as="geometry" />
          </mxCell>
        </mxCell>

        <mxCell id="53" edge="1" source="20" target="41" parent="1">
          <mxGeometry relative="1" as="geometry" />
          <mxCell value="HttpClient + Polly" style="edgeLabel;" vertex="1" connectable="0" parent="53">
            <mxGeometry relative="0.5" as="geometry" />
          </mxCell>
        </mxCell>

        <!-- Backend Layers -->
        <mxCell id="60" value="Controllers → Services → Repositories → DbContext" style="text;html=1;align=center;verticalAlign=middle;" vertex="1" parent="1">
          <mxGeometry x="280" y="400" width="320" height="20" as="geometry" />
        </mxCell>

      </root>
    </mxGraphModel>
  </diagram>
</mxfile>
```

## Layer Descriptions

### Frontend (React 19)
- **Pages**: Route-level components with full loading/error/empty state handling
- **Hooks**: TanStack Query wrappers — all server state lives here
- **Components**: Stateless presentational atoms (StatCard, StatusBadge, etc.)
- **Services**: Axios client with base URL, timeout, and error normalization

### Backend (ASP.NET Core 8)
- **Controllers**: Thin — parse request, delegate to service, return response
- **Services**: All business logic lives here. No EF Core directly.
- **Repositories**: Data access abstraction over EF Core. Enables mocking in tests.
- **Channel Services**: One per external API. Isolated failure domains.
- **Middleware**: Global exception handler — maps exceptions to HTTP status codes
- **Polly**: Exponential backoff retry (3 attempts) + circuit breaker (5 failures → 30s open) per HttpClient

### Database
- PostgreSQL 16 with EF Core migrations
- Indexes on frequently-queried columns (Channel, Status, ExternalOrderId, CreatedAt)
- Unique constraint on IntegrationHealths.Channel (upsert semantics)

### Data Flow: Sync Operation

```
POST /api/sync
    │
    ▼
SyncController.Sync()
    │
    ▼
SyncService.SyncAllChannelsAsync()
    ├── AmazonChannelService.FetchOrdersAsync()
    │       │── GET dummyjson.com/carts (with Polly retry)
    │       └── Map to Order entities
    │
    ├── NoonChannelService.FetchProductsAsync()
    │       │── GET fakestoreapi.com/products (with Polly retry)
    │       └── Map to Product entities
    │
    ├── Deduplication via ExternalOrderId + Channel
    ├── Persist new records to PostgreSQL
    ├── Record SyncLog (Success/Failed)
    └── Update IntegrationHealth (upsert)
```
