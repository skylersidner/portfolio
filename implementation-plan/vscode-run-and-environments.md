# VS Code Run and Environment Plan

## Planning intent

This document describes how local development and environment management should work once implementation begins. It reflects the approved target architecture: Angular for the front end, a .NET BFF for server-side orchestration, and a relational SQL database for persisted content and submissions. It is a planning artifact only. Concrete run configurations, tasks, launch settings, and deployment scripts should be created after scope and architecture are approved.

## Recommended local development approach in VS Code

VS Code should be the primary local workspace for the portfolio project, with a setup that supports fast iteration across the full stack while keeping operational overhead reasonable.

### Workspace expectations

- One workspace for the portfolio repository
- Separate terminals for the Angular app, the .NET BFF, and the SQL database or local container services
- Clear README instructions for first-time setup
- Shared scripts or tasks for consistent local startup across the approved stack

### Day-to-day developer flow

A typical local workflow should look like this:
1. Pull latest changes and install dependencies.
2. Start the Angular development server.
3. Start the .NET BFF.
4. Start the relational SQL database locally or through the approved development container setup.
5. Validate the integrated experience in the browser from a single local entry point.

## Expected run modes

Because this portfolio is side-project-first, the runtime model should stay practical and easy to operate, but it should still represent the approved full-stack architecture from day one.

### Front end

**Recommended mode**
- Hot-reload Angular development server in VS Code
- Fast local preview at a single front-end port
- API requests proxied to the local .NET BFF for integrated testing
- A non-production component-library or style-guide route for previewing reusable component states during development and review

**Reasoning**
- The core value of the portfolio is presentation and storytelling
- Angular remains the primary delivery surface, but it should be validated against the real application boundary rather than a temporary front-end-only setup
- Reusable abstract components benefit from a safe preview surface before they are used in domain-specific pages

### Backend or BFF

**Recommended mode**
- Always run a thin local .NET BFF process alongside the Angular app
- Handle content delivery, contact submission, secret-backed integrations, and response shaping for Angular views

**Expectation**
- The BFF is part of the approved MVP architecture, not a later add-on
- It should stay thin and focused rather than become a heavy application layer

### Database

**Recommended mode**
- Run a relational SQL database in development from the first implementation phase
- Prefer a containerized local database for day-to-day development rather than complex self-hosting
- Keep the configuration cloud-friendly and provider-neutral so later hosting choices remain flexible

**Expectation**
- Portfolio content, contact submissions, and supporting metadata should have a real persistence layer early
- Seed data and migrations should make local setup repeatable and low-friction
- Developers should be able to wipe and reseed data safely when testing new flows or refreshing demos

### Integrated experience

**Target developer experience**
- A developer should be able to run the full stack predictably from VS Code with minimal manual steps
- For isolated UI work, the Angular app may still use stable seeded data, but the default workflow should target the live local BFF and database
- The integrated experience should feel like one application even if it is composed of multiple processes

## Environment strategy

### Development

Development should prioritize speed, safe experimentation, and easy onboarding.

Recommended characteristics:
- Local-first configuration
- Relaxed non-production service limits
- Test credentials only
- Helpful logs and debug visibility
- Repeatable startup for Angular, the .NET BFF, and the SQL database
- Component preview routes or style-guide pages enabled only in non-production environments
- Reliable seed, wipe, and reseed workflows for mock and test data

### Staging

Staging should act as the closest practical preview of production.

Recommended characteristics:
- Deployed from the main pre-release branch or controlled preview builds
- Uses realistic environment variables and integration paths
- Suitable for portfolio review, regression checks, and final content sign-off
- Protected from accidental public misuse where applicable

### Production

Production should optimize for reliability, professionalism, and minimal maintenance burden.

Recommended characteristics:
- Stable domain and HTTPS
- Managed hosting with straightforward rollback support
- Tight secret management
- Low-touch deployment process suitable for a solo owner
- Performance and accessibility checks as part of release readiness
- Internal-only review surfaces such as the component showcase explicitly disabled or excluded from production

## Configuration and secrets handling

### Configuration strategy

The project should separate:
- source-controlled defaults for non-sensitive settings,
- environment-specific overrides,
- secret values stored outside the repository.

Recommended planning rules:
- Keep configuration naming consistent across front end, BFF, and deployment targets
- Use appsettings.json for source-controlled defaults in the .NET layer
- allow environment-specific local and test overrides without changing shared defaults
- Prefer a small number of well-documented environment variables
- Avoid embedding URLs, tokens, or provider IDs directly in source files
- Use one reference example file to show required values without exposing secrets

### Secrets handling

Secrets should never be committed to the repository.

Recommended handling:
- local secrets stored in ignored environment files,
- staging and production secrets stored in the hosting platform's secret manager,
- any third-party API keys routed through the BFF when they must remain private.

For a portfolio project, the default assumption should be that the front end remains public and only the BFF handles sensitive credentials.

## Deployment expectations

The deployment model should match the scale of the project while supporting the approved full-stack design.

### Recommended expectation for MVP

- Angular front end deployed to a static-friendly platform or lightweight web host
- .NET BFF deployed as a small managed web service
- Relational SQL database provisioned in a managed service tier appropriate for a personal portfolio
- Simple preview deployment flow for review
- Minimal manual operations after setup

### Recommended expectation for later phases

- Expand BFF responsibilities only where they improve maintainability or security
- Introduce a protected admin area only when there is a real editorial need
- Tune the managed database and operational setup as content volume and integrations grow
- Keep infrastructure decisions reversible and low-cost

## Approval note

This is a planning document only. Specific VS Code tasks, launch profiles, run scripts, container setup, and environment files should be created as part of implementation, using the approved Angular, .NET BFF, and relational SQL architecture as the baseline.
