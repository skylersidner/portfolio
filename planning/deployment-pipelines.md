# Deployment Pipeline Options

This document compares two viable deployment strategies for the portfolio: a container-based pipeline and a platform-native PaaS deployment. Both assume the current stack: Angular SPA + .NET 10 BFF/API + PostgreSQL.

---

## Approach A: Container-Based Pipeline (Docker + CI/CD)

### How it works

1. A GitHub Actions workflow triggers on push to the deployment branch (e.g., `main`).
2. The workflow builds a Docker image containing the published .NET application with the Angular build output embedded (bundled via `dotnet publish`).
3. The image is tagged (e.g., with the commit SHA or `latest`) and pushed to a container registry — GitHub Container Registry (GHCR) is the natural choice given GitHub Actions.
4. The target host (a VPS, Fly.io, DigitalOcean App Platform, Azure Container Apps, etc.) pulls the new image and restarts the service.
5. PostgreSQL lives separately — either on a managed service (DigitalOcean Managed Databases, Neon, Supabase) or as a container with a persistent volume (the latter requiring volume management and backup handling).

### Infrastructure and tooling involved

- **GitHub Actions** for CI orchestration and image builds
- **Docker** for image construction and packaging
- **GHCR or Docker Hub** for image storage and versioning
- **Container host** — a VPS with Docker, Fly.io, DigitalOcean App Platform, or Azure Container Apps
- **Managed PostgreSQL** (strongly preferred over containerized Postgres in production)
- A **Dockerfile** for the .NET application — multi-stage builds are the standard pattern
- Secrets management on the host for connection strings, environment variables, and registry credentials

### Pros for this project

- **Consistent environments.** The container image is identical in development, staging, and production. Environment-specific configuration is injected at runtime via environment variables, not baked in.
- **Portable.** The stack is not locked to any one hosting provider. Moving between hosts is a Dockerfile and a few environment variable changes away.
- **Production-grade pattern.** Working through container pipelines once teaches tooling that applies across many future projects. The learning investment compounds.
- **Supports multiple environments cleanly.** Separate workflows can build and deploy test vs. production from different branches with different image tags.

### Cons and complexity risks

- **Non-trivial setup for someone new to DevOps.** Writing a correct multi-stage Dockerfile for a .NET + Angular app, wiring GitHub Actions secrets, configuring a container registry, and standing up a host that runs containers reliably involves working through several unfamiliar systems simultaneously.
- **Failure modes are hard to debug remotely.** Container build failures, missing environment variables, registry authentication errors, and Docker networking problems are each their own learning surface.
- **Database is not handled by the pipeline.** A separate plan for PostgreSQL is always required — managed service adds cost; running Postgres in a container adds volume and backup complexity.
- **Not zero-maintenance.** OS-level container hosting means security patches, restart policies, uptime monitoring, and log management are your responsibility unless you layer a managed container platform on top.

### Operational overhead

Moderate to high initially, settling to low once stable. Expect several hours of setup and debugging on first deployment. Ongoing maintenance is minimal if the host is a managed container platform — but you permanently own the pipeline, Dockerfile, and registry configuration.

---

## Approach B: Platform-Native Deploy (Railway)

Railway is the recommended alternative. It connects directly to a GitHub repository, auto-detects the .NET application via Nixpacks buildpacks (no Dockerfile required), builds and deploys on every push, and provisions managed PostgreSQL as a first-class addon — all from one dashboard.

### How it works

1. Connect the GitHub repository to Railway. Select the branch to deploy from (e.g., `main`).
2. Railway detects the .NET project via Nixpacks and builds it automatically. The Angular build output is bundled into the .NET publish step (`dotnet publish` produces a self-contained artifact that includes the SPA files under `wwwroot`).
3. On every push to the connected branch, Railway rebuilds and redeploys automatically. No workflow file required.
4. Add a PostgreSQL service from the Railway dashboard. Railway injects the connection string into the application's environment as `${{Postgres.DATABASE_URL}}`.
5. Custom domains are configured per service in the dashboard; Railway provisions and auto-renews SSL certificates.

For environments: create a staging environment within the same Railway project, pointed at a `test` or `staging` branch. All configuration — connection strings, API keys, feature flags — is scoped per environment. Each environment gets its own database instance.

### Infrastructure and tooling involved

- **Railway** — the only platform to learn and operate
- **GitHub** — already in use; Railway connects via OAuth app
- No Dockerfile required, though Railway will use one if present
- **Railway-managed PostgreSQL** (Postgres 15, point-in-time backups available on paid plans)
- Railway dashboard for environment variables, secrets, domain configuration, and deployment logs

### Pros for this project

