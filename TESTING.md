# TESTING

## Test Projects

The solution contains three xUnit test projects under `tests/`:

| Project | Current coverage |
| --- | --- |
| `SupportOpsAI.UnitTests` | Domain defaults; authentication service registration and successful login; development seed account configuration, creation, updates, and skipping accounts without passwords; structured mock AI output. |
| `SupportOpsAI.IntegrationTests` | Ticket creation and owner access; triage job creation and publishing through a fake queue; triage processing success and failure; human review actions and customer authorization restrictions. |
| `SupportOpsAI.WorkerTests` | Contains a single placeholder assertion. It does not currently verify worker host or RabbitMQ consumer behavior. The triage processor is exercised in `SupportOpsAI.IntegrationTests`. |

## What the Tests Verify

- Tickets default to pending triage with medium priority and a general category; users default to the customer role.
- Registration saves a user and returns an access token. Valid login returns an access token and the user's role.
- Development seed account options bind the configured password keys. Seeding creates or updates admin/agent accounts with working login credentials and skips an account without a configured password.
- The mock AI provider returns valid category/priority values, a confidence score between zero and one, reasoning, suggested steps, and the expected billing category for the tested input.
- Creating a ticket saves the ticket and triage job and records a message in the fake publisher. The creator can retrieve the ticket; an unrelated customer is forbidden.
- Successful triage processing saves a result, marks the job `Completed`, and sets the ticket to `TriageCompleted`. An AI failure at the maximum attempt count marks the job failed and records an error.
- Human review tests cover accepting suggested category/priority, revising suggestions, rejecting a result with notes, and preventing customers from approving, editing, or rejecting triage results.

## Test Boundaries

Persistence-dependent tests use EF Core's **InMemory** provider. The integration project calls services directly; it does not start an HTTP server or connect to PostgreSQL. These tests do not verify PostgreSQL migrations, relational constraints, or provider-specific query behavior.

Automated tests use fake current-user context, a fake queue publisher, and fake/mock AI providers where appropriate. They require neither a running RabbitMQ broker nor an OpenAI API key or real OpenAI API call. The queue assertions verify service publishing requests, not broker delivery. No tests currently exercise the OpenAI provider through mocked HTTP responses.

## Commands

Run the backend tests from the repository root:

```powershell
dotnet test SupportOpsAI.sln
```

Run frontend verification from the repository root:

```powershell
cd frontend
npm install
npm run build
```

The frontend build runs TypeScript checking and creates the Vite production bundle. It does not run UI interaction tests; the frontend currently has no automated test script. See [SETUP.md](SETUP.md) for local prerequisites and the running application workflow.

## Potential Additional Coverage

These are optional ways to extend verification of the completed portfolio application:

- Hosted API endpoint tests, including HTTP responses and authentication middleware.
- PostgreSQL-backed integration tests for migrations and relational behavior.
- RabbitMQ integration tests for publishing, consumption, acknowledgments, and retry delivery.
- Worker host and consumer lifecycle tests beyond the current placeholder.
- OpenAI provider contract tests with mocked HTTP responses.
- Frontend component or browser workflow tests.
