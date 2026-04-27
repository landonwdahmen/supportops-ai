# SupportOps AI

## Overview

SupportOps AI is a backend-focused portfolio project for exploring AI-assisted support ticket triage. This repository is currently in Phase 1 and intentionally contains only solution scaffolding, project structure, documentation placeholders, and local infrastructure setup.

## Tech Stack

- Backend: ASP.NET Core Web API with C#
- Application architecture: Clean Architecture-inspired project layering
- Database: PostgreSQL with Entity Framework Core planned for a later phase
- Messaging: RabbitMQ
- Background processing: .NET Worker Service
- Frontend: React, TypeScript, and Vite planned for a later phase
- Testing: xUnit
- Containerization: Docker Compose
- AI provider: OpenAI API planned for a later phase

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
docs/
scripts/
requests/
postman/
.github/workflows/
```

## Current Status

Phase 1 establishes the repository foundation only. There are no controllers, domain entities, data models, migrations, authentication flows, or ticket-triage business rules yet.

## Quick Start

1. Copy `.env.example` to `.env`.
2. Start local infrastructure with `docker compose up -d`.
3. Restore the solution with `dotnet restore SupportOpsAI.sln`.
4. Build the solution with `dotnet build SupportOpsAI.sln`.
5. Run tests with `dotnet test SupportOpsAI.sln`.

See `SETUP.md` and `TESTING.md` for more detail.
