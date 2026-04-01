# ChromaDB + .NET Blazor Interactive Sample

An interactive learning sample that demonstrates how to integrate **ChromaDB** (a vector database) with a **.NET 8 Blazor Web App**, fully orchestrated with **Docker Compose**.

## What Does This App Do?

This application provides a hands-on way to explore ChromaDB's capabilities:

- **Seed sample documents** about .NET and AI/Vector Databases into ChromaDB
- **Perform semantic searches** using natural language queries
- **View results** with similarity distances and metadata

ChromaDB handles embedding generation and similarity matching automatically — you just provide text and search by meaning.

## Tech Stack

| Component | Technology |
|-----------|-----------|
| Frontend | .NET 8 Blazor Web App (Interactive Server) |
| Language | C# |
| Vector DB | ChromaDB |
| Client | ChromaDB REST API (`ChromaDB.Client` v1.0.1) |
| Deployment | Docker & Docker Compose |

---

## Embedding Functions

This project does **not** call any external embedding API. Embeddings are generated **server-side by ChromaDB itself** using its built-in default embedding model:

- **Model:** `all-MiniLM-L6-v2` (Sentence Transformers)
- **Provider:** Hugging Face / Sentence Transformers — runs entirely inside the `chromadb` container
- **How it works:** When you call `UpsertDocumentsAsync`, the application sends raw text to ChromaDB via `POST /api/v1/collections/{collectionId}/upsert`. ChromaDB encodes each document into a high-dimensional vector automatically. The same process applies to search queries — `POST /api/v1/collections/{collectionId}/query` accepts raw query text and ChromaDB converts it to a vector before performing cosine similarity matching.

The relevant service methods in `Services/ChromaService.cs` are:

| Method | ChromaDB Endpoint | Purpose |
|--------|------------------|---------|
| `GetOrCreateCollectionAsync` | `POST /api/v1/collections` | Creates or retrieves a named collection |
| `UpsertDocumentsAsync` | `POST /api/v1/collections/{id}/upsert` | Stores documents and triggers embedding generation |
| `QueryAsync` | `POST /api/v1/collections/{id}/query` | Converts query to vector and returns top-N similar documents |
| `IsHealthyAsync` | `GET /api/v1/heartbeat` | Checks if ChromaDB is reachable |

---

## LLM Usage

**This project does not use a Large Language Model (LLM).** There are no calls to OpenAI, Anthropic, or any other LLM API.

The application is a **semantic search / RAG retrieval layer only** — it stores and retrieves documents based on vector similarity. To add generative answers on top of search results (full RAG), you would integrate an LLM as a second step (see the NextJS section below for context on extending this).

If you want to add LLM-generated answers, the typical options are:

| LLM Provider | How to add |
|---|---|
| OpenAI (GPT-4o) | Add `OpenAI` NuGet package, pass retrieved documents as context |
| Anthropic (Claude) | Call Claude API via `HttpClient` with retrieved documents in the prompt |
| Ollama (local, no API key) | Run `ollama` as a Docker service and call its REST API |

---

## ChromaDB Uses in This Project

ChromaDB serves as the **vector storage and retrieval engine** for this project. Specifically it is used to:

1. **Create and manage collections** — a collection (`sample_documents`) acts as a namespace for a group of related documents with their embeddings.
2. **Store documents with metadata** — each document is stored with a unique ID, raw text, and metadata tags (e.g., `category: AI`, `topic: embeddings`). ChromaDB automatically generates and stores the embedding alongside.
3. **Perform semantic similarity search** — given a natural language query, ChromaDB converts it to a vector and returns the closest matching documents ranked by cosine distance. Lower distance = more semantically similar.
4. **Persist data across restarts** — data is stored on a Docker named volume (`chroma-data`) so the knowledge base survives container restarts.

In a production RAG pipeline, ChromaDB sits between the user's query and an LLM answer: retrieve the most relevant documents from ChromaDB first, then pass them as context to the LLM to generate a grounded answer.

---

## Prerequisites

