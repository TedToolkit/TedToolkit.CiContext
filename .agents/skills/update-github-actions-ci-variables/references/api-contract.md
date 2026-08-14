# API contract and classification rules

## Authority

- Rendered English reference: `https://docs.github.com/en/actions/reference/workflows-and-actions/variables`
- Maintained source: `https://raw.githubusercontent.com/github/docs/main/content/actions/reference/workflows-and-actions/variables.md`
- Source repository view: `https://github.com/github/docs/blob/main/content/actions/reference/workflows-and-actions/variables.md`

The rendered reference is the public coverage authority because GitHub can supply rendered content that is not yet present in the maintained Markdown file. The Markdown remains useful for deterministic extraction and independent drift detection.

As retrieved on 2026-08-14, both references name the same 46 default environment variables. Treat a changed count or difference as upstream drift requiring review.

## Canonical navigation

Use the static `TedToolkit.GitHub.GitHubActions` root and focused domains:

```csharp
using TedToolkit.GitHub;

GitHubActions.General.Detected
GitHubActions.Action.Name
GitHubActions.Actor.Id
GitHubActions.Event.PayloadPath
GitHubActions.Ref.Type
GitHubActions.Repository.Workspace
GitHubActions.Run.Retention
GitHubActions.Server.ApiUrl
GitHubActions.WorkflowCommands.StepSummaryFile
GitHubActions.Runner.OperatingSystem
```

The current domains are `General`, `Action`, `Actor`, `Event`, `Job`, `Ref`, `Repository`, `Run`, `Server`, `Workflow`, `WorkflowCommands`, and `Runner`.

## Type decisions

Names are hints, not proof. `_ID`, `_URL`, and path-like suffixes never override the official description.

- Documented numeric identifiers and counters: `long?` or `int?` as supported by the published range.
- Opaque IDs, names, refs, SHAs, and repository coordinates: `string?` unless GitHub states otherwise.
- Absolute service endpoints: `Uri?`.
- Local files: `FilePath?`; local directories: `DirectoryPath?`.
- Durations with documented units: `TimeSpan?`.
- Closed documented sets: nullable enums whose exact official tokens are declared with `CiEnumValueAttribute` and read through the common cached enum parser.
- Documented booleans such as `CI` and `GITHUB_ACTIONS`: `bool?`, read through the common Boolean parser.
- Exact presence markers such as `RUNNER_DEBUG`: `bool?`, recognizing only the documented marker.
- Sensitive values: `CiSecret<T>?` only when GitHub documents the environment value as sensitive and the variable is within scope.

Use the public `CiEnvironment` readers for booleans, markers, numbers, durations, URIs, paths, lists, and mapped enums. Do not create provider-local parsing helpers.

Every property reads the current process environment at access time through `CiEnvironment`. Missing values return null. Present malformed typed values throw `FormatException` without including the original value. `CiEnvironment.GetRawValue` remains available for custom, future, and malformed variables; do not add a provider-root forwarding method.

## Scope boundary

Cover the official default environment-variable table only. Exclude contexts available solely through expressions, automatically created `GITHUB_TOKEN` context access, event-payload models, workflow syntax evaluation, environment-file writes, secrets retrieval, and GitHub API access.

## Review checklist

- Every rendered official name appears exactly once.
- The maintained-source set and its difference from the rendered reference are explicit.
- Every terminal property directly carries one `CiVariableAttribute` with its exact name.
- Every parsed enum field has exactly one current official `CiEnumValueAttribute` mapping unless the official reference explicitly documents aliases.
- Every changed type has official evidence.
- Every variable has one canonical path.
- No captured environment or mutable library-owned environment source exists.
- README examples compile.
- Every target framework builds with zero diagnostics.
