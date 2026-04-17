# Phase Journal

This log tracks notable implementation progress across the project lifecycle.

## 2026-04-17

### Status
Local PostgreSQL persistence slice resumed and wired into the backend MVP.

### Summary
- Added a Docker Compose PostgreSQL service for local development.
- Replaced the default in-memory content path with EF Core + Npgsql-backed persistence.
- Added first-run schema application and starter content seeding so the API can boot cleanly against a local database.

### Notes
- Keep local credentials in appsettings.Local.json or environment variables only.
- This remains intentionally lean: content reads plus contact submission storage are now database-backed.

## 2026-04-17 06:41 CT

### Status
Implementation paused at a restart-safe checkpoint.

### Summary
- Work is paused so Docker can become available after a machine restart.
- Angular and .NET foundations are already in place.
- PostgreSQL has been selected as the project database direction.

### Next resume step
- Restart the machine, confirm Docker is running, and then continue the local PostgreSQL container and persistence setup.

## 2026-04-17 06:41 CT

### Status
Database provider decision finalized for implementation.

### Summary
- Standardized the project database choice on PostgreSQL.
- Updated the backend configuration defaults and local example connection settings to reflect Postgres.
- Closed the earlier provider-selection question so future persistence work and migrations can target one concrete engine.

### Notes
- Continue using local override files for machine-specific credentials.
- Keep the production hosting choice flexible, but assume PostgreSQL compatibility going forward.

## 2026-04-16 10:15 PM CT

### Status
Phase 1 backend foundation scaffolded and verified locally for the approved portfolio architecture.

### Summary
- Created the initial .NET 10 BFF foundation under backend/Portfolio.Api.
- Added RESTful starter endpoints for projects, tags, contact intake, and health checks.
- Verified that the new backend project builds successfully in the local solution.
- Added source-controlled defaults plus development, local override, and test configuration scaffolding.

### Obvious note for review
- **OPEN QUESTION:** confirm the first relational database provider to standardize for migrations and local container setup. The foundation is intentionally provider-friendly for now.

### Notes
- Keep the first backend slice thin and contract-oriented.
- Defer authentication until the approved future admin path becomes real.

## 2026-04-16 10:11 PM CT

### Status
Implementation approved for Phases 0, 1, and 2.

### Summary
- Quiet Constellation Hybrid is the confirmed implementation direction.
- Active development has begun against the approved planning package.
- Environment verification now confirms Node, npm, and the .NET 10 SDK are available locally.

### Obvious note for review
- **TIMESTAMP CORRECTION:** Earlier log entries were recorded with an incorrect time offset and have now been normalized to Central Time.
- **RESOLVED:** The earlier .NET SDK blocker is cleared, so backend foundation work can proceed normally.

### Notes
- Use this journal for milestone-level progress, not every small code edit.
- Add new entries as phases begin, complete, or materially shift direction.

## 2026-04-16 10:08 PM CT

### Status
Implementation logging initialized.

### Summary
- Created the implementation log structure.
- Established Central Time timestamp guidance.
- Confirmed that planning documents remain the source guide while development proceeds.

### Notes
- Use this journal for milestone-level progress, not every small code edit.
- Add new entries as phases begin, complete, or materially shift direction.
