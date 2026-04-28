# SETUP

## Prerequisites

- .NET 8 SDK
- Docker Desktop or another Docker engine with Compose support

## Local Environment

1. Copy `.env.example` to `.env`.
2. Review environment values and adjust credentials if needed.
3. Start infrastructure services with `docker compose up -d`.

## Solution Commands

```powershell
dotnet restore SupportOpsAI.sln --configfile NuGet.Config
dotnet build SupportOpsAI.sln
dotnet test SupportOpsAI.sln
```

## Database Migrations

Current migrations:

- `20260427190622_InitialCreate`
- `20260428025556_AddPhase3TriageWorkflow`

Apply migrations:

```powershell
dotnet dotnet-ef database update --project .\src\SupportOpsAI.Infrastructure\SupportOpsAI.Infrastructure.csproj --startup-project .\src\SupportOpsAI.Api\SupportOpsAI.Api.csproj
```

Create a future migration:

```powershell
dotnet dotnet-ef migrations add MigrationName --project .\src\SupportOpsAI.Infrastructure\SupportOpsAI.Infrastructure.csproj --startup-project .\src\SupportOpsAI.Api\SupportOpsAI.Api.csproj --output-dir Data\Migrations
```

## Running Locally

Run the API and worker in separate terminals:

```powershell
dotnet run --project .\src\SupportOpsAI.Api\SupportOpsAI.Api.csproj
dotnet run --project .\src\SupportOpsAI.Worker\SupportOpsAI.Worker.csproj
```

## Demo API Requests

Repeatable `.http` request files live in `requests/` for VS Code REST Client or the JetBrains Rider HTTP Client. Start Docker services, apply migrations, run the API and worker, then open `requests/full-workflow.http` and run the requests from top to bottom. The smaller `auth.http`, `tickets.http`, and `triage.http` files are useful when testing one part of the backend flow.

## Development Seed Accounts

The API can seed local-only review accounts on startup in the Development environment.

1. Set these values in `.env` before starting the API:

```text
DevelopmentSeedAccounts__AdminPassword=<your-local-admin-password>
DevelopmentSeedAccounts__AgentPassword=<your-local-agent-password>
```

2. Start the API with the Development environment configuration.
3. Log in through `POST /api/auth/login` with one of these accounts:

```json
{
  "email": "admin@supportops.local",
  "password": "<your-local-admin-password>"
}
```

```json
{
  "email": "agent@supportops.local",
  "password": "<your-local-agent-password>"
}
```

Seeded development accounts:

- `admin@supportops.local` with role `Admin`
- `agent@supportops.local` with role `Agent`

If either password is left blank, that specific development account is skipped.

## RabbitMQ

The API publishes triage messages to `RabbitMQ:TriageQueueName` when tickets are created. The worker consumes that queue and processes messages with a capped retry strategy.

Default queue:

```text
supportops.triage.jobs
```

RabbitMQ Management UI is available at `http://localhost:15672` with local credentials from `.env`.

## AI Provider

Local development and automated tests use the mock provider by default:

```text
AiTriage__Provider=Mock
```

To use OpenAI locally:

```text
AiTriage__Provider=OpenAI
OpenAI__ApiKey=<your-api-key>
OpenAI__Model=gpt-4o-mini
```

Do not commit real API keys.

## Notes

- PostgreSQL and RabbitMQ are defined in `docker-compose.yml`.
- The API and worker read PostgreSQL from `ConnectionStrings:DefaultConnection`.
- JWT values can be overridden with environment variables such as `Jwt__SigningKey`.
- RabbitMQ and OpenAI settings can be overridden with `RabbitMQ__*`, `AiTriage__Provider`, and `OpenAI__*`.
