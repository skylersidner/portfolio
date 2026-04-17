# Developer Portfolio

A professional portfolio repository for showcasing work, technical skills, project highlights, and personal brand in a clear, modern format.

## Overview

This project is intended to serve as a central place for presenting portfolio content, selected accomplishments, and supporting materials for potential collaborators, employers, or clients.

## Purpose

This repository is designed to:
- present portfolio work in a polished and organized way
- highlight technical strengths and project experience
- provide a simple starting point for future portfolio development and deployment

## Stack and tooling

This project currently uses:

- Angular for the front-end application
- .NET 10 for the backend BFF/API
- PostgreSQL for local and future hosted relational data
- Docker for running PostgreSQL locally
- VS Code for local editing, debugging, and launch profiles
- NVM with a pinned Node version in `.nvmrc`
- npm for front-end package management

## Prerequisites for a new developer

Install these tools before getting started:

- Git
- [NVM](https://github.com/nvm-sh/nvm) (or [NVM for Windows](https://github.com/coreybutler/nvm-windows))
- Node 24 and npm
- [.NET 10 SDK](https://dotnet.microsoft.com/en-us/download/dotnet/10.0)
- [Docker Desktop](https://docs.docker.com/get-started/get-docker/)
- [VS Code](https://code.visualstudio.com/download)
- Optional: [pgAdmin](https://www.pgadmin.org/download/) or DBeaver for inspecting PostgreSQL

## First-time setup

From a fresh clone of the repository:

1. Clone the repo.
2. Open the repository root in VS Code.
3. Activate the pinned Node version from the repository root:
   - `nvm use`
   - If your Windows shell does not pick up the change immediately, open a fresh terminal in the repo after this step.
4. Install front-end dependencies:
   - `cd frontend`
   - `npm install`
5. Return to the repository root.
6. Start Docker Desktop if it is not already running.
7. Start the local PostgreSQL container:
   - `docker compose up -d postgres`
8. Start the backend API:
   - `dotnet run --project backend/Portfolio.Api --launch-profile local`
9. In a separate terminal, start the Angular UI:
   - `nvm use`
   - `cd frontend`
   - `npm start`

## Local URLs

Once everything is running, use:

- Front-end UI: `http://localhost:4200`
- API base: `https://localhost:7190/api`
- API metadata: `https://localhost:7190/api/meta`
- Health check: `https://localhost:7190/healthz`
- PostgreSQL: `localhost:5432`

## Local database configuration

Default local PostgreSQL settings:

- Host: `localhost`
- Port: `5432`
- Database: `portfolio_dev`
- Username: `postgres`
- Password: `postgres`

The backend creates its initial schema on startup and seeds starter portfolio content automatically.

## VS Code debugging

The workspace includes launch profiles for:

- a backend-only local API run
- a front-end watch mode for UI development
- a full-stack run profile for local development

Use the VS Code Run and Debug panel to start whichever workflow matches the task you are working on.

## Day-to-day development notes

- Run `nvm use` from the repository root before front-end work so the pinned Node version is active.
- Use the Angular watch flow when working on UI and styling.
- Use the backend-only profile when working on API, persistence, or data contracts.
- Docker should remain running when working with the PostgreSQL-backed API.
- If you need to inspect data directly, connect to the Postgres container with pgAdmin.

## Local backend database

To run the backend against local PostgreSQL:

1. Start Docker, then run `docker compose up -d postgres` from the repository root.
2. Start the API with `dotnet run --project backend/Portfolio.Api --launch-profile local`.
3. The backend creates its initial schema on startup and seeds the starter portfolio content automatically.

## Squad

This project makes use of [Squad for Copilot](https://github.com/bradygaster/squad) by Brady Gaster.

The project team includes:

- **Mario** — Lead
- **Luigi** — Frontend Developer
- **Bowser** — Backend Developer / DevOps
- **Rosalina** — Designer
- **Toad** — QA Tester
- **Peach** — Product Owner / Business Analyst
- **Boo** — Architect
- **Koopa** — Retrospective Analyst
- **Scribe** — Session Logger
- **Ralph** — Work Monitor
