# GitHub Actions variable API architecture

- Status: Active
- Owner: TedToolkit maintainers
- Scope and system boundary: The public .NET API that maps officially documented GitHub Actions default runner environment variables to typed live views.
- Applicable product intent: `docs/product/README.md` (approved locally; no initial commit SHA exists)
- Governing principles: None
- Related ADRs: `docs/adr/ADR-003-use-provider-packages-over-a-ci-context-foundation.md`; `docs/adr/ADR-005-share-ci-value-parsing.md`; `docs/adr/ADR-008-use-policy-free-common-environment-readers.md`
- Last approved revision: Approved locally on 2026-08-13; no initial commit SHA exists

## 📝 Clarification and decision log

| ID | Question and why it mattered | Recommended answer | User decision and source | Affected sections | Status |
| --- | --- | --- | --- | --- | --- |
| AD-GH-01 | Whether the API covers default environment variables or all GitHub expression contexts determines the system boundary. | Cover only the official default environment-variable table. | Approved by the repository owner on 2026-08-13. | Current architecture; constraints | Resolved |
| AD-GH-02 | Whether the API copies GitLab's phase hierarchy determines semantic accuracy. | Use GitHub-specific semantic groups with one canonical path per variable. | Approved by the repository owner on 2026-08-13. | Current architecture | Resolved |
| AD-GH-03 | Whether the GitHub root should repeat provider-neutral raw access determines its public surface. | Keep the root focused on typed GitHub properties and use `CiEnvironment.GetRawValue` for raw values. | Approved by the repository owner on 2026-08-13. | Current architecture; constraints | Resolved |

## Current architecture

The public root is the static `TedToolkit.GitHub.GitHubActions` type. Its nested static classes group the official variables by their GitHub Actions meaning: `General`, `Action`, `Actor`, `Event`, `Job`, `Ref`, `Repository`, `Run`, `Server`, `Workflow`, `WorkflowCommands`, and `Runner`.

Every terminal property carries the provider-neutral `CiVariableAttribute` directly with the exact environment name and reads the current process environment on access. Closed documented values declare exact `CiEnumValueAttribute` mappings and use the common cached enum reader. GitHub publishes no availability phases or categories for the default-variable table, so the API does not invent official phase or category metadata.

The rendered reference and maintained `github/docs` Markdown each name the same 46 variables as of 2026-08-14. The rendered English reference remains the coverage authority, while source Markdown provides an independent extraction and drift signal.

## Constraints for change design

- Cover every name in the current rendered default-environment-variable table exactly once.
- Keep GitHub expression contexts distinct from runner environment variables.
- Preserve documented URLs, paths, booleans, numeric values, durations, and closed value sets with appropriate nullable types; keep insufficiently specified values as strings.
- Treat `RUNNER_DEBUG` as a presence marker: absent is `null`, documented `1` is `true`, and another present value is malformed.
- Expose workflow-command and event payload locations only as paths; do not read or write their contents.
- Do not expose `GITHUB_TOKEN` as a predefined environment property because GitHub documents it in the secrets context.
- Unknown, custom, future, and source-version-specific variables remain available through `CiEnvironment.GetRawValue` in the common package.
- Upstream synchronization must stop on unrecognized table structure, duplicates, or disagreement between official rendered and source name sets.

## Decision links and exceptions

ADR-003 owns package placement and dependency direction. ADR-005 owns shared variable metadata and stable parsing. ADR-008 keeps raw access and the common Boolean-token union on `CiEnvironment`. The navigation groups are provider-specific library design and do not claim official GitHub category status.

## Review triggers

- GitHub publishes a machine-readable default-variable schema.
- The official reference adds availability or category metadata.
- A documented variable cannot be represented by one canonical path or safe nullable type.
- Consumers require expression contexts, event payload models, or workflow-command operations.
