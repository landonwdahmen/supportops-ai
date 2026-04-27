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
dotnet restore SupportOpsAI.sln
dotnet build SupportOpsAI.sln
dotnet test SupportOpsAI.sln
```

## Running Individual Projects

```powershell
dotnet run --project .\src\SupportOpsAI.Api\SupportOpsAI.Api.csproj
dotnet run --project .\src\SupportOpsAI.Worker\SupportOpsAI.Worker.csproj
```

## Notes

- PostgreSQL and RabbitMQ are defined in `docker-compose.yml`
- Application configuration and real connection wiring will be added in a later phase
