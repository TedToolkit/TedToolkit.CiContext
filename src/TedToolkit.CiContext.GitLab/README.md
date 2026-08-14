# TedToolkit.CiContext.GitLab

`TedToolkit.CiContext.GitLab` provides strongly typed, phase-aware, on-demand access to GitLab CI/CD variables in the current process environment while keeping its public API in the concise `TedToolkit.GitLab` namespace.

## Install

```shell
dotnet add package TedToolkit.CiContext.GitLab
```

## Quick Start

```csharp
using TedToolkit.GitLab;

if (GitLabCi.PrePipeline.General.Detected is true)
{
    Console.WriteLine($"Project: {GitLabCi.PrePipeline.Project.Path}");
    Console.WriteLine($"Pipeline: {GitLabCi.JobOnly.Pipeline.Id}");
    Console.WriteLine($"Working directory: {GitLabCi.JobOnly.Project.Dir}");
}
```

No context needs to be created. `GitLabCi`, each availability phase, and every domain or official category are nested static classes. A terminal property reads its environment variable when accessed.

| GitLab variable | C# property | Type |
| --- | --- | --- |
| `CI_PROJECT_ID` | `GitLabCi.PrePipeline.Project.Id` | `long?` |
| `CI_COMMIT_TIMESTAMP` | `GitLabCi.PrePipeline.Commit.Timestamp` | `DateTimeOffset?` |
| `CI_JOB_TIMEOUT` | `GitLabCi.JobOnly.Job.Timeout` | `TimeSpan?` |
| `CI_PROJECT_URL` | `GitLabCi.PrePipeline.Project.Url` | `Uri?` |
| `CI_PROJECT_DIR` | `GitLabCi.JobOnly.Project.Dir` | `DirectoryPath?` |
| `CI_SERVER_TLS_CA_FILE` | `GitLabCi.Pipeline.Server.TlsCaFile` | `FilePath?` |
| `CI_PIPELINE_SOURCE` | `GitLabCi.PrePipeline.Pipeline.Source` | `GitLabPipelineSource?` |
| `CI_PIPELINE_IID` | `GitLabCi.Pipeline.Iid` | `long?` |
| `CI_RUNNER_TAGS` | `GitLabCi.JobOnly.Runner.Tags` | `IReadOnlyList<string>?` |
| `CI_JOB_TOKEN` | `GitLabCi.JobOnly.Job.Token` | `CiSecret<string>?` |
| `CI_RUNNER_SHORT_TOKEN` | `GitLabCi.JobOnly.Runner.ShortToken` | `CiSecret<string>?` |

Official relationships remain visible in paths such as `GitLabCi.PrePipeline.MergeRequest.Source.BranchName`, `GitLabCi.ExternalPullRequest.Target.BranchSha`, and `GitLabCi.JobOnly.Integrations.Harbor.Url`.

## Live Process Values

Each access reads the current process environment. No full-environment snapshot, raw dictionary, or navigation object is allocated:

```csharp
string? before = GitLabCi.PrePipeline.Commit.Sha;

Environment.SetEnvironmentVariable("CI_COMMIT_SHA", "updated");

string? after = GitLabCi.PrePipeline.Commit.Sha; // "updated"
```

GitLab Runner supplies stable variables to a job process. Tests or tools that mutate process environment variables must run those mutations serially and restore previous values afterward.

## Missing And Malformed Values

A variable that GitLab did not provide returns `null`, which is normal because availability depends on the pipeline type and phase.

When a present value does not match its documented type, the typed property throws `FormatException`. The current text remains available for diagnosis:

```csharp
string? rawProjectId = CiEnvironment.GetRawValue("CI_PROJECT_ID");

// Accessing GitLabCi.PrePipeline.Project.Id throws FormatException
// when CI_PROJECT_ID is present but is not a documented numeric ID.
```

## Sensitive Values

Credentials and authentication-capable tokens use the shared `CiSecret<string>`. `ToString()` shows only the final four characters for generated values of at least 12 characters and fully redacts shorter values; call `Reveal()` only where the credential is required:

```csharp
string? token = GitLabCi.JobOnly.Job.Token?.Reveal();
```

`CI_TRIGGER_SHORT_TOKEN` remains a plain string because GitLab defines it as a four-character display fragment rather than an authentication value.

`CiEnvironment.GetRawValue(...)` can return sensitive original values. Do not log its result without applying the appropriate secret-handling policy.

## Custom And Future Variables

The typed surface covers every explicitly named variable in the current [official GitLab predefined variable documentation](https://docs.gitlab.com/ci/variables/predefined_variables/). Project, group, instance, deployment-specific, and newly introduced variables remain available without a library update:

```csharp
string? region = CiEnvironment.GetRawValue("DEPLOY_REGION");
```

Use reflection over static terminal properties carrying `CiVariableAttribute` when tooling needs the exact environment name, and read `GitLabCiVariableCategoryAttribute` for the official documentation category. Read `GitLabCiVariableAvailabilityAttribute` from `PrePipeline`, `Pipeline`, or `JobOnly` when tooling needs the official phase. `ExternalPullRequest` is a direct root category without availability metadata because GitLab does not assign those variables to one of the three phases.

## Target Frameworks

`net6.0`, `net7.0`, `net8.0`, `net9.0`, `net10.0`, `net472`, `net48`, `netstandard2.0`, and `netstandard2.1`.

## Related Documentation

- [Repository overview](../../README.md)
- [Product intent](../../docs/product/README.md)
- [Package-family architecture](../../docs/architecture/ci-context-package-family.md)
- [Current architecture](../../docs/architecture/gitlab-ci-variable-api.md)

## License

Licensed under LGPL-3.0.
