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

## Database Migrations

The initial migration is `20260427190622_InitialCreate`.

Apply migrations to the local PostgreSQL database:

```powershell
dotnet dotnet-ef database update --project .\src\SupportOpsAI.Infrastructure\SupportOpsAI.Infrastructure.csproj --startup-project .\src\SupportOpsAI.Api\SupportOpsAI.Api.csproj
```

Create a future migration:

```powershell
dotnet dotnet-ef migrations add MigrationName --project .\src\SupportOpsAI.Infrastructure\SupportOpsAI.Infrastructure.csproj --startup-project .\src\SupportOpsAI.Api\SupportOpsAI.Api.csproj --output-dir Data\Migrations
```

## Running Individual Projects

```powershell
dotnet run --project .\src\SupportOpsAI.Api\SupportOpsAI.Api.csproj
dotnet run --project .\src\SupportOpsAI.Worker\SupportOpsAI.Worker.csproj
```

## Notes

- PostgreSQL and RabbitMQ are defined in `docker-compose.yml`
- The API reads the PostgreSQL connection string from `ConnectionStrings:DefaultConnection`
- JWT values can be overridden with environment variables such as `Jwt__SigningKey`
