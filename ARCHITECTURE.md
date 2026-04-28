# ARCHITECTURE

## System Intent

SupportOps AI is a backend-centered system that ingests support tickets, enriches and classifies them with AI-assisted workflows, and routes follow-up work through API and background processing components.

## Solution Layers

### Domain

Core entities and enums live in `SupportOpsAI.Domain`. Domain entities remain free of EF Core, RabbitMQ, OpenAI, and API-specific dependencies.

### Application

DTOs, validation, message contracts, and service abstractions live in `SupportOpsAI.Application`.

### Infrastructure

EF Core persistence, RabbitMQ publishing/consuming, password hashing, audit logging, and AI provider implementations live in `SupportOpsAI.Infrastructure`.

### API

HTTP controllers, JWT wiring, and current-user request access live in `SupportOpsAI.Api`.

### Worker

The worker host runs the RabbitMQ triage consumer registered from Infrastructure.

## External Dependencies

- PostgreSQL for relational persistence
- RabbitMQ for asynchronous triage messages
- OpenAI API for optional real AI triage

## Triage Workflow

1. A customer creates a ticket through the API.
2. The ticket is saved as `PendingTriage`.
3. A `TriageJob` is saved as `Queued`.
4. The API publishes a versioned `TriageJobMessage` to RabbitMQ.
5. The worker consumes the message and marks the job `Processing`.
6. The worker calls the configured AI triage provider.
7. The worker saves a `TicketTriageResult`, marks the job `Completed`, and updates the ticket to `TriageCompleted`.
8. An agent/admin can approve, edit, reject, or retry the recommendation.

## Phase 3 Note

Phase 3 includes RabbitMQ publishing/consuming, mock/OpenAI triage providers, worker processing, and human review endpoints. Frontend, deployment, advanced search, pgvector, notifications, SignalR, and admin dashboard UI remain deferred.
