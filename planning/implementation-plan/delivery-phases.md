# Portfolio Delivery Phases

## Planning intent

This document outlines a realistic, side-project-first delivery path for turning the portfolio concepts in this repository into a production-ready experience. The approved target architecture is full-stack from the outset: an Angular front end, a thin .NET BFF, and a relational SQL database. The emphasis is on phasing that stack sensibly so an MVP ships quickly without treating the backend or database as speculative work.

## Recommended build order

1. **Choose and lock the primary direction** from the approved design proposal.
2. **Stand up the full-stack foundation** with the Angular app shell, .NET BFF skeleton, and relational database baseline.
3. **Build the responsive MVP experience on top of that foundation** with real portfolio content and polished project pages.
4. **Add contact, analytics, SEO, and content-management foundations** through the approved BFF and data model.
5. **Harden for staging and production** with performance, accessibility, observability, and deployment checks.
6. **Add portfolio enhancements later** once the core platform is stable and useful.

## Phase plan

| Phase | Goal | Scope focus | Delivery note |
| --- | --- | --- | --- |
| Phase 0 - Direction and architecture lock | Reduce ambiguity before engineering starts | Design selection, content inventory, page map, technical decision record, full-stack architecture confirmation | Short planning step that prevents rework |
| Phase 1 - Foundation slice | Establish the approved platform baseline | Angular shell, .NET BFF project, SQL schema baseline, local integration, initial deployment wiring | Highest priority because everything builds on it |
| Phase 2 - MVP experience | Launch a polished portfolio quickly on the approved stack | Core pages, real content, responsive layout, project discovery, BFF-backed content and contact flow | First public release |
| Phase 3 - Experience and operations upgrades | Improve credibility, maintainability, and insight | SEO, analytics, accessibility, admin-oriented content management, observability, performance tuning | Near-term release hardening |
| Phase 4 - Growth enhancements | Expand the portfolio over time | Blog, richer filtering, experiments, advanced storytelling, deeper content operations | Later enhancement track |

## MVP scope versus later enhancements

### MVP scope

The MVP should be intentionally narrow so the portfolio can ship as a serious personal brand asset without overengineering. The architecture, however, is still full-stack: the Angular client, .NET BFF, and SQL persistence should all be present in the first implementation slice, even if some features remain simple.

Included in MVP:
- One approved visual direction implemented cleanly
- Home or landing page with clear introduction and value proposition
- Projects section with 2-4 real case studies or polished summaries
- About section and clear contact path
- Responsive behavior for mobile, tablet, and desktop
- BFF-backed content delivery and contact submission endpoints
- Relational storage for portfolio content, contact submissions, and supporting metadata
- Basic accessibility, metadata, performance hygiene, and a simple deployment pipeline

Explicitly not required for MVP:
- Full CMS authoring experience
- Complex role-based admin tooling
- Personalization or account features
- Complex animation system beyond tasteful polish
- Custom analytics dashboards or operational back office

### Later enhancements

These can follow once the portfolio is live and providing value:
- Lightweight CMS or richer editorial workflow on top of the existing data model
- Blog or writing section
- Search and filtering across work samples
- Deeper storytelling modules for individual projects
- Contact workflow integrations, email automation, or lead tracking
- Expanded reporting and content operations built on the existing BFF and database foundation

## Phase detail and acceptance themes

### Phase 0 - Direction and content lock

**Objectives**
- Confirm the primary portfolio direction and visual system
- Decide which pages, sections, and stories are in scope for first release
- Identify missing assets, copy, and project screenshots

**Key outputs**
- Approved direction
- Final sitemap and content outline
- Initial engineering approach and deployment target

**Acceptance criteria themes**
- Stakeholders can clearly state what is being built first
- Every MVP page has an owner and required content source
- Open design questions are reduced to a manageable list

### Phase 1 - Foundation slice

**Objectives**
- Stand up the approved Angular application, .NET BFF, and relational SQL database as one working platform
- Establish the initial schema, environment configuration, and local integration path
- Create a thin but production-shaped baseline that later UX work can build on safely

**Key outputs**
- Angular application shell and routing baseline
- Shared UI primitives for common interaction patterns such as buttons, links, cards, menu items, and focused list items
- Front-end stores designed around Angular signals to support clean view composition
- A non-production-only component showcase route or page for visual review of component states and styles
- .NET BFF project with health, content, and contact API boundaries defined
- Initial SQL schema and migration plan aligned with the data model
- Local integrated startup flow and first shared deployment wiring

**Acceptance criteria themes**
- The Angular app, BFF, and database run together reliably in development
- Core data can be loaded through the BFF rather than being trapped in front-end-only structures
- Shared primitives improve consistency without replacing clear domain-focused feature components
- The platform is ready for real content and UX work without a later architecture reset

### Phase 2 - MVP experience

**Objectives**
- Build the production visitor experience on the approved stack
- Replace placeholder content with portfolio-ready copy and assets
- Publish a working preview that feels trustworthy and cohesive

**Key outputs**
- Responsive homepage
- Domain-specific feature components for Projects, About, Contact, and related page sections
- Shared layout, typography, navigation, and component patterns
- BFF-backed content responses and persisted contact handling
- Initial production deployment

**Acceptance criteria themes**
- The site loads reliably and reads well on common screen sizes
- Navigation, project discovery, and contact path are intuitive
- The portfolio communicates technical ability and product thinking within a few minutes of browsing
- The core user journeys are served through the real full-stack architecture

### Phase 3 - Experience and operations upgrades

**Objectives**
- Strengthen hiring-manager and collaborator confidence
- Improve content depth, accessibility, discoverability, observability, and maintainability

**Key outputs**
- Better project narratives and proof points
- SEO metadata and social sharing support
- Basic analytics and event tracking
- Accessibility review fixes and performance cleanup
- Admin-oriented content and operational improvements on top of the existing BFF and database

**Acceptance criteria themes**
- Lighthouse-style quality metrics are directionally strong
- Accessibility issues are low severity and understood
- Key visitor journeys can be measured and improved
- The system can be updated safely without fragile manual steps

### Phase 4 - Growth enhancements

**Objectives**
- Expand the portfolio into a longer-term professional platform
- Add features that support ongoing publishing and experimentation

**Key outputs**
- Blog or journal capability
- Rich project filters or tags
- Additional storytelling modules, experiments, or interactive demos

**Acceptance criteria themes**
- New features do not dilute clarity of the core portfolio
- Maintenance burden stays proportional to the value gained
- The platform remains fast, professional, and easy to evolve

## Delivery recommendation

For engineering review, the recommended path is:
- approve a single design direction,
- establish the Angular, .NET BFF, and relational SQL foundation immediately,
- ship a narrow but real MVP on that stack,
- then layer in richer content operations and portfolio enhancements.

This keeps the portfolio achievable for a side project while avoiding a costly architecture pivot later.
