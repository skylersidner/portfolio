# Database Design Plan

## Purpose

This plan describes a relational content model for a developer portfolio where side projects are the primary product and career history supports credibility. The database should make it easy to publish, reorder, feature, and update projects without changing front-end code.

**Chosen relational platform:** PostgreSQL for local development and production hosting.

## Design goals

- keep projects as the central content object
- support multiple portfolio layouts without changing the source data model
- allow a lightweight admin experience for content updates
- preserve auditability for edits and publishing decisions
- handle external project launches, screenshots, tags, links, and contact submissions cleanly
- stay normalized by default, while allowing targeted denormalization for homepage performance and featured sections

---

## Recommended relational schema

### 1. projects

Stores the core project entries shown across the portfolio.

| Column | Type intent | Notes |
| --- | --- | --- |
| id | UUID or bigint | Primary key |
| slug | unique string | Stable public identifier for routing |
| title | string | Display title |
| short_summary | string | Card-sized summary |
| full_overview | text | Detailed project description |
| problem_statement | text | Optional case-study framing |
| solution_summary | text | Optional solution narrative |
| impact_summary | text | Outcomes, metrics, or lessons |
| role_label | string | Example: solo developer, lead engineer |
| project_type | enum | personal, freelance, open-source, internal-demo |
| status | enum | draft, scheduled, published, archived |
| visibility | enum | public, unlisted, private-admin |
| launch_url | string | External destination for the project |
| repo_url | string | Source repository if public |
| primary_cta_label | string | Example: View Live Project |
| featured_rank | integer nullable | Lower value means higher prominence |
| is_featured | boolean | Fast homepage filter |
| started_on | date nullable | When work began |
| completed_on | date nullable | When it shipped or paused |
| last_substantive_update_on | date nullable | Signals freshness |
| stack_summary | string | Optional display shortcut for UI |
| content_version | integer | Helps optimistic admin editing |
| created_at | timestamp | Audit field |
| updated_at | timestamp | Audit field |
| published_at | timestamp nullable | Publish lifecycle |
| archived_at | timestamp nullable | Archive lifecycle |
| created_by | user reference or string | Audit trail |
| updated_by | user reference or string | Audit trail |

Recommended indexes:

- unique index on slug
- index on status and visibility
- index on is_featured and featured_rank
- index on published_at descending
- optional index on project_type

### 2. project_tags

Master tag list for technologies, themes, and categories.

| Column | Type intent | Notes |
| --- | --- | --- |
| id | UUID or bigint | Primary key |
| slug | unique string | Stable identifier |
| name | string | Display name |
| tag_type | enum | technology, domain, platform, practice |
| sort_order | integer | Optional admin ordering |
| created_at | timestamp | Audit |
| updated_at | timestamp | Audit |

Indexes:

- unique index on slug
- index on tag_type and sort_order

### 3. project_tag_map

Many-to-many relationship between projects and tags.

| Column | Type intent | Notes |
| --- | --- | --- |
| project_id | foreign key | References projects |
| tag_id | foreign key | References project_tags |
| emphasis_rank | integer nullable | Lets UI prefer a few primary tags |
| created_at | timestamp | Audit |

Keys and indexes:

- composite primary key on project_id and tag_id
- index on tag_id and project_id

### 4. project_links

Stores all link destinations related to a project.

| Column | Type intent | Notes |
| --- | --- | --- |
| id | UUID or bigint | Primary key |
| project_id | foreign key | References projects |
| link_type | enum | live, repo, case-study, demo-video, write-up, package, app-store |
| label | string | User-facing text |
| url | string | Absolute external or internal URL |
| is_primary | boolean | One main link per experience type if desired |
| sort_order | integer | Controls presentation order |
| status | enum | active, broken, retired |
| last_checked_at | timestamp nullable | Optional link health monitoring |
| created_at | timestamp | Audit |
| updated_at | timestamp | Audit |

Indexes:

- index on project_id and sort_order
- index on link_type
- index on status

### 5. project_assets

Holds screenshots, thumbnails, videos, logos, and downloadable assets.

| Column | Type intent | Notes |
| --- | --- | --- |
| id | UUID or bigint | Primary key |
| project_id | foreign key | References projects |
| asset_kind | enum | hero-image, thumbnail, screenshot, logo, video, attachment |
| storage_path | string | Blob path, CDN path, or media URL |
| alt_text | string | Accessibility text |
| caption | string nullable | Optional context |
| width_px | integer nullable | Useful for rendering hints |
| height_px | integer nullable | Useful for rendering hints |
| mime_type | string nullable | Media typing |
| file_size_bytes | bigint nullable | Asset management |
| sort_order | integer | Gallery ordering |
| is_primary | boolean | Hero or card asset |
| status | enum | processing, ready, hidden, retired |
| created_at | timestamp | Audit |
| updated_at | timestamp | Audit |

Indexes:

- index on project_id and asset_kind
- index on project_id and sort_order
- index on is_primary

### 6. resume_entries

Supporting career history, used to reinforce credibility without overshadowing projects.

