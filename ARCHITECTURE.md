# ARCHITECTURE

## System Intent

SupportOps AI is a clean architecture-inspired support workflow application. It accepts customer tickets, persists them in PostgreSQL, queues AI triage work through RabbitMQ, processes that work in a background service, and exposes human review actions through the API and frontend demo.

The system is intentionally scoped for local portfolio demonstration. It uses real infrastructure patterns while keeping production deployment, advanced search, notifications, and expanded admin tooling out of scope.

## API Layer

Project: `src/SupportOpsAI.Api`

The API layer is the HTTP boundary for the system. It owns:

- Controller endpoints for auth, tickets, triage review, and health checks
- JWT bearer authentication wiring
- Request-time current-user access through `CurrentUserService`
- API host configuration and dependency registration
- Development-time Swagger launch support

The API delegates workflow behavior to application contracts implemented by Infrastructure. It should not contain domain business rules or persistence details.

## Application Layer

Project: `src/SupportOpsAI.Application`

The Application layer defines the contracts and data shapes used by the API, worker, and infrastructure services. It contains:

- DTOs for auth, tickets, and triage
- Service interfaces such as `ITicketService`, `ITriageReviewService`, `ITriageJobProcessor`, and `IAiTriageService`
- Validation classes for incoming requests
- Messaging contracts such as `TriageJobMessage`
- Application exceptions used by outer layers

This layer may depend on Domain, but it avoids direct dependencies on EF Core, RabbitMQ, ASP.NET Core controllers, or external AI SDKs.

## Domain Layer

Project: `src/SupportOpsAI.Domain`

The Domain layer contains the core model:

- `User`
- `Ticket`
- `TicketComment`
- `TriageJob`
- `TicketTriageResult`
- `AuditLog`
- Supporting enums for roles, statuses, priority, category, and audit event types

Domain remains independent of outer layers. It should not reference API, Infrastructure, database, queue, or AI provider implementation concerns.

## Infrastructure Layer

Project: `src/SupportOpsAI.Infrastructure`

Infrastructure implements the application contracts and integrates with external systems. It owns:

- EF Core `SupportOpsDbContext`, entity configurations, migrations, and design-time factory
- PostgreSQL-backed service implementations for tickets, auth, audit logging, triage review, and triage job processing
- RabbitMQ publisher and consumer implementations
- Password hashing
- Development seed account setup
- Mock AI and OpenAI triage providers
- Dependency injection registration for infrastructure services

Infrastructure may depend on Application and Domain. It is the correct layer for concrete persistence, messaging, and provider code.

## Worker Service

Project: `src/SupportOpsAI.Worker`

The worker is a separate .NET host that runs background triage processing. It registers Infrastructure services, connects to RabbitMQ, consumes triage job messages, and calls `ITriageJobProcessor`.

The worker uses its own current-user service implementation because background processing does not run inside an authenticated HTTP request. It is responsible for keeping queued triage work moving without blocking customer-facing API requests.

## RabbitMQ Flow

1. A customer creates a ticket through the API.
2. `TicketService` saves the ticket with a pending triage state.
3. `TicketService` creates a `TriageJob` record with queued status.
4. The API publishes a `TriageJobMessage` to the configured RabbitMQ triage queue.
5. The worker consumes the message from RabbitMQ.
6. The worker calls the triage job processor.
7. The job processor marks the job processing, completed, or failed based on the outcome.

Default local queue:

```text
supportops.triage.jobs
```

RabbitMQ makes ticket creation responsive and keeps AI processing isolated from the HTTP request lifecycle.

## AI Triage Flow

1. The worker receives a triage job message.
2. The job processor loads the ticket and job from PostgreSQL.
3. The configured `IAiTriageService` receives a structured triage request.
4. The mock provider returns deterministic local recommendations by default.
5. The optional OpenAI provider can be enabled through configuration.
6. The recommendation is saved as a `TicketTriageResult`.
7. The ticket is moved into a triage-completed state for human review.
8. Audit records capture important workflow events.

The AI result is advisory. It does not silently close, reroute, or resolve a ticket without review.

## Human Review Flow

Agents and admins can review AI triage results through the API and frontend demo. The review workflow supports:

- Approving the recommendation
- Editing the recommendation before approval
- Rejecting the recommendation
- Retrying failed or rejected triage work

Human review keeps the AI-assisted workflow accountable. The system presents a recommendation, but the support agent makes the final operational decision.

## Database Overview

PostgreSQL stores the system of record:

- Users and password hashes
- Tickets and ticket comments
- Triage jobs and their processing status
- AI triage results
- Audit log entries

Entity Framework Core manages schema migrations in:

```text
src/SupportOpsAI.Infrastructure/Data/Migrations
```

Current local migrations include the initial schema and the triage workflow schema. The database schema should change only when a documented project phase requires it.

## Frontend Demo

Project: `frontend/`

The frontend is a React, TypeScript, and Vite demo app. It provides enough UI to demonstrate the local workflow:

- Login and registration
- Local/demo-only credential autofill
- Ticket list and ticket detail views
- Ticket creation
- Triage result display
- Agent review actions

The frontend is intentionally lightweight and should remain demo-focused until a later phase expands product scope.
