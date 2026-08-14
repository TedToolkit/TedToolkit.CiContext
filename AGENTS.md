# User First Principles

## 1. Don't assume I know exactly what I want

When motivation or objectives are unclear, ambiguous, or contradictory, stop and ask before proceeding.

## 2. Challenge inefficient paths

When the objective is clear but the proposed path is inefficient or incorrect, explain the issue and recommend a better approach before changing code.

## 3. Fix root causes, not symptoms

Identify and address underlying causes. Every material decision must have a defensible reason.

## 4. Deliver only essentials

Keep code, documentation, and reports focused on information that affects the outcome or next decision.

# Project Instructions

## Language Policy

This is an English-only project.

- All code comments must be written in English.
- All documentation must be written in English.
- All README files must be written in English.
- Do not add Chinese comments, Chinese documentation, or mixed-language project text unless the user explicitly asks for it.

## GitLab CI Context API Design

- `src/TedToolkit.CiContext.GitLab` is the single public package boundary for typed GitLab CI/CD environment context.
- Tests may reference the production project; production code must not reference test projects.
- Cover every variable explicitly named by GitLab's official predefined CI/CD variables reference, including merge request, external pull request, Auto DevOps, and integration variables.
- Preserve raw values for custom, future, and malformed variables; do not implement GitLab API access or `.gitlab-ci.yml` evaluation.
- Treat missing variables as normal because GitLab exposes variables at pre-pipeline, pipeline, and job-only phases.
- Parse values with invariant culture and never reveal secret values through implicit formatting or diagnostics.
- Keep each target-framework guard scoped to the smallest member or statement block that requires it.

## Test Execution

- Use `dotnet run --project tests/TedToolkit.CiContext.GitLab.Tests/TedToolkit.CiContext.GitLab.Tests.csproj -c Release` to execute tests.
- Do not use `dotnet test` unless the user explicitly asks for it.

## Project Skills

- Use `.agents/skills/update-gitlab-ci-variables/SKILL.md` whenever synchronizing or reviewing the API against GitLab's official predefined CI/CD variable Markdown.
