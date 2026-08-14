# TedToolkit.CiContext.GitHubActions

`TedToolkit.CiContext.GitHubActions` provides strongly typed, on-demand access to GitHub Actions default environment variables in the current process while keeping its public API in the concise `TedToolkit.GitHub` namespace.

## Install

```shell
dotnet add package TedToolkit.CiContext.GitHubActions
```

## Quick Start

```csharp
using TedToolkit.GitHub;

if (GitHubActions.General.Detected is true)
{
    Console.WriteLine($"Repository: {GitHubActions.Repository.NameWithOwner}");
    Console.WriteLine($"Run: {GitHubActions.Run.Id}");
    Console.WriteLine($"Workspace: {GitHubActions.Repository.Workspace}");
}
```

No context needs to be created. The root and its semantic groups are nested static classes, and every terminal property reads its environment variable when accessed.

| GitHub variable | C# property | Type |
| --- | --- | --- |
| `GITHUB_ACTOR_ID` | `GitHubActions.Actor.Id` | `long?` |
| `GITHUB_EVENT_PATH` | `GitHubActions.Event.PayloadPath` | `FilePath?` |
| `GITHUB_REF_PROTECTED` | `GitHubActions.Ref.Protected` | `bool?` |
| `GITHUB_REF_TYPE` | `GitHubActions.Ref.Type` | `GitHubActionsRefType?` |
| `GITHUB_RETENTION_DAYS` | `GitHubActions.Run.Retention` | `TimeSpan?` |
| `GITHUB_API_URL` | `GitHubActions.Server.ApiUrl` | `Uri?` |
| `RUNNER_OS` | `GitHubActions.Runner.OperatingSystem` | `GitHubActionsRunnerOperatingSystem?` |
| `GITHUB_STEP_SUMMARY` | `GitHubActions.WorkflowCommands.StepSummaryFile` | `FilePath?` |

## Missing And Malformed Values

An unavailable variable returns `null`. A present value that does not match its documented type throws `FormatException` without including the original value:

```csharp
using TedToolkit.Ci;

string? rawRunId = CiEnvironment.GetRawValue("GITHUB_RUN_ID");
```

The common raw accessor also supports custom and newly introduced variables. Do not log its result unless the variable's sensitivity is known.

## Scope

The typed surface covers the 46 names in GitHub's current [rendered default-variable reference](https://docs.github.com/en/actions/reference/workflows-and-actions/variables). It does not model expression contexts, retrieve secrets, expose `GITHUB_TOKEN`, parse event payloads, evaluate workflows, or write workflow-command files.

The maintained [github/docs Markdown](https://github.com/github/docs/blob/main/content/actions/reference/workflows-and-actions/variables.md) contains the same 46 names as of 2026-08-14. Synchronization snapshots both sources independently so any future difference is executable drift evidence.

Use reflection over terminal properties carrying the shared `CiVariableAttribute` when tooling needs exact names. Closed documented string sets declare their exact tokens with `CiEnumValueAttribute`.

## Target Frameworks

`net6.0`, `net7.0`, `net8.0`, `net9.0`, `net10.0`, `net472`, `net48`, `netstandard2.0`, and `netstandard2.1`.

## Related Documentation

- [Repository overview](../../README.md)
- [GitHub Actions variable API architecture](../../docs/architecture/github-actions-variable-api.md)

## License

Licensed under LGPL-3.0.
