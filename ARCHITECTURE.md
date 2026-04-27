# ARCHITECTURE

## System Intent

SupportOps AI is planned as a backend-centered system that will ingest support tickets, enrich and classify them with AI-assisted workflows, and route follow-up work through API and background processing components.

## Planned Solution Layers

### Domain

Core business concepts and rules will live in `SupportOpsAI.Domain`.

### Application

Use cases, contracts, orchestration, and abstractions will live in `SupportOpsAI.Application`.

### Infrastructure

Persistence, messaging, external providers, and framework integrations will live in `SupportOpsAI.Infrastructure`.

### API

HTTP entry points and request orchestration will live in `SupportOpsAI.Api`.

### Worker

Asynchronous processing and background jobs will live in `SupportOpsAI.Worker`.

## External Dependencies Planned

- PostgreSQL for relational persistence
- RabbitMQ for asynchronous messaging
- OpenAI API for future AI-assisted triage capabilities

## Phase 1 Note

This document is intentionally high level until the first functional slices are approved.
