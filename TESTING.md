# TESTING

## Test Projects

- `SupportOpsAI.UnitTests`
- `SupportOpsAI.IntegrationTests`
- `SupportOpsAI.WorkerTests`

## Current Scope

Phase 3 includes tests for domain defaults, auth registration/login, ticket creation/retrieval, triage job creation/publishing, mock AI output, worker-style processing, failed AI triage handling, and human review authorization/actions.

## Commands

```powershell
dotnet test SupportOpsAI.sln
```

Frontend verification:

```powershell
cd frontend
npm install
npm run build
```

Automated tests use fakes and the mock AI provider. They do not require a real OpenAI API call.

## Future Coverage

- API endpoint tests
- PostgreSQL-backed integration tests
- RabbitMQ integration tests
- OpenAI contract tests with mocked HTTP responses
