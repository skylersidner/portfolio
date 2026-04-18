# Hosting Options

Four hosting options for the portfolio stack: Angular SPA + .NET 10 BFF/API + PostgreSQL. Each option covers both the application and the database.

Assume traffic will be light: a handful of concurrent visitors, bursty around job searches or project announcements. The database will hold a few hundred rows of portfolio content and contact submissions.

---

## 1. Render

Render is a PaaS platform with a free tier for web services and a managed PostgreSQL addon.

**How the portfolio would be hosted here**

- The .NET application is deployed as a Web Service (Docker-based or via Render's native buildpack). The Angular build output is bundled at `dotnet publish` time.
- PostgreSQL is provisioned as a Render-managed database, linked to the web service and accessible via an auto-injected connection string.
- Automatic deploys from GitHub on push to the connected branch.

**Pros**

- Completely free to start — the lowest barrier to entry of any option here
- Familiar GitHub deploy flow with zero pipeline configuration
- PostgreSQL addon is simple to connect with no external service
- SSL and custom domains are handled automatically

**Cons**

- **Free tier web services spin down after 15 minutes of inactivity.** Cold start on wake-up is 30–60 seconds — a poor first impression for a recruiter opening the portfolio
- Free PostgreSQL instances have a 90-day expiry; production use requires a paid plan
- The Individual plan ($7/month web service + $7/month PostgreSQL) adds up faster than the free tier suggests
- Render's free tier is only viable for development and experimentation, not a real production portfolio

**Estimated monthly cost**

- Free tier: $0 — but with spin-down cold starts and 90-day database expiry; unsuitable for public production use
- Paid (Individual plan): $7/month web service + $7/month PostgreSQL = **$14/month**

**Subdomain strategy compatibility**

Good. Custom domains are configured in the Render dashboard with DNS verification. Each service gets its own custom domain with auto-provisioned SSL. Per-project subdomains and `test-` prefixed staging domains both work straightforwardly.

---

## 2. Railway

Railway is a developer-focused PaaS platform with native GitHub integration, Nixpacks auto-detection for .NET projects, and managed PostgreSQL as a first-class service addon.

**How the portfolio would be hosted here**

- Connect the GitHub repository to Railway; it auto-detects the .NET application and builds it on every push. No Dockerfile required.
- Add a PostgreSQL service from the Railway dashboard. The connection string is injected into the application environment automatically via `${{Postgres.DATABASE_URL}}`.
- Multiple environments (production, staging) are isolated within the same Railway project, with separate environment variables and separate database instances per environment.
- Custom domains are configured per service with automatic SSL provisioned and renewed by Railway.

**Pros**

- Best-in-class DX for solo developers: connect GitHub, add a database, set a domain — that is the entire setup
- Managed PostgreSQL is tightly integrated; no external service to coordinate or separately configure
- Multiple environments are a first-class feature with isolated config; `test-` staging environment maps directly to a staging branch
- Nixpacks build detection means no Dockerfile to write or maintain for standard .NET projects
- Deployment logs, metrics, restart behavior, and environment config all visible in one dashboard

**Cons**

- No permanent free tier; the Hobby plan is $5/month flat regardless of traffic
- Metered compute and egress could creep up if the application is poorly configured, though this is extremely unlikely for a personal portfolio
- Railway-managed Postgres lacks some advanced DBA controls; for this use case, this is irrelevant

**Estimated monthly cost**

- Hobby plan: $5/month base + metered compute and database usage
- Realistic for this portfolio with low traffic: **$5–12/month**

**Subdomain strategy compatibility**

Excellent. Each Railway service accepts a custom domain with automatic SSL. Subdomains (`sandsimulator.skylersidner.com`) and environment prefixes (`test-www.skylersidner.com`) map directly to services. No extra infrastructure or manual cert management required.

---

## 3. DigitalOcean Droplet + Managed PostgreSQL

A Linux VPS (Droplet) running the .NET application under nginx, with a DigitalOcean Managed Database cluster for PostgreSQL.

**How the portfolio would be hosted here**

- Provision a Droplet (2GB RAM recommended; .NET's baseline memory consumption makes 1GB uncomfortably tight). Install the .NET runtime, configure nginx as a reverse proxy to the Kestrel process, and manage a systemd service unit that keeps the app running.
- PostgreSQL is a DigitalOcean Managed Database cluster ($15/month for the smallest). The connection string is set as an environment variable on the Droplet or in a `.env` file not tracked in source control.
- Deployments are either manual (SSH in, `git pull`, restart systemd) or automated via a GitHub Actions workflow using SSH deploy keys.
- SSL is managed via Certbot (Let's Encrypt) on the Droplet, with a cron job for renewal.

**Pros**

- Full control over the operating environment; no platform restrictions on configuration
- DigitalOcean Managed PostgreSQL includes automated daily backups, connection pooling via PgBouncer, and standby nodes for availability
- DigitalOcean's documentation is among the best in the industry for self-hosting workflows; the learning path is well-documented
- Scales up without changing providers; same model works if traffic grows significantly

**Cons**

- **Highest operational overhead of any option here.** You own OS patching, nginx configuration, systemd unit files, Certbot renewal, firewall rules (ufw), and deployment scripting — none of which are difficult individually, but together they represent a meaningful maintenance surface
- No automatic deploys out of the box; a GitHub Actions SSH pipeline is doable but requires setup and maintenance
- No zero-downtime deployment without a load balancer or a more complex deployment strategy
- Cost is meaningfully higher than managed platforms for equivalent workloads

**Estimated monthly cost**

- 2GB Droplet: ~$18/month + Managed PostgreSQL Starter: $15/month = **~$33/month**
- A 1GB Droplet at $6/month is possible but leaves limited headroom for the .NET runtime under load

**Subdomain strategy compatibility**

Functional but fully self-managed. Configure A records or CNAMEs per subdomain at your DNS registrar, then run `certbot --expand` or request a wildcard certificate via DNS challenge (`certbot certonly --dns-*`). Each new subdomain requires a deliberate manual step.

---

## 4. Fly.io ⭐ Best Overall Fit

Fly.io is a container-based application platform with global edge deployment, no cold starts on paid plans, a genuinely useful free tier, and strong support for .NET workloads.

**How the portfolio would be hosted here**

- Write a Dockerfile for the .NET application — a standard two-stage build using `mcr.microsoft.com/dotnet/sdk` to publish and `mcr.microsoft.com/dotnet/aspnet` to run. The Angular build is bundled at `dotnet publish` time. This is a 20-line Dockerfile; no Docker expertise required to produce it.
- Deploy with the Fly CLI: `fly launch` scaffolds the configuration, `fly deploy` builds and deploys. GitHub Actions integration is available for automatic deploys on push.
- For PostgreSQL: use **Neon** or **Supabase** as a companion managed Postgres host. Both offer a free tier sized appropriately for a personal portfolio (Neon: 0.5GB storage, generous compute). Connect via a `DATABASE_URL` environment variable set as a Fly secret.
- Custom domains are configured with `fly certs add <domain>` and DNS verification; SSL is provisioned automatically.

**Pros**

- **Free tier is genuinely usable for a low-traffic personal site.** Three shared-CPU VMs and 160GB outbound data per month at no cost. The portfolio app and a staging instance can both run within the free allowance.
- No meaningful cold starts even on free VMs; Fly keeps applications warm
- Global deployment: run in the region closest to your audience with a one-line config change
- The Dockerfile requirement creates a portable, reproducible build artifact — a useful artifact to have independent of where it is deployed
- Strong .NET community presence on Fly; the documentation and community examples are reliable

**Cons**

- **Requires a Dockerfile.** A small but real barrier for someone new to containers. The .NET base images are well-documented and the Dockerfile itself is minimal, but debugging a broken container build is not always fast.
- The `flyctl` CLI is the primary interface; there is no point-and-click GitHub deploy. GitHub Actions integration is available but requires a workflow file.
- Fly's built-in Postgres (`fly postgres`) is self-managed on Fly infrastructure — not a fully managed service. Using Neon or Supabase as the database is a better production posture.
- Dashboard is functional but less polished than Railway or Render for day-to-day visibility

**Estimated monthly cost**

- Free tier: **$0** for light traffic — portfolio app and staging both comfortably within three-VM allowance
- If traffic exceeds free tier: ~$1.94/month per additional shared-CPU VM
- Neon or Supabase PostgreSQL: **$0** on free tier (sufficient for this project indefinitely at current scale)
- Realistic paid cost if the portfolio sees sustained traffic: **$5–10/month**

**Subdomain strategy compatibility**

Good. Each Fly application accepts a custom domain via `fly certs add`. Map subdomains per app using CNAME records. Wildcard certificates are supported via DNS challenge. No limitations on the `test-` prefix subdomain convention.

---

## Recommendation Summary

For the first production deployment of this portfolio, **Railway is the right starting point.** It combines the best developer experience for solo deployment — GitHub push-to-deploy, managed PostgreSQL in the same dashboard, and automatic SSL — with minimal setup time and no prerequisite Docker knowledge. At $5–12/month, it is appropriately priced for a personal project.

**Fly.io is the best long-term home** once a Dockerfile is in place. Its free tier, absence of cold starts, and portable container-based model make it the most cost-effective and technically sound option at portfolio scale. The only reason not to start there is the container learning curve.

Deploy on Railway first. When the portfolio is live and stable, writing a Dockerfile and moving to Fly.io is a deliberate, low-urgency improvement that also solves the portability concern. Render's free tier is not suitable for a public portfolio due to cold starts. DigitalOcean with a Droplet is worth revisiting only if full OS-level control becomes a genuine requirement.
