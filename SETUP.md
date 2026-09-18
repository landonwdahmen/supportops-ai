# SETUP

## Prerequisites

- .NET 8 SDK
- Node.js 20 or later
- Docker Desktop or another Docker engine with Compose support

## Local Environment

1. Copy `.env.example` to `.env`.
2. Review environment values and adjust credentials if needed.
3. Start infrastructure services with `docker compose up -d`.

Docker Compose reads the root `.env` file. The API, worker, and EF tooling do not load it automatically. Set backend overrides as process environment variables in the PowerShell terminal that launches each command, using the existing names in `.env.example`. For example, if you change PostgreSQL credentials or ports, set `ConnectionStrings__DefaultConnection` for migrations, the API, and the worker; set matching `RabbitMQ__*` overrides for the API and worker. The checked-in Development settings match the default local infrastructure values.

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

Restore the repository's local EF tool, then apply migrations:

```powershell
dotnet tool restore --configfile NuGet.Config
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

Run the frontend in a third terminal:

```powershell
cd frontend
npm install
npm run dev
```

The Vite app runs at `http://localhost:5173` by default.

## Frontend Configuration

Copy `frontend/.env.example` to `frontend/.env` if you need to override defaults.

```text
VITE_API_BASE_URL=/api
VITE_API_PROXY_TARGET=http://localhost:5116
```

The frontend calls `VITE_API_BASE_URL`. During local development, Vite proxies `/api` to the API at `VITE_API_PROXY_TARGET`, so the backend does not need extra local CORS setup.

## Demo API Requests

Repeatable `.http` request files live in `requests/` for VS Code REST Client or the JetBrains Rider HTTP Client. Start Docker services, apply migrations, run the API and worker, then open `requests/full-workflow.http` and run the requests from top to bottom. The smaller `auth.http`, `tickets.http`, and `triage.http` files are useful when testing one part of the backend flow.

## Development Seed Accounts

The API can seed local-only review accounts on startup in the Development environment.

1. Set these process environment variables in the API terminal before starting the API:

```powershell
$env:DevelopmentSeedAccounts__AdminPassword = '<your-local-admin-password>'
$env:DevelopmentSeedAccounts__AgentPassword = '<your-local-agent-password>'
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

For the frontend demo, the login page includes quick-fill buttons for:

- customer demo: `customer1@example.com` / `Password123!`
- agent demo: `agent@supportops.local` / `AgentPassword123!`

Register the customer first if the database does not already contain that account. The agent account is seeded only when the API starts in Development and `DevelopmentSeedAccounts__AgentPassword` is configured. If your configured agent password differs from the quick-fill value, replace the autofilled password before logging in.

## RabbitMQ

The API publishes triage messages to `RabbitMQ:TriageQueueName` when tickets are created. The worker consumes that queue and processes messages with a capped retry strategy.

Default queue:

```text
supportops.triage.jobs
```

RabbitMQ Management UI is available at `http://localhost:15672` with local credentials from `.env`.

## AI Provider

Local development uses the mock provider by default, and automated tests use fake/mock providers. To explicitly select Mock in the worker terminal:

```powershell
$env:AiTriage__Provider = 'Mock'
```

To use OpenAI locally, set these process environment variables in the worker terminal before starting the worker:

```powershell
$env:AiTriage__Provider = 'OpenAI'
$env:OpenAI__ApiKey = '<your-api-key>'
$env:OpenAI__Model = 'gpt-4o-mini'
```

Do not commit real API keys.

## Notes

- PostgreSQL and RabbitMQ are defined in `docker-compose.yml`.
- The API and worker read PostgreSQL from `ConnectionStrings:DefaultConnection`.
- JWT values can be overridden with environment variables such as `Jwt__SigningKey`.
- RabbitMQ and OpenAI settings can be overridden with `RabbitMQ__*`, `AiTriage__Provider`, and `OpenAI__*`.
