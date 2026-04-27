# TESTING

## Test Projects

- `SupportOpsAI.UnitTests`
- `SupportOpsAI.IntegrationTests`
- `SupportOpsAI.WorkerTests`

## Current Scope

Phase 2 includes initial tests for domain defaults, auth registration/login, ticket creation/retrieval, and unauthorized ticket access.

## Commands

```powershell
dotnet test SupportOpsAI.sln
```

## Future Coverage

- Worker behavior tests
- API endpoint tests
- PostgreSQL-backed integration tests
- RabbitMQ integration tests when messaging is introduced
