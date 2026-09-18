# AGENTS

## Purpose

SupportOps AI is a completed solo portfolio project demonstrating an AI-assisted support ticket workflow with human review. This guide helps contributors and coding agents maintain the implemented application and keep future changes focused.

## Current Expectations

- Preserve clean architecture boundaries and the local portfolio/demo scope.
- Keep changes and commits focused and reviewable.
- Document proposed scope changes before adding features or production integrations. The potential enhancements in `PLAN.md` are optional, not unfinished requirements.
- Do not commit secrets, real credentials, API keys, or local `.env` files.

## Layering Rules

- `SupportOpsAI.Domain` contains entities and enums and must remain independent of outer layers and external infrastructure.
- `SupportOpsAI.Application` may depend on Domain. It owns contracts, DTOs, validation, and service interfaces without depending on concrete persistence, messaging, or AI providers.
- `SupportOpsAI.Infrastructure` may depend on Application and Domain. It implements persistence, messaging, AI providers, security, and workflow services behind application contracts.
- `SupportOpsAI.Api` may depend on Application and Infrastructure. It exposes HTTP endpoints, authentication, and request-time user context, delegating workflow behavior through application interfaces.
- `SupportOpsAI.Worker` may depend on Application and Infrastructure. It hosts background triage consumption and processing using the Infrastructure consumer and application contracts.
- `frontend/` is the React/TypeScript demo client. It uses the API for authentication and workflow actions.

## Workflow and Provider Rules

- Preserve the human-in-the-loop review model: AI results are recommendations; agents or admins approve or edit them before their category and priority are applied to tickets.
- Do not bypass review authorization or remove the audit records that make workflow decisions traceable.
- Keep AI providers behind the existing `IAiTriageService` abstraction and provider configuration.
- Prefer deterministic mock providers and fakes for automated tests. Routine verification must not require a real OpenAI API call.

## Verification and Documentation

- Add or update tests when changing behavior. Describe test boundaries accurately; service tests with EF Core InMemory and fakes do not prove PostgreSQL or RabbitMQ integration.
- Run the relevant backend tests and frontend build described in `TESTING.md` before handing off changes.
- Keep `SETUP.md`, `API.md`, `TESTING.md`, `ARCHITECTURE.md`, and `README.md` aligned with code changes, and update `PLAN.md` when project scope changes.
- Keep setup commands, configuration names, API descriptions, and test coverage claims grounded in the repository. Do not imply deployment, production use, or coverage that has not been verified.