- [Docker](https://docs.docker.com/get-docker/) (v20.10+)
- [Docker Compose](https://docs.docker.com/compose/install/) (v2.0+)
- (Optional) [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0) — only needed for local development without Docker

---

## Getting Started

### 1. Clone the Repository

```bash
git clone https://github.com/lorenzo9616/chromaDbSample1.git
cd chromaDbSample1
```

### 2. Run with Docker Compose

```bash
docker-compose up -d
```

This starts two services:
- **chromadb** — ChromaDB vector database on port `8000`
- **webapp** — Blazor web application on port `8080`

### 3. Open the Application

Navigate to [http://localhost:8080](http://localhost:8080) in your browser.

### 4. Using the Application

1. **Seed Data**: Click the **"Seed Data"** button to upsert 10 predefined documents (facts about .NET, Blazor, vector databases, ChromaDB, Docker, and more) into the ChromaDB collection. A success message confirms the operation.

2. **Search**: Type a natural language query into the search box (e.g., *"What is a vector database?"* or *"How does Blazor work?"*) and click **"Search"** or press Enter.

3. **View Results**: The app displays the most similar documents along with:
   - The matching document text
   - A **distance score** (lower = more similar)
   - **Metadata tags** (category and topic)

### 5. Stop the Application

```bash
docker-compose down
```

To also remove the persisted ChromaDB data:

```bash
docker-compose down -v
```

---

## Running Without Docker (If Docker Is Not Working)

If Docker is unavailable or not functioning, you can run both services manually.

### Option A — Run ChromaDB with pip + run Blazor with dotnet

**Step 1: Install and start ChromaDB locally**

```bash
pip install chromadb
chroma run --path ./chroma-data --port 8000
```

ChromaDB will be available at `http://localhost:8000`.

**Step 2: Run the Blazor app**

```bash
dotnet run
```

The Blazor app will be available at `http://localhost:5000` and will connect to ChromaDB at `http://localhost:8000` (configured in `appsettings.json`).

### Option B — Run only ChromaDB in Docker, Blazor locally

If Docker works for ChromaDB but not for the full compose stack:

```bash
docker run -p 8000:8000 chromadb/chroma:latest
dotnet run
```

### Option C — Run ChromaDB with uvicorn directly (advanced)

```bash
pip install chromadb uvicorn
uvicorn chromadb.app:app --host 0.0.0.0 --port 8000
dotnet run
```

> **Note:** Python 3.8+ is required for ChromaDB. Verify with `python --version`.

---

## Local Development (Blazor Only, ChromaDB in Docker)

```bash
# Start only ChromaDB
docker-compose up -d chromadb

# Run the Blazor app locally
dotnet run
```

The app will be available at `http://localhost:5000` and will connect to ChromaDB at `http://localhost:8000`.

---

## Adding New Documents to the Seeder (Expanding the Knowledge Base)

The seed data lives in the `SeedData()` method inside `Components/Pages/Home.razor`. To add new documents:

**Step 1:** Open `Components/Pages/Home.razor` and locate the three lists in `SeedData()`:

```csharp
var ids = new List<string> { "doc-1", "doc-2", ... };
var documents = new List<string> { "...", "..." };
var metadatas = new List<Dictionary<string, string>> { ... };
```

**Step 2:** Add a new entry to each list. The index positions must stay in sync across all three lists.

```csharp
// Add to ids
"doc-11",

// Add to documents
"Minimal APIs in ASP.NET Core allow you to build HTTP APIs with minimal ceremony using top-level statements and lambda handlers.",

// Add to metadatas
new() { { "category", ".NET" }, { "topic", "minimal-api" } },
```

**Step 3:** Save the file and restart the app. Click **"Seed Data"** again — upsert is idempotent, so existing documents are updated and new ones are added without duplication.

### Guidelines for good RAG documents

- Keep each document focused on a **single concept** — shorter, focused chunks retrieve better than long mixed paragraphs.
- Use consistent `category` and `topic` metadata values so you can filter results later.
- Avoid duplicate or near-identical content — ChromaDB will store it but it will inflate result noise.
- Aim for documents between **1–5 sentences** each for best embedding quality with `all-MiniLM-L6-v2`.

---

## NextJS Integration

### Can this project integrate with NextJS?

Yes. ChromaDB exposes a plain HTTP REST API, so any frontend or backend that can make HTTP requests can talk to it. A NextJS app can call ChromaDB directly from API Routes or Server Actions:

```ts
// pages/api/search.ts (NextJS API Route)
export default async function handler(req, res) {
  const { query } = req.body;

  // 1. Get or create collection
  const colRes = await fetch("http://localhost:8000/api/v1/collections", {
    method: "POST",
    headers: { "Content-Type": "application/json" },
    body: JSON.stringify({ name: "sample_documents", get_or_create: true }),
  });
  const { id } = await colRes.json();

  // 2. Query ChromaDB
  const qRes = await fetch(`http://localhost:8000/api/v1/collections/${id}/query`, {
    method: "POST",
    headers: { "Content-Type": "application/json" },
    body: JSON.stringify({ query_texts: [query], n_results: 5, include: ["documents", "metadatas", "distances"] }),
  });
  const results = await qRes.json();

  res.json(results);
}
```

### Is NextJS integration efficient long-term?

**It depends on the goal:**

| Scenario | Recommendation |
|---|---|
| **Greenfield web app** with React UI | NextJS is a strong choice — React ecosystem, server components, and App Router scale well |
| **Already invested in .NET** | Keep Blazor — adding NextJS introduces a second language and build pipeline for no gain |
| **Separate frontend + .NET backend** | NextJS frontend + ASP.NET Core Web API backend is a common and scalable pattern |
| **Full RAG with streaming LLM responses** | NextJS with Server Actions or Route Handlers handles streaming well via the Vercel AI SDK |
| **Team expertise is primarily JS/TS** | NextJS will be faster to iterate on than Blazor |

**Verdict:** NextJS is a fully viable and long-term efficient option for consuming a ChromaDB-backed API, especially if you want to add streaming LLM responses (e.g., using Vercel AI SDK + Claude or OpenAI). However, it does not replace the `.NET` backend — ChromaDB REST calls should still originate from a server-side layer (NextJS API Routes or a separate ASP.NET Core API), not directly from the browser, to avoid CORS issues and to keep ChromaDB off the public network.

---

## Project Structure

```
├── Components/
│   ├── Layout/
│   │   └── MainLayout.razor        # App layout
│   ├── Pages/
│   │   ├── Home.razor               # Main interactive page + Seeder logic
│   │   └── Error.razor              # Error page
│   ├── App.razor                    # Root component
│   ├── Routes.razor                 # Routing configuration
│   └── _Imports.razor               # Global usings
├── Services/
│   └── ChromaService.cs             # ChromaDB REST API client
├── wwwroot/
│   └── app.css                      # Styles
├── ChromaSampleUI.csproj            # Project file
├── Program.cs                       # App entry point & DI config
├── Dockerfile                       # Multi-stage Docker build
├── docker-compose.yml               # Service orchestration
└── README.md                        # This file
```
