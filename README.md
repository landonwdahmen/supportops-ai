# SupportOps AI

## Overview

SupportOps AI is a backend-focused portfolio project for AI-assisted support ticket triage. Phase 3 adds asynchronous triage processing: ticket creation queues a RabbitMQ message, the worker consumes it, the configured AI provider produces a recommendation, and support staff can review that recommendation.

## Tech Stack

- Backend: ASP.NET Core Web API with C#
- Application architecture: Clean Architecture-inspired project layering
- Database: PostgreSQL with Entity Framework Core
- Messaging: RabbitMQ
- Background processing: .NET Worker Service
- AI triage: mock provider by default, OpenAI provider by configuration
- Frontend: lightweight React, TypeScript, and Vite demo UI
- Testing: xUnit
- Containerization: Docker Compose

## Repository Structure

```text
src/
  SupportOpsAI.Api
  SupportOpsAI.Application
  SupportOpsAI.Domain
  SupportOpsAI.Infrastructure
  SupportOpsAI.Worker
tests/
  SupportOpsAI.UnitTests
  SupportOpsAI.IntegrationTests
  SupportOpsAI.WorkerTests
frontend/
docs/
scripts/
requests/
postman/
.github/workflows/
```

## Current Status

Phase 4 includes domain models, EF Core persistence, JWT authentication, ticket APIs, RabbitMQ queue publishing/consuming, mock/OpenAI AI triage providers, worker processing, human review endpoints, repeatable `.http` requests, and a lightweight React demo UI.

Deployment automation, advanced knowledge base search, pgvector, real email notifications, SignalR, and a full admin dashboard UI are intentionally out of scope.

## Quick Start

1. Copy `.env.example` to `.env`.
2. Start local infrastructure with `docker compose up -d`.
3. Restore with `dotnet restore SupportOpsAI.sln --configfile NuGet.Config`.
4. Build with `dotnet build SupportOpsAI.sln`.
5. Apply migrations with `dotnet dotnet-ef database update --project src/SupportOpsAI.Infrastructure/SupportOpsAI.Infrastructure.csproj --startup-project src/SupportOpsAI.Api/SupportOpsAI.Api.csproj`.
6. Run tests with `dotnet test SupportOpsAI.sln`.
7. Run the API with `dotnet run --project .\src\SupportOpsAI.Api\SupportOpsAI.Api.csproj`.
8. Run the worker in a second terminal with `dotnet run --project .\src\SupportOpsAI.Worker\SupportOpsAI.Worker.csproj`.
9. Install and run the frontend from `frontend/` with `npm install` and `npm run dev`.

See `API.md`, `SETUP.md`, and `TESTING.md` for more detail.
