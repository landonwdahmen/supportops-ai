# Demo Walkthrough

This walkthrough shows the local Phase 4 demo flow from infrastructure startup through customer ticket creation and agent triage review.

## Start Docker

From the repository root:

```powershell
docker compose up -d
```

This starts PostgreSQL and RabbitMQ. RabbitMQ Management UI is available at `http://localhost:15672` with the local credentials configured in `.env`.

## Run the API

In a terminal at the repository root:

```powershell
dotnet run --project .\src\SupportOpsAI.Api\SupportOpsAI.Api.csproj
```

The API runs at `http://localhost:5116` by default.

If the database has not been initialized yet, apply migrations first:

```powershell
dotnet dotnet-ef database update --project .\src\SupportOpsAI.Infrastructure\SupportOpsAI.Infrastructure.csproj --startup-project .\src\SupportOpsAI.Api\SupportOpsAI.Api.csproj
```

## Run the Worker

In a second terminal at the repository root:

```powershell
dotnet run --project .\src\SupportOpsAI.Worker\SupportOpsAI.Worker.csproj
```

The worker consumes RabbitMQ triage jobs and writes AI triage recommendations back to PostgreSQL.

## Run the Frontend

In a third terminal:

```powershell
cd frontend
npm install
npm run dev
```

Open `http://localhost:5173`.

The Vite development server proxies `/api` requests to the API at `http://localhost:5116`.

## Customer Demo Flow

1. Open the frontend login page.
2. Use the local/demo-only customer credential button, or register a new customer account.
3. Log in as the customer.
4. Create a ticket with a clear title and description.
5. Return to the ticket list.
6. Open the ticket detail page.

Expected result:

- The ticket is visible to the customer.
- The ticket starts in a pending triage state.
- A triage job is queued for background processing.

## Agent Demo Flow

1. Wait briefly for the worker to process the queued triage job.
2. Log out of the customer account.
3. Use the local/demo-only agent credential button.
4. Log in as the agent.
5. Open the ticket list and select the customer ticket.
6. Review the AI triage recommendation on the ticket detail page.
7. Approve the recommendation, edit it before approval, reject it, or retry triage if needed.

Expected result:

- The ticket detail page shows the AI-generated category, priority, confidence, summary, and rationale.
- Agent-only review controls are available for triage decisions.
- Approved or edited recommendations update the ticket review state.
- Rejected recommendations remain visible as rejected review history.

## Expected End-to-End Result

After a successful demo:

- PostgreSQL stores the customer, ticket, triage job, triage result, and audit records.
- RabbitMQ receives and delivers the triage message.
- The worker completes the triage job.
- The frontend shows a customer-created ticket and an agent-reviewed AI recommendation.

Demo credentials are for local development only and should not be treated as production accounts.
