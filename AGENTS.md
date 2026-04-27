# AGENTS

## Purpose

This file gives future contributors and coding agents a stable operating baseline while the project is still in its foundation stage.

## Current Expectations

- Preserve clean architecture boundaries
- Keep business logic out of Phase 1 scaffolding
- Prefer small, reviewable commits
- Add documentation alongside structural changes
- Avoid introducing production integrations before the related phase is approved

## Layering Rules

- `SupportOpsAI.Domain` should remain independent of outer layers
- `SupportOpsAI.Application` may depend on `SupportOpsAI.Domain`
- `SupportOpsAI.Infrastructure` may depend on `SupportOpsAI.Application` and `SupportOpsAI.Domain`
- `SupportOpsAI.Api` may depend on `SupportOpsAI.Application` and `SupportOpsAI.Infrastructure`
- `SupportOpsAI.Worker` may depend on `SupportOpsAI.Application` and `SupportOpsAI.Infrastructure`

## Near-Term Guidance

- Add new business features only after documenting the target phase
- Keep local development instructions current in `SETUP.md`
- Keep API notes current in `API.md`
- Keep testing expectations current in `TESTING.md`
