# Backend Models and API Planning

## Purpose

This document outlines the backend planning model for a portfolio application whose main job is to present side projects cleanly, support content management, and expose stable data to an Angular front end. This is a planning document only and does not prescribe implementation code.

## Product emphasis

The backend should prioritize:

1. project discovery and presentation
2. external launch routing and supporting project context
3. simple content administration for updates and featured ordering
4. contact handling and operational visibility
5. career history only as supporting evidence of credibility

---

## Core domain entities

### Project

Represents the primary portfolio artifact.

Key responsibilities:

- stores public-facing project content
- tracks publish state and featured placement
- references tags, links, assets, and related credibility signals
- supports both short card views and deeper case-study style views

Suggested attributes:

- identity: id, slug, title
- summaries: short_summary, full_overview, impact_summary
- classification: project_type, status, visibility
- display: is_featured, featured_rank, stack_summary
- external routing: launch_url, repo_url, primary_cta_label
- timing: started_on, completed_on, published_at
- audit: created_at, updated_at, created_by, updated_by

### Tag

Represents reusable project metadata for filtering and quick scanning.

Examples:

- Angular
- .NET
- Azure
- Accessibility
- Data Visualization
- Open Source

Suggested attributes:

- id, slug, name
- tag_type
- sort_order
- audit fields

### ProjectLink

Represents destination links associated with a project.

Typical use cases:

- live application
- GitHub repository
- case study write-up
- demo video
- package listing

Suggested attributes:

- id, project_id
- link_type, label, url
- is_primary, sort_order
- status and last_checked_at

### ProjectAsset

Represents media and downloadable assets for a project.

Typical use cases:

- hero image
- card thumbnail
- screenshots
- logo
- short demo clip

Suggested attributes:

- id, project_id
- asset_kind, storage_path
- alt_text, caption
- width, height, mime type
- is_primary, sort_order, status

### ResumeEntry

Represents work history items that support trust and credibility.

Important positioning:

- should not dominate homepage payloads
- should be available for resume-style sections and credibility callouts
- may be related back to projects through references or shared skills

Suggested attributes:

- id, slug
- organization_name, role_title
- summary, highlights
- started_on, ended_on
- status, sort_rank

### ProjectResumeReference

Represents supporting links between a project and relevant career history.

Purpose:

- reinforces credibility for project claims
- gives the front end optional context such as related industry or role experience
- keeps the product project-first while still grounding the work in professional experience

### ContactSubmission

Represents inbound contact and opportunity messages.

Suggested attributes:

- sender identity and message content
- topic or intent type
- related project reference if applicable
- review and response status
- spam and moderation signals

### AuditEvent

Represents backend-level change tracking for important admin actions and publishing flows.

Purpose:

- explains who changed what and when
- supports troubleshooting and content accountability
- improves observability during launch and maintenance

---

## Backend service and model groupings

### Public content services

These services support the visitor-facing Angular application.

- project catalog service
- project detail composition service
- featured projects service
- tag filtering service
- resume summary service
- contact submission intake service

### Admin and content-management services

These services support content maintenance.

- project draft management
- asset ordering and media metadata management
- featured ordering and homepage curation
- link health review
- resume credibility mapping
- moderation and spam handling for contact submissions
- publishing workflow and audit capture

### Cross-cutting support services

- validation service
- logging and telemetry service
- error translation layer
- authentication and authorization for admin routes
- background health checks for links and media readiness

---

## API contract and Angular composition planning

The backend should avoid returning raw database shapes, but it also should not overfit responses to individual screens. The preferred direction is a more RESTful BFF API made up of domain-scoped resources and operations.

The Angular application should then compose page-ready state in its store layer after calling those domain-scoped data-access services.

### Contract design principles

- keep endpoint groups aligned to business domains such as projects, tags, resume context, and contact
- expose stable resource models that can serve multiple screens
- avoid creating a separate backend model for every visual variation in the UI
- reserve server-side aggregation for security, workflow, and consistency concerns rather than purely presentational assembly
- let Angular stores combine, refine, and expose signals for component consumption

### Example domain response shapes

These are planning-level response contracts, not implementation commitments.

#### ProjectSummaryDto

Useful for directory views, featured sections, and lightweight launch cards.

Suggested fields:

- id or slug
- title
- short_summary
- status
- featured state
- primary thumbnail reference
- top tags
- primary launch link
- freshness metadata such as updated or completed date

#### ProjectDetailDto

Useful for detail routes and richer storytelling pages.

Suggested fields:

- core project content
- extended overview and outcomes
- ordered links
- ordered screenshots and hero media
- related tags
- optional related resume credibility items
- optional notes, lessons, or architecture highlights

#### ResumeSummaryDto

Useful for supporting credibility sections without overwhelming the product-first experience.

Suggested fields:

- relevant roles or engagements
- concise impact bullets
- date ranges and organization names
- optional mapping to related projects

#### ContactSubmissionRequestDto

Used by the public form.

Suggested fields:

- name
- email
- company or context
- topic
- message
- optional related project slug

#### ContactSubmissionResultDto

Used to confirm successful intake without exposing internal detail.

Suggested fields:

