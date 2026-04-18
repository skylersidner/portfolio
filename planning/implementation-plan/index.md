# Implementation Plan Package

This package translates the portfolio concept into an implementation-ready plan for a production site built with Angular, a .NET Back End for Front End, and a relational SQL database.

The main goal of the site is to act as a launch hub for side projects hosted on subdomains. Resume details, LinkedIn, and contact paths remain important, but they support the primary job of helping visitors discover and navigate to projects quickly.

## Contents

- [Architecture Overview](architecture-overview.md) — approved end-to-end structure, responsibilities, topology, and deployment shape.
- [Backend Models and API Planning](backend-models.md) — domain entities, DTO planning, endpoint groups, and BFF responsibilities.
- [Database Design Plan](database-design.md) — relational schema, lifecycle fields, indexing strategy, and rollout order.
- [Delivery Phases](delivery-phases.md) — phased implementation plan for shipping the approved full-stack product sensibly.
- [VS Code Run and Environment Plan](vscode-run-and-environments.md) — local workflow, environments, and operational planning for the Angular app, .NET BFF, and SQL database.

## Recommended Review Order

1. Start with [Architecture Overview](architecture-overview.md) to align on the approved platform shape and deployment model.
2. Continue to [Backend Models and API Planning](backend-models.md) to confirm what the .NET BFF is responsible for serving.
3. Review [Database Design Plan](database-design.md) to validate the relational data model and persistence strategy.
4. Then read [Delivery Phases](delivery-phases.md) to confirm how the full stack should be delivered in manageable increments.
5. Finish with [VS Code Run and Environment Plan](vscode-run-and-environments.md) to validate local workflow and environment assumptions.

## Intended Outcome

After reviewing this package, the next planning step should be clear:

- what the portfolio system is responsible for
- how the Angular front end and .NET BFF should interact
- what belongs in the SQL data model
- which technical choices should be decided before development begins
