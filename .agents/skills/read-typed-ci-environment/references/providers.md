# Provider accessors

## Read GitHub Actions Context

- Import `TedToolkit.GitHub` and use `GitHubActions.General.Detected` when the tool also runs locally.
- Read the provider-specific category, for example `GitHubActions.Repository.NameWithOwner` or `GitHubActions.Run.Id`. Keep optional and event-dependent values nullable.
- Use the raw reader for custom environment variables. Exercise the path with and without Actions metadata and keep secret values out of diagnostics.

```csharp
using TedToolkit.GitHub;

if (GitHubActions.General.Detected is true)
{
    Console.WriteLine($"Repository: {GitHubActions.Repository.NameWithOwner}");
    Console.WriteLine($"Run: {GitHubActions.Run.Id}");
    Console.WriteLine($"Workspace: {GitHubActions.Repository.Workspace}");
}
```

## Read GitLab CI Context

- Import `TedToolkit.GitLab` and check `GitLabCi.PrePipeline.General.Detected` when the same tool can run outside GitLab.
- Select `PrePipeline`, `Pipeline`, or `JobOnly` according to the variable’s GitLab availability. For example, read `GitLabCi.PrePipeline.Project.Path` for project identity and `GitLabCi.JobOnly.Pipeline.Id` for the job’s pipeline.
- Handle missing values normally across job types. Use raw access only for a custom or unsupported variable, and keep secret values out of diagnostics.

```csharp
using TedToolkit.GitLab;

if (GitLabCi.PrePipeline.General.Detected is true)
{
    Console.WriteLine($"Project: {GitLabCi.PrePipeline.Project.Path}");
    Console.WriteLine($"Pipeline: {GitLabCi.JobOnly.Pipeline.Id}");
    Console.WriteLine($"Working directory: {GitLabCi.JobOnly.Project.Dir}");
}
```

Provider names and nullable values reflect the job environment. Choose the provider-specific accessor rather than equating GitLab and Actions categories.
