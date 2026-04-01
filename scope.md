# Scope: ChromaDB + .NET Blazor Interactive Sample

## Purpose

A learning application that demonstrates semantic similarity search by integrating ChromaDB (a vector database) with a .NET 8 Blazor Web App. The app lets users seed documents and query them using natural language, with results ranked by semantic similarity rather than keyword matching.

---

## In Scope

### Features

- **Document Seeding** — One-click population of ChromaDB with 10 predefined documents covering .NET, Blazor, ChromaDB, embeddings, Docker, Entity Framework, ASP.NET Core, LINQ, and semantic search.
- **Semantic Search** — Free-text query input that returns the top 5 most similar documents based on embedding distance scores.
- **Results Display** — Each result shows the document text, similarity distance, and metadata tags (category and topic).
- **Health Awareness** — UI surfaces errors when ChromaDB is unreachable.
- **Docker Deployment** — Single `docker-compose up` command starts both the webapp and ChromaDB.

### Technical Boundaries

| Layer | Technology |
|---|---|
| UI | .NET 8 Blazor Web App (Interactive Server Rendering) |
| Language | C# |
| Vector Database | ChromaDB (REST API) |
| HTTP Client | `HttpClient` + `System.Net.Http.Json` |
| Containerization | Docker & Docker Compose |
| Styling | Custom CSS (no framework) |

### Architecture

- Single Blazor page (`/`) handles all interactions.
- `ChromaService` encapsulates all ChromaDB REST calls: `GetOrCreateCollection`, `Upsert`, `Query`, `IsHealthy`.
- ChromaDB collection name is hardcoded to a single collection for simplicity.
- ChromaDB URL is configurable via `ChromaDb:Url` in `appsettings.json` or environment variable `ChromaDb__Url`.

---

## Out of Scope

- User-defined document input (no custom document upload or editing).
- Multiple collections or collection management UI.
- Authentication or authorization.
- Persistent user sessions or query history.
- Filtering or sorting results by metadata.
- Production hardening (rate limiting, health endpoints, monitoring).
- ChromaDB cluster/distributed mode.
- Any embedding model integration beyond ChromaDB's default embeddings.

---

## Deployment Targets

| Environment | Method |
|---|---|
| Local (Docker) | `docker-compose up -d` → webapp at `http://localhost:8080` |
| Local (dev, no container) | `dotnet run` + ChromaDB container → webapp at `http://localhost:5000` |

---

## Data

The seeded dataset is static and hardcoded in `Components/Pages/Home.razor`. Documents are tagged with:

- **Category**: `.NET`, `AI`, `DevOps`
- **Topic**: `platform`, `blazor`, `vector-database`, `chromadb`, `orm`, `embeddings`, `web-framework`, `search`, `containers`, `linq`

ChromaDB persists data in a named Docker volume (`chroma-data`) between restarts.

---

## Non-Goals

This is a learning sample, not a production system. It intentionally avoids:

- Scalability and performance optimization.
- CI/CD pipelines.
- Comprehensive test coverage.
- Dynamic configuration or admin UI.
