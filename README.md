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
| Client | ChromaDB REST API |
| Deployment | Docker & Docker Compose |

## Prerequisites

- [Docker](https://docs.docker.com/get-docker/) (v20.10+)
- [Docker Compose](https://docs.docker.com/compose/install/) (v2.0+)
- (Optional) [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0) — only needed for local development without Docker

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

## Local Development (Without Docker)

If you want to run the Blazor app locally (you still need ChromaDB running):

```bash
# Start only ChromaDB
docker-compose up -d chromadb

# Run the Blazor app
dotnet run
```

The app will be available at `http://localhost:5000` and will connect to ChromaDB at `http://localhost:8000`.

## Project Structure

```
├── Components/
│   ├── Layout/
│   │   └── MainLayout.razor        # App layout
│   ├── Pages/
│   │   ├── Home.razor               # Main interactive page
│   │   └── Error.razor              # Error page
│   ├── App.razor                    # Root component
│   ├── Routes.razor                 # Routing configuration
│   └── _Imports.razor               # Global usings
├── Services/
│   └── ChromaService.cs             # ChromaDB integration service
├── wwwroot/
│   └── app.css                      # Styles
├── ChromaSampleUI.csproj            # Project file
├── Program.cs                       # App entry point & DI config
├── Dockerfile                       # Multi-stage Docker build
├── docker-compose.yml               # Service orchestration
├── claude.md                        # Project goal
├── skills.md                        # Technology stack
└── README.md                        # This file
```
