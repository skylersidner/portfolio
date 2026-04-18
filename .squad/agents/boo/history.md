# Project Context

- **Owner:** Skyler Sidner
- **Project:** developer portfolio site for showcasing personal projects, ideas, LinkedIn, and possibly a resume later
- **Stack:** HTML, CSS, JavaScript, with React or Angular as likely frontend options; C# and deployment support available if needed
- **Created:** 2026-04-16

## Learnings

- The architecture should support a polished initial portfolio without overcomplicating the first release.
- Recommendations should stay high-level and leave implementation to the coding roles.
- Produced deployment pipeline comparison (`planning/deployment-pipelines.md`) and hosting options guide (`planning/hosting-options.md`) for first production deployment planning.
- **Top platform recommendation:** Railway for first production deployment (push-to-deploy, managed Postgres, no Docker required); Fly.io as the long-term target once a Dockerfile exists.
- **Pipeline recommendation:** Platform-native PaaS (Railway) over container-based CI/CD for initial launch; container pipeline is the right long-term pattern but wrong starting point for a solo dev new to DevOps.
- **Subdomain strategy recommendation:** Single domain (`skylersidner.com`) with `test-` prefix for non-production environments. Do not register separate domain names per environment — cost and DNS overhead are not justified at personal portfolio scale. Wildcard cert covers all subdomains.