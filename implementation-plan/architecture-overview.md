# Architecture Overview

## 1. System Context

This portfolio should be treated as a lightweight product platform rather than a single static resume page.

Its primary purpose is to:

- present a curated directory of side projects
- route visitors to project experiences hosted on subdomains
- explain what each project is, why it exists, and what role it plays in the broader body of work
- provide secondary trust signals such as resume, LinkedIn, and contact options

The portfolio root domain acts as the front door. Individual projects may live on their own subdomains, each with separate deployment lifecycles, but the portfolio remains the central discovery and identity layer.

### Primary user groups

- recruiters and hiring managers evaluating depth and range of work
- peers or collaborators exploring projects and technical interests
- the site owner maintaining featured content over time

### Architectural implication

Because the site is mostly public-facing but may eventually need protected administration, content management, analytics, or contact workflows, it benefits from a structured application architecture rather than a purely static site.

---

## 2. Recommended Architecture Style

The recommended shape is a .NET web host using a Back End for Front End approach that launches the Angular single-page application in the browser.

In practice, this means:

- the browser reaches the portfolio domain through the .NET application
- the .NET host is responsible for the app shell, edge security concerns, future admin-session boundaries, and a stable RESTful API surface for the front end
- Angular owns the interactive user experience once loaded and composes page-ready state in its store layer
- the SQL database stores structured portfolio content and any future operational data

This is a good fit because it keeps the browser client focused on presentation while reserving security-sensitive and integration-heavy responsibilities for the server.

### Why BFF fits this portfolio

A BFF layer is useful even for a relatively small site when:

- the browser should consume domain-scoped, RESTful resources rather than raw database shapes
- secrets must never be exposed to the browser
- future admin features may require secure session handling even though the initial release stays public-only
- the application may later integrate with email, analytics, resume generation, or project metadata services
- the front end benefits from a predictable API contract while still retaining flexibility to compose UI-specific state client-side

---

## 3. Major Responsibilities by Component

| Component | Primary Responsibilities | What Should Not Live Here |
| --- | --- | --- |
| Angular app | page composition, navigation, filtering, polished interactions, responsive layout, and store-managed client state for viewing content | direct database access, secret management, authentication token storage strategy that bypasses server controls |
| .NET BFF and API | serve the SPA entry point, expose RESTful domain-scoped endpoints, handle future auth or session logic, validate inputs, orchestrate contact or admin workflows, and enforce security rules | heavy client-side presentation logic, long-term business reporting, direct exposure of internal infrastructure concerns to the browser |
| SQL database | store canonical project metadata, tags, descriptions, links, featured ordering, contact records, and future admin content | rendering concerns, user interface rules, public-facing formatting logic |

---

## 4. Suggested Modules and Layers

A practical first architecture should stay simple while leaving room for growth.

### Angular application areas

- **App shell** — top-level layout, navigation, footer, route framing
- **Home and launch hub** — featured projects, quick story, primary calls to action
- **Project directory** — browsable grid or list of projects with tags, status, and outbound links
- **Project detail or case study pages** — richer narrative for selected projects when needed
- **About and resume area** — secondary professional profile content
- **Contact flow** — a simple form or contact handoff path
- **Store layer** — data composition after calling domain-scoped data-access services, with signals exposed for components to consume and refine
- **Shared design system** — typography, cards, buttons, spacing, themes, reusable abstract UI components, and sensible shared interaction patterns
- **Component showcase route** — a style-guide or library page available only in non-production environments for reviewing reusable component states

### Front-end composition guidance

The Angular front end should balance consistency with clear feature ownership.

Recommended front-end planning rules:

- Use **reusable shared UI primitives** where they genuinely improve consistency and speed, especially for buttons, links, cards, menu items, focused list items, and similar presentation building blocks.
- Keep **domain-specific feature components** as first-class parts of the application for areas such as Projects, About, Contact, and related page sections.
- Encourage reuse, but do **not** force abstraction past the point where readability, intent, or maintainability would suffer.
- Prefer front-end stores that **expose Angular signals** so route-level and section-level views can compose state cleanly without unnecessary coupling.
- Maintain a **component showcase route or page in non-production environments only** so component states, variants, and styles can be reviewed safely without leaking internal review surfaces into production.

### .NET server-side areas

- **Web host layer** — runtime entry point, configuration, environment setup, routing into the SPA
- **BFF endpoints** — RESTful, domain-scoped endpoints that provide stable application contracts without being hard-wired to individual views
- **Application services** — business workflows such as content retrieval, featured ordering, and contact intake
- **Security layer** — future-ready authentication, authorization, session policy, and anti-forgery controls for a later admin area
- **Infrastructure adapters** — email providers, telemetry, external profile integrations if added later
- **Persistence layer** — SQL access and mapping logic

### SQL data domains

- **Projects** — title, slug, description, status, hosting link, repository link, thumbnail, priority
- **Tags and technology labels** — stack keywords and categories
- **Featured content** — home page ordering and spotlight controls
- **Case studies or narrative sections** — optional long-form writeups
- **Contact submissions** — if the contact workflow is stored rather than purely forwarded
- **Audit or content history** — optional, only if admin tooling becomes real

### Layering recommendation

Use a clear layered structure from the start:

1. presentation and UI contract layer
2. application workflow layer
3. domain and content rules
4. infrastructure and data access layer

That separation keeps future changes manageable without introducing unnecessary complexity now.

---

## 5. Authentication and Session Options

Because the public portfolio is mostly anonymous-read, authentication should be driven by actual administrative need rather than added for its own sake.

