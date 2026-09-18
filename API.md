# API

## Current Status

The API exposes authentication, ticket, health, and triage review endpoints. Ticket and triage endpoints require a JWT bearer token unless noted otherwise.

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

## Current Limits and Potential Enhancements

The exception handler returns an `error` array for handled service and validation exceptions, with status codes for bad requests, unauthorized or forbidden access, missing resources, conflicts, and unexpected failures. Framework-generated model-binding and authentication responses may use different formats.

Ticket listing returns an unpaginated list, newest first, limited by the caller's access. There are no pagination or filter query parameters.

Consistent error responses across all HTTP failure paths, API versioning, pagination and query filtering, and knowledge-base search are optional future enhancements, not unfinished requirements for the portfolio version.