- success flag
- human-friendly confirmation message
- optional follow-up expectation guidance

#### Future AdminProjectEditDto

Reserved for a later protected admin area rather than the initial public release.

Suggested fields:

- editable project content fields
- tag assignments
- ordered link list
- ordered asset list
- publication and visibility values
- optimistic concurrency token such as version number

### Angular client composition guidance

The front end should be organized so that:

- data-access services call the BFF by domain
- Angular stores orchestrate and combine the returned data
- stores expose signals that components can consume directly and refine further when needed
- reusable abstract UI components handle common states and patterns
- domain-specific components remain free to tailor those shared building blocks to portfolio content

---

## Validation rules

Validation should exist at request boundaries and again at domain boundaries for important business rules.

### Project validation

- title is required and should be concise
- slug is required, unique, and URL-safe
- short_summary is required for any published project
- launch_url should be required when the project is intended to be externally launched
- featured_rank should only be set when is_featured is true
- published content should have at least one meaningful link or asset
- archived projects should not appear in public responses

### Tag validation

- tag names should be unique within their type where practical
- slugs should remain stable once published

### ProjectLink validation

- URL must be absolute and well-formed
- label should match the destination purpose clearly
- only one primary live link should exist per project unless intentionally overridden

### ProjectAsset validation

- alt_text should be required for all public images
- sort_order should be unique within a project and asset grouping where practical
- only ready assets should appear in public payloads

### Contact validation

- name, email, and message are required
- email must be syntactically valid
- message should enforce reasonable minimum and maximum lengths
- anti-spam checks should run before persistence or before notification fan-out

### Admin workflow validation

- draft to published transitions should require required public fields
- concurrent edits should be guarded with version or timestamp checks
- destructive actions should be auditable and permission-limited

---

## API and BFF endpoint groups

This plan assumes a backend-for-frontend style shape that serves the Angular client through RESTful, domain-scoped endpoints.

### Public portfolio endpoints

- projects collection and filtering
- featured projects feed
- project detail by slug
- tags and filter metadata
- career summary and credibility highlights
- contact submission intake

### Admin content endpoints

- create and update project drafts
- publish, unpublish, archive, and reorder projects
- manage tags and project associations
- manage project links and media assets
- update resume entries and project credibility references
- review contact submissions and moderation state

### Operational endpoints

- health and readiness
- telemetry or diagnostics summary for admins
- link integrity reports
- background processing or asset status visibility

The BFF layer should keep contracts practical and front-end-friendly, but it should stop short of becoming a collection of page-specific view endpoints. Angular should handle the final composition step in its store layer.

---

## Admin and content-management considerations

### Editing workflow

A lightweight admin experience should support:

- drafting new projects before publishing
- editing summaries and media without touching deployment code
- rearranging featured projects by simple ordered controls
- previewing how content appears across multiple design directions
- attaching supporting resume references only when they genuinely strengthen credibility

### Roles and permissions

The initial release should remain public-only with no active sign-in requirement. If an admin area is introduced later, plan for:

- editor role for content updates
- admin role for publishing, archive actions, and visibility changes
- read-only analytics or reviewer role if collaboration expands later

### Content review safeguards

- publish actions should create audit events
- significant ordering changes should be traceable
- broken external links should not silently remain primary calls to action

---

## Logging and observability plan

The portfolio is content-driven, but it still needs operational visibility.

### What to log

- content publish and archive events
- admin edits to featured ordering
- contact submission intake and moderation outcomes
- failed external link checks
- API errors by endpoint group
- high-latency requests for project detail composition

### Logging characteristics

- structured logs with correlation identifiers
- environment-aware verbosity
- no sensitive message content in logs beyond what is operationally necessary
- sanitized contact submission logging to protect personal data

### Metrics to monitor

- request volume by endpoint group
- response latency for listing and detail endpoints
- contact form submission success and failure rates
- count of published versus draft projects
- broken or inactive external links
- asset readiness backlog if media processing is introduced

### Tracing and diagnostics

If the stack grows beyond a simple app, add distributed tracing for:

- public request to BFF composition path
- admin publishing workflows
- outbound services such as email or storage

---

## Error-handling plan

The backend should return predictable, front-end-friendly responses.

Guidelines:

- validate early and return clear user-safe error messages
- separate user-facing errors from internal diagnostic detail
- map domain validation failures to consistent response shapes
- treat missing public content as not found rather than generic server failure
- record unexpected failures with sufficient context for debugging
- degrade gracefully when an external launch link or asset is temporarily unavailable

For the Angular front end, prefer consistent categories such as:

- validation issue
- not found
- unauthorized or forbidden for admin flows
- rate limited or blocked for abuse prevention
- unexpected server error

---

## Review-friendly implementation priorities

### Phase 1

- public project catalog and detail models
- tags, links, assets, and featured ordering support
- public contact submission intake

### Phase 2

- admin content management workflow
- draft and publish lifecycle enforcement
- audit logging and moderation support

### Phase 3

- link health checks
- richer diagnostics and dashboards
- optional analytics and content insights

This sequence supports a fast launch for a project-first portfolio while leaving room for stronger editorial and operational maturity later.