- **Lowest time-to-deployed for a solo developer.** The path from "pushed to GitHub" to "running in production with a real domain and SSL" is measured in minutes, not days.
- **Managed PostgreSQL is a first-class citizen.** No separate service to coordinate. Railway injects the connection string automatically and the database is visible in the same project dashboard.
- **Automatic deploys from GitHub.** Push to `main`, production updates. No workflow file to write or maintain.
- **Per-environment configuration is built in.** Multiple environments with isolated variables and separate database instances are a first-class Railway feature, not a workaround.
- **No Docker knowledge required to start.** Nixpacks handles build detection for standard .NET projects. A Dockerfile can be added later for more control without breaking anything.
- **Cost is predictable and low.** The Hobby plan at $5/month covers base compute; PostgreSQL storage is metered and negligible for a personal portfolio.

### Cons and limitations

- **Vendor lock-in is real but low-risk at this scale.** Migrating away means writing a Dockerfile and a GitHub Actions workflow — both skills worth developing eventually, and neither is urgent for an initial launch.
- **No permanently free tier.** Railway ended its free tier in 2023. The Hobby plan costs $5/month regardless of usage. This is not meaningfully expensive, but it is not zero.
- **Less control than a VPS.** Custom system packages, nginx routing rules below the application layer, and OS-level settings are not accessible. For this portfolio, none of that matters.
- **Railway-managed Postgres is not AWS RDS.** If data portability, SLA guarantees, or specific extension requirements become a concern later, a dedicated database provider (Neon, Supabase) can be substituted with a connection string change.

### Operational overhead

Low. Initial setup is roughly 30–60 minutes for first deployment including domain configuration. Ongoing maintenance is near-zero: Railway handles SSL renewal, uptime monitoring with alerts, and automatic restarts. Your responsibility is application-level health.

---

## Recommendation

**Use Railway.**

The container pipeline is the right long-term pattern and the right choice once DevOps familiarity is established. For a solo developer launching a personal portfolio who is new to production hosting, it is the wrong starting point. Writing a correct Dockerfile, wiring a CI pipeline, managing registry credentials, and configuring a container host that stays reliable adds 2–4 days of work that does not improve the portfolio — it just delays it.

Railway delivers the same outcome — real domain, HTTPS, automatic deploys, managed Postgres — in roughly an hour. The cost is $5–12/month. The skills gap is minimal. When the portfolio is live and stable, migrating to a container-based pipeline is a deliberate, low-urgency improvement rather than a launch blocker.

Start with Railway. Move to containers when you have a reason.

---

## Subdomain and Environment Strategy

### The two approaches

**Approach A: Subdomain prefix on a single domain**

| Environment | URL |
|---|---|
| Production portfolio | `www.skylersidner.com` |
| Staging portfolio | `test-www.skylersidner.com` |
| Production sand simulator | `sandsimulator.skylersidner.com` |
| Staging sand simulator | `test-sandsimulator.skylersidner.com` |

**Approach B: Separate domain names per environment**

| Environment | Domain |
|---|---|
| Production | `skylersidner.com` |
| Staging | `testskylersidner.com` |
| Development | `devskylersidner.com` |

### Analysis

| Concern | Subdomain prefix | Separate domains |
|---|---|---|
| DNS management | One zone; all records in one place | Multiple zones; each domain requires its own NS setup and record management |
| Annual cost | One domain (~$10–15/year) | $10–15/year per additional domain; $20–30/year for two extra environments |
| SSL certificates | A wildcard cert (`*.skylersidner.com`) covers every subdomain | Each domain needs its own cert, or per-domain cert provisioning via your host |
| Browser behavior | Consistent origin behavior across environments | Separate origins eliminate any cross-environment cookie risk (minor, theoretical advantage) |
| Clarity of non-production URLs | `test-www.skylersidner.com` is unambiguous | `testskylersidner.com` is marginally cleaner in appearance but not meaningfully so |
| Setup complexity | Moderate — one wildcard DNS record or per-subdomain CNAMEs, wildcard cert once | Higher — register domain, configure new DNS zone, provision certs, remember to renew |

### Recommendation

**Use the subdomain prefix approach on a single domain.**

Registering additional domains for environments adds real and recurring cost, increases DNS surface area, and creates more things to track when a domain renewal lapses. For a personal portfolio, the operational simplicity of a single DNS zone with a wildcard certificate far outweighs any benefit of separate domain names.

Configure a wildcard DNS record (`*.skylersidner.com → your host IP or CNAME`) or per-service CNAMEs. Railway and comparable platforms support custom domains per service with automatic SSL — this maps naturally to the per-project subdomain model without additional infrastructure.

The `test-` prefix is unambiguous, visually obvious, and a well-established convention. It does not need further justification.
