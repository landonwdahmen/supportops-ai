# PLAN

## Project Status

SupportOps AI is a completed solo portfolio project. The original project was implemented incrementally in phases, from solution scaffolding through the customer-to-agent triage workflow. The implementation phases are complete for the current portfolio version, which is scoped for local demonstration.

## Completed Implementation Phases

### Solution Foundation and Contracts — Completed

- .NET solution with Domain, Application, Infrastructure, API, Worker, and test projects
- Clean architecture boundaries separating domain models, application contracts, and external implementations
- Domain entities and enums for users, tickets, comments, triage jobs, recommendations, and audit records
- Application DTOs, validation, service interfaces, and triage messaging contracts
- Docker Compose definitions for local PostgreSQL and RabbitMQ

### Persistence and Authentication — Completed

- PostgreSQL persistence through EF Core, with entity configurations and checked-in migrations
- Registration, login, password hashing, and JWT bearer authentication
- Current-user access and role-aware workflow authorization
- Configurable development seed accounts for local use

### Ticket API and Background Processing — Completed

- Ticket creation, listing, detail retrieval, and comments through authenticated API endpoints
- Persistent triage jobs created alongside tickets and published to RabbitMQ
- RabbitMQ publishing and consumption implementations
- A separate .NET worker host that consumes queued jobs and runs triage processing
- Job status tracking, processing attempts, and failure/retry handling

### AI Triage and Human Review — Completed

- AI recommendations containing category, priority, confidence, reasoning, and suggested steps
- Deterministic mock AI provider used by default and an OpenAI provider selectable through configuration
- Both providers behind the existing `IAiTriageService` abstraction
- Saved recommendations awaiting human review before their category and priority are applied to tickets
- Agent/admin actions to approve, edit and apply, reject, or retry triage
- Audit logging for authentication, ticket, triage processing, and review events

### Frontend Demo — Completed

- React, TypeScript, and Vite client with login, registration, and protected routes
- Local demo login helpers, ticket lists, ticket creation, and ticket detail views
- AI recommendation display and agent review controls

### Verification and Portfolio Documentation — Completed

- xUnit coverage for domain defaults, authentication, development seed accounts, mock AI output, ticket services, triage processing, and human review
- Service tests using EF Core InMemory, fake current-user context, fake messaging, and fake/mock AI; automated tests do not require a real OpenAI call
- GitHub Actions workflow for backend restore, build, and tests
- Setup, API, architecture, and testing documentation, plus reusable HTTP requests in `requests/`
- Captured workflow screenshots and a [local demo walkthrough](docs/demo-walkthrough.md)

See [TESTING.md](TESTING.md) for the exact coverage and its limits, including the placeholder worker test project. See [README.md](README.md) for the application overview and screenshots.

## Potential Future Enhancements

These are optional ideas for future work, not incomplete requirements for the finished portfolio version:

- pgvector / semantic knowledge-base search
- Support-article recommendations
- SLA tracking
- Email notifications
- SignalR / live updates
- Admin analytics
- CSV export
- Expanded user profiles
- Rate limiting
- Prometheus/Grafana-style observability
- Production deployment automation
