# API

## Current Status

The API exposes authentication, ticket, health, and Phase 3 triage review endpoints. Ticket and triage endpoints require a JWT bearer token unless noted otherwise.

## Endpoints

- `POST /api/auth/register`
- `POST /api/auth/login`
- `GET /api/auth/me`
- `POST /api/tickets`
- `GET /api/tickets`
- `GET /api/tickets/{id}`
- `POST /api/tickets/{id}/comments`
- `GET /api/tickets/{id}/triage`
- `POST /api/tickets/{id}/triage/approve`
- `POST /api/tickets/{id}/triage/edit`
- `POST /api/tickets/{id}/triage/reject`
- `POST /api/tickets/{id}/triage/retry`
- `GET /api/health`

## Authentication

`POST /api/auth/register` and `POST /api/auth/login` return an `accessToken`. Use it as a bearer token:

```http
Authorization: Bearer <accessToken>
```

## Triage Workflow

Ticket creation saves a ticket as `PendingTriage`, creates a queued `TriageJob`, and publishes a RabbitMQ message. The worker consumes the message, runs the configured AI triage provider, stores a `TicketTriageResult`, marks the job `Completed`, and updates the ticket to `TriageCompleted`.

## Human Review

Customers can view triage results for their own or assigned tickets. `Agent` and `Admin` users can approve, edit, reject, or retry triage.

Approving applies the AI category and priority to the ticket. Editing applies reviewer-selected category and priority. Rejecting marks the recommendation rejected with notes. Retrying creates and publishes a new triage job when no job is queued or processing.

## Deferred Decisions

- Error contract format
- API versioning strategy
- Pagination and filtering shape
- Advanced AI prompt design and knowledge base search
