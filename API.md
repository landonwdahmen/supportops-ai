# API

## Current Status

The API now exposes the initial Phase 2 authentication, ticket, and health endpoints. Endpoints that create or read tickets require a JWT bearer token unless noted otherwise.

## Endpoints

- `POST /api/auth/register`
- `POST /api/auth/login`
- `GET /api/auth/me`
- `POST /api/tickets`
- `GET /api/tickets`
- `GET /api/tickets/{id}`
- `POST /api/tickets/{id}/comments`
- `GET /api/health`

## Authentication

`POST /api/auth/register` and `POST /api/auth/login` return an `accessToken`. Use it as a bearer token:

```http
Authorization: Bearer <accessToken>
```

## Ticket Behavior

Ticket creation persists a ticket with `PendingTriage` status. It does not publish RabbitMQ messages, start worker processing, or call any AI provider yet.

## Deferred Decisions

- Error contract format
- API versioning strategy
- Pagination and filtering shape
- AI triage workflow endpoints
