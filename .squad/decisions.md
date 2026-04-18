# Squad Decisions

## Active Decisions

### 2026-04-18: Deployment Architecture
**Author:** Boo (Architect)  
**Status:** Proposed

#### Pipeline Strategy: Platform-Native (Railway) over Container-Based CI/CD
A container-based pipeline (Docker + GitHub Actions + registry + container host) is the right long-term pattern but the wrong starting point for a solo developer new to DevOps. The setup complexity adds 2–4 days of work that delays the launch without improving the portfolio.

**Recommendation:** Start with Railway's platform-native deploy. GitHub push triggers automatic build and deploy. Nixpacks handles .NET detection. No Dockerfile required. Managed PostgreSQL is a first-class addon in the same dashboard.

Migrate to a container pipeline when DevOps familiarity warrants it and there is a specific reason (portability, cost optimization, CI maturity).

#### First Production Host: Railway
Railway is the recommended first production platform: push-to-deploy from GitHub with no workflow file, managed PostgreSQL as a service addon, per-environment isolation, automatic SSL and custom domain configuration. Estimated cost: $5–12/month.

#### Long-Term Host: Fly.io (when a Dockerfile exists)
Fly.io is the best long-term platform for this stack once a Dockerfile is in place. Free tier adequate for portfolio + staging at low traffic. No cold starts. Container portability means no vendor lock-in. Neon or Supabase as companion managed Postgres.

#### Subdomain Strategy: Single domain with environment prefix
Use `skylersidner.com` as the single registered domain. Apply subdomains for projects and a `test-` prefix for non-production environments. Do not register separate domain names for environments.

Supporting documents: [planning/deployment-pipelines.md](../../planning/deployment-pipelines.md), [planning/hosting-options.md](../../planning/hosting-options.md)

---

### 2026-04-17: Backend Persistence MVP
**By:** Bowser (via Copilot)  
**What:** Use EF Core with Npgsql for the first persistence slice, backed by a repo-level Docker Compose PostgreSQL service and startup seeding for initial portfolio content.  
**Why:** Keeps local development simple, gives the frontend a real persisted API surface, and avoids overbuilding admin infrastructure before it is needed.

---

### 2026-04-17 06:41 CT: Standardize on PostgreSQL
**By:** Skyler Sidner (via Copilot)  
**What:** Standardize the application database on PostgreSQL.  
**Why:** User preference — this is now the chosen relational database provider for implementation, local development, and future production hosting.

---

### 2026-04-16T00:30:00-05:00: Implementation Logging Conventions
**By:** Skyler Sidner (via Copilot)  
**What:** Keep the planning files unchanged. Create a separate implementation log directory with living documents that summarize choices made during development. Each log entry should include a timestamp down to the minute and assume Central Time. If code-approach questions arise, consult the Architect. After each phase, the Retro Analyst should review performance and suggest improvements to the squad definitions.  
**Why:** User request — captured for team memory.

---

### 2026-04-16T00:20:00Z: Architecture Refinements
**By:** Skyler Sidner (via Copilot)  
**What:** Keep auth at Option A (no auth initially, future admin possible). Prefer a more RESTful BFF API. Angular should use domain-scoped data services and stores exposing signals. Encourage reusable UI components with a non-production component library route. Use a containerized DB locally with flexible cloud-ready configuration plus tooling to seed, wipe, and reseed. .NET app settings should support production defaults plus local and test override files.  
**Why:** User request — captured for team memory.

---

### 2026-04-16T00:10:00Z: Initial Architecture Planning Directive
**By:** Skyler Sidner (via Copilot)  
**What:** Produce a planning-only package for the portfolio using Angular on the front end, .NET on the back end, a relational SQL database, VS Code for local development, and a BFF-based .NET SPA hosting approach. No coding should begin until the plan is reviewed.  
**Why:** User request — captured for team memory.

---

### 2026-04-16T00:00:00Z: Portfolio Site Purpose
**By:** Skyler Sidner (via Copilot)  
**What:** The portfolio site's primary purpose is to showcase personal side projects hosted on subdomains, with resume, LinkedIn, and contact information as secondary supporting links. Work experience may be present but should not be the primary focus.  
**Why:** User request — captured for team memory.

---

## Governance

- All meaningful changes require team consensus
- Document architectural decisions here
- Keep history focused on work, decisions focused on direction