### Option A — No public auth, protected admin later

- the public site remains fully open
- administrative editing features are deferred or disabled initially
- content is updated through direct database management or a controlled release process

**Best when:** the first release is mainly a showcase and content changes are infrequent.

### Option B — External identity provider with server-managed session

- use a trusted identity provider for admin sign-in
- .NET manages the authenticated session using secure server-side cookie patterns
- Angular relies on the BFF for authenticated state rather than holding sensitive tokens directly

**Best when:** an admin area is expected in the near future and security simplicity matters.

### Option C — Token-heavy SPA model

- the Angular client handles more direct auth concerns
- APIs expect bearer tokens from the browser
- this introduces more client security and token lifecycle complexity

**Best when:** the portfolio evolves into a larger app platform with broader client-driven integrations.

### Recommended approach

Remain at **Option A** for the initial application: no authentication in the first public release. At the same time, keep the server and routing structure ready to add a protected admin area later using **Option B** patterns when there is a real editorial workflow to support. That gives a fast path to launch without closing off a secure future admin story.

---

## 6. Local Development Topology vs Production Topology

### Local development

A practical local setup in VS Code should support fast front-end iteration without drifting away from the real production shape.

Recommended local behavior:

- Angular runs in a developer-friendly mode for rapid UI iteration
- the .NET application remains the main server-side runtime and API host
- local requests can be routed so the Angular app and BFF still behave like one system
- the development database runs in a containerized local setup for consistency across machines
- configuration stays cloud-friendly and provider-neutral so the same planning model can move cleanly to managed hosting later
- development workflows should support seed, wipe, and reseed cycles for realistic mock and test data

This preserves the BFF contract early and reduces surprises later.

### Production topology

In production, the shape should be straightforward:

- root domain points to the .NET-hosted portfolio application
- Angular assets are served through the production web application path, optionally with CDN support for static asset performance
- the .NET BFF handles server endpoints and session-aware behavior
- SQL is provided by a managed relational database service
- subdomains for side projects are deployed independently, but the portfolio stores the metadata and links to them

### Key difference to respect

Local development may split concerns across tools for speed, but production should behave as a single, cohesive web platform from the visitor perspective.

---

## 7. Deployment and Hosting Considerations

The architecture should stay cloud-neutral, but the hosting model should support these needs:

- reliable HTTPS for the root domain and project subdomains
- simple deployment of the .NET host and Angular assets together
- secure secret storage for any mail, analytics, or identity provider configuration
- managed SQL backups and operational monitoring
- basic observability for errors, uptime, and contact workflow health

### Hosting guidance

A single managed web hosting environment for the portfolio root app is the cleanest first step. Avoid splitting the Angular and .NET deployment across many platforms unless there is a strong operational reason.

### Operational checklist for later implementation

- environment-specific configuration strategy, including shared defaults and local or test overrides
- secrets management and rotation
- logging and health checks
- custom domain and certificate management
- rollback strategy for deployments
- basic analytics and traffic monitoring

---

## 8. Risks and Tradeoffs

### Risk: overengineering a portfolio

A portfolio can become harder to ship if it is treated like a large enterprise app too early.

**Mitigation:** keep the first release focused on public content discovery and project launch routing.

### Risk: under-planning content structure

If project entries and case studies are not modeled clearly, the UI may become inconsistent and hard to maintain.

**Mitigation:** define a stable content model early, especially for projects, tags, featured ordering, and outbound links.

### Risk: authentication complexity too early

Adding sign-in, roles, and admin tooling before they are truly needed can slow the launch.

**Mitigation:** begin public-only, then add server-managed admin access when there is a real editorial workflow to support.

### Tradeoff: BFF simplicity vs direct API exposure

The BFF adds one more layer, but it also creates a safer and cleaner contract for the front end.

**Recommendation:** keep the BFF thin and contract-oriented rather than turning it into an overly broad monolith or a set of page-specific endpoints.

### Tradeoff: SQL-backed content vs static content files

A SQL model supports future growth and administration, but it adds operational overhead compared to static content.

**Recommendation:** still proceed with SQL if long-term maintainability, sorting, tagging, and structured project content are important from the start.

---

## 9. Recommended First Implementation Path

The most practical first path is:

### Phase 1 — Launch-focused public portfolio

- build the public portfolio as a discoverability hub for projects
- prioritize the home page, project listing, project detail storytelling where useful, and contact options
- store project and profile content in SQL with a simple schema
- expose only the RESTful, domain-scoped server endpoints needed by the Angular application
- defer admin editing, complex analytics, and advanced personalization

### Phase 2 — Operational hardening

- add monitoring and deployment automation
- improve content management workflow
- introduce protected admin access if needed
- strengthen observability and content governance

### Phase 3 — Platform expansion

- add richer project analytics or engagement tracking
- integrate external profile and content sources where useful
- expand the portfolio into a fuller product directory if the number of hosted subdomain projects grows significantly

### Recommended starting decision set

Before any coding begins, the team should lock in these choices:

1. public information architecture and page inventory
2. minimum viable SQL content model
3. whether admin editing is part of the first release or deferred
4. target hosting platform and deployment workflow
5. contact flow behavior and retention needs

If those decisions are made early, the Angular and .NET implementation can proceed with much less churn.

---

## 10. Final Recommendation

Use a simple but disciplined architecture:

- Angular for the visitor experience
- .NET as the hosting and BFF layer
- SQL as the canonical content store
- public-first release scope with admin/auth deferred unless clearly needed

This gives the portfolio a professional foundation without slowing delivery with unnecessary platform complexity.