| Column | Type intent | Notes |
| --- | --- | --- |
| id | UUID or bigint | Primary key |
| slug | unique string | Stable admin reference |
| organization_name | string | Employer or client |
| role_title | string | Position name |
| summary | text | Short experience summary |
| highlights | text or structured JSON | Key impact points |
| started_on | date | Employment start |
| ended_on | date nullable | Null when current |
| employment_type | enum | full-time, contract, freelance, internship |
| sort_rank | integer | Resume ordering |
| status | enum | draft, published, archived |
| created_at | timestamp | Audit |
| updated_at | timestamp | Audit |

Indexes:

- index on status and sort_rank
- index on started_on descending

### 7. project_resume_reference

Links projects to relevant career credibility points.

| Column | Type intent | Notes |
| --- | --- | --- |
| project_id | foreign key | References projects |
| resume_entry_id | foreign key | References resume_entries |
| relationship_note | string nullable | Example: similar domain or skill proof |
| sort_order | integer | Prioritize related experiences |
| created_at | timestamp | Audit |

Keys and indexes:

- composite primary key on project_id and resume_entry_id
- index on resume_entry_id

### 8. contact_submissions

Captures inbound interest from the site.

| Column | Type intent | Notes |
| --- | --- | --- |
| id | UUID or bigint | Primary key |
| name | string | Sender name |
| email | string | Contact email |
| company | string nullable | Optional organization |
| message | text | Inquiry content |
| topic | enum | general, project, freelance, hiring, speaking |
| source_page | string nullable | Page or route where submitted |
| related_project_id | foreign key nullable | Optional project context |
| status | enum | new, reviewed, replied, spam, archived |
| spam_score | numeric nullable | Anti-abuse support |
| submitted_at | timestamp | Submission time |
| responded_at | timestamp nullable | Follow-up time |
| created_at | timestamp | Audit |
| updated_at | timestamp | Audit |

Indexes:

- index on status and submitted_at
- index on email
- index on related_project_id

### 9. content_audit_log

Append-only audit history for admin and publishing actions.

| Column | Type intent | Notes |
| --- | --- | --- |
| id | UUID or bigint | Primary key |
| entity_type | string | Example: project, asset, resume-entry |
| entity_id | string | Identifier of changed row |
| action | enum | created, updated, published, archived, reordered, deleted |
| change_summary | text | Human-readable change note |
| actor | string | User or service identity |
| occurred_at | timestamp | Event time |
| correlation_id | string nullable | Traceability across services |

Indexes:

- index on entity_type and entity_id
- index on occurred_at descending
- index on actor

---

## Relationship summary

- one project can have many tags through project_tag_map
- one project can have many links
- one project can have many assets
- one project can reference many resume entries through project_resume_reference
- one contact submission may optionally point to one project
- all major content entities should write to content_audit_log when changed

This keeps side projects at the center while still letting the portfolio show supporting professional context where useful.

---

## Lifecycle and status guidance

### Content lifecycle fields

For project-like content, prefer a small shared lifecycle model:

- draft: editable, not visible publicly
- scheduled: approved and queued for release
- published: visible in production
- archived: removed from active display but retained for history

### Operational status fields

For assets and links, maintain a separate operational status so publishing and health do not get mixed:

- active or ready for healthy content
- processing while uploads or transformations are running
- broken or hidden when public rendering should be suppressed
- retired when intentionally preserved but not shown

This separation reduces confusion in admin tools and reporting.

---

## Featured ordering strategy

Featured projects should use both:

- is_featured for a simple homepage filter
- featured_rank for precise visual ordering

This is a small, intentional denormalization because the portfolio front end will likely request a featured list often and should avoid expensive joins or complex ranking logic at runtime.

---

## Asset storage guidance

Store media metadata in the relational database, but keep large files in object storage or a CDN-backed media store. The database should store:

- canonical storage path
- media kind
- display order
- accessibility text
- readiness state

This approach avoids bloating the database while preserving queryable presentation metadata.

---

## Normalization guidance

### Keep normalized

Normalize the following:

- tags through a shared tag table and mapping table
- links as a separate child table per project
- assets as a separate child table per project
- resume history as its own supporting entity set
- audit events in an append-only audit table

This improves maintainability, avoids duplicate values, and supports future filtering and content management.

### Acceptable denormalization

Denormalization is acceptable in a few targeted areas:

- stack_summary on projects for quick card rendering
- is_featured and featured_rank on projects for homepage performance
- cached thumbnail or hero asset pointer on projects if the UI frequently needs the first media item
- precomputed display labels for recruiter-facing summaries if those are requested often

The rule should be: normalize for authoring and correctness, denormalize only when it clearly simplifies read performance or front-end composition.

---

## Suggested rollout order

1. projects
2. project_tags and project_tag_map
3. project_links
4. project_assets
5. resume_entries and project_resume_reference
6. contact_submissions
7. content_audit_log

This order supports an early project-first launch and allows supporting credibility and contact workflows to be added cleanly afterward.
