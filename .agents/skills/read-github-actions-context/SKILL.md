---
name: read-github-actions-context
description: >-
  Read predefined GitHub Actions environment variables with TedToolkit.CiContext.GitHubActions for repository, workflow, run, or runner logic.
---

# Read GitHub Actions Context

- Import `TedToolkit.GitHub` and use `GitHubActions.General.Detected` when the tool also runs locally.
- Read the provider-specific category, for example `GitHubActions.Repository.NameWithOwner` or `GitHubActions.Run.Id`. Keep optional and event-dependent values nullable.
- Use the raw reader for custom environment variables. Exercise the path with and without Actions metadata and keep secret values out of diagnostics.
