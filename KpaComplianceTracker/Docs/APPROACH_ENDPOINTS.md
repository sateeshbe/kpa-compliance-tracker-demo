# APPROACH (3 Endpoints) — Angular + .NET Core API

## What this app does

* Angular uploads a JSON array of tasks to a .NET Core API.
* API upserts tasks into the database and exposes a simple list and update flow.

## Architecture (high-level)

Angular (Standalone + HttpClient) -> JSON over HTTP -> .NET Core API (Controllers + EF Core) -> SQLite dev / SQL Server prod

## Data (conceptual)

* Task fields: id (Guid), title (required), category?, site?, owner?, status?, dueDate (string, kept as given), updatedAt (server).
* Separation: UI DTOs ↔ API DTOs ↔ EF Entity.

## API — 3 endpoints (behavioral summary)

1. POST /api/upload

   * Input: JSON array of task DTOs.
   * Behavior: For each item: if id present and found -> update; else create.
     Trim strings; set updatedAt server-side; keep dueDate as provided (no timezone math).
   * Output: counts (e.g., { inserted, updated } or { saved }).

2. GET /api/tasks

   * Returns list of tasks ordered by updatedAt desc (demo keeps it simple; paging/filters optional).

3. PUT /api/tasks/{id}

   * Partial update: apply changed fields; set updatedAt; return updated row or 404.

## Frontend flow (concise)

* Upload: pick .json -> parse -> show count -> POST /api/upload -> show result.
* List/Edit: load GET /api/tasks -> open edit -> PUT /api/tasks/{id} -> refresh UI.

## Validation \& errors (lightweight)

* title required; string length caps; friendly error banner in UI.
* API returns ProblemDetails-style errors for 4xx/5xx.

## Production notes

* Config via appsettings/env; CORS limited to dev origin.
* EF migrations; swap SQLite <-> SQL Server via connection string.
* Structured logging; optional request correlation id.

## Run (quick)

* API: dotnet ef database update; dotnet run
* UI: npm i; ng serve
