# TedToolkit.CiContext

TedToolkit.CiContext is a family of .NET packages for typed, discoverable, live access to CI environment variables. It currently supports GitLab CI/CD and GitHub Actions while preserving each provider's own terminology and structure.

[![License: LGPL-3.0](https://img.shields.io/badge/License-LGPL--3.0-blue.svg)](COPYING.LESSER)

## What It Is For

CI systems expose job metadata as strings in the process environment. These packages turn the officially documented variables into nullable .NET properties with appropriate types such as numbers, timestamps, URIs, paths, lists, and enums.

- Values are read from the current process environment on every access.
- Missing variables return `null`, which is normal across different job types and execution phases.
- Malformed typed values throw `FormatException` without exposing the original value in the message.
- Raw access remains available for custom, future, and malformed variables.
- Sensitive values can use `CiSecret<T>`, which redacts ordinary string formatting and requires `Reveal()` for access.

The library does not call CI provider APIs, evaluate pipeline or workflow configuration, execute jobs, or normalize unlike provider concepts into a lowest-common-denominator interface.

## Choose a Package

| Package | Use it for | Guide |
| --- | --- | --- |
| `TedToolkit.CiContext.GitLab` | GitLab predefined CI/CD variables, organized by availability phase and domain | [GitLab guide](src/TedToolkit.CiContext.GitLab/README.md) |
| `TedToolkit.CiContext.GitHubActions` | GitHub Actions default environment variables, organized by provider-specific concepts | [GitHub Actions guide](src/TedToolkit.CiContext.GitHubActions/README.md) |
| `TedToolkit.CiContext` | Provider-neutral raw and typed environment readers, variable metadata, enum mappings, and `CiSecret<T>` | [Foundation guide](src/TedToolkit.CiContext/README.md) |

Provider packages are independently installable and depend only on the common foundation.

## 🚀 Quick Start

Install the package for your CI provider:

```shell
dotnet add package TedToolkit.CiContext.GitLab
```

```csharp
using TedToolkit.GitLab;

if (GitLabCi.PrePipeline.General.Detected is true)
{
    Console.WriteLine($"Project: {GitLabCi.PrePipeline.Project.Path}");
    Console.WriteLine($"Pipeline: {GitLabCi.JobOnly.Pipeline.Id}");
    Console.WriteLine($"Working directory: {GitLabCi.JobOnly.Project.Dir}");
}
```

For GitHub Actions:

```shell
dotnet add package TedToolkit.CiContext.GitHubActions
```

```csharp
using TedToolkit.GitHub;

if (GitHubActions.General.Detected is true)
{
    Console.WriteLine($"Repository: {GitHubActions.Repository.NameWithOwner}");
    Console.WriteLine($"Run: {GitHubActions.Run.Id}");
    Console.WriteLine($"Workspace: {GitHubActions.Repository.Workspace}");
}
```

No context object or initialization is required. Each terminal property reads its environment variable when accessed.

## Scope and Compatibility

The provider packages cover every variable explicitly named by their selected official reference. Provider-specific metadata preserves facts such as GitLab availability phases and documentation categories, while the common package supplies only reusable environment-reading contracts.

Supported target frameworks:

- `net6.0`
- `net7.0`
- `net8.0`
- `net9.0`
- `net10.0`
- `net472`
- `net48`
- `netstandard2.0`
- `netstandard2.1`

See the approved [product intent](docs/product/README.md) and [package-family architecture](docs/architecture/ci-context-package-family.md) for the complete product boundary and dependency rules.

## Development

### Prerequisites

- A .NET SDK capable of building the target frameworks
- Git with submodule support

Clone the repository and its build infrastructure:

```shell
git clone --recursive https://github.com/TedToolkit/TedToolkit.CiContext.git
cd TedToolkit.CiContext
```

For an existing clone:

```shell
git submodule update --init --recursive
```

### Build and Test

```shell
dotnet restore TedToolkit.CiContext.slnx
dotnet build TedToolkit.CiContext.slnx -c Release --no-restore
dotnet run --project tests/TedToolkit.CiContext.Tests/TedToolkit.CiContext.Tests.csproj -c Release -f net10.0 --no-restore
dotnet run --project tests/TedToolkit.CiContext.Tests/TedToolkit.CiContext.Tests.csproj -c Release -f net48 --no-restore
dotnet run --project tests/TedToolkit.CiContext.GitLab.Tests/TedToolkit.CiContext.GitLab.Tests.csproj -c Release --no-restore
dotnet run --project tests/TedToolkit.CiContext.GitHubActions.Tests/TedToolkit.CiContext.GitHubActions.Tests.csproj -c Release --no-restore
```

CI runs the multi-target core tests in a dedicated job with `-f net10.0`; the shared build pipeline runs the single-target provider tests. The build pipeline depends on the core tests and Native AOT checks passing.

The [GitLab benchmark project](benchmarks/TedToolkit.CiContext.GitLab.Benchmarks/README.md) measures environment-access and parsing trade-offs separately from correctness tests.

### Native AOT Verification

The .NET 8 and later package assets support Native AOT. Enum mappings use reflection once per enum type, with public-field preservation annotations flowing through the generic API and cache. This preserves `CiEnumValueAttribute` mappings for provider enums and enums defined by consumers without requiring a source generator.

CI publishes and runs the native smoke executable on Linux and Windows for .NET 8, 9, and 10. It rejects execution with a JIT runtime and verifies every provider enum type, consumer-defined enums, mapping aliases and errors, shared readers, and secret redaction. Native compiler and trimming warnings fail the publish step.

With the [Native AOT toolchain](https://learn.microsoft.com/dotnet/core/deploying/native-aot/#prerequisites) installed, run the Linux check locally:

```shell
dotnet publish tests/TedToolkit.CiContext.AotSmoke/TedToolkit.CiContext.AotSmoke.csproj -c Release -f net8.0 -r linux-x64 -o artifacts/aot
./artifacts/aot/TedToolkit.CiContext.AotSmoke
```

On Windows, select `win-x64` and run `artifacts/aot/TedToolkit.CiContext.AotSmoke.exe`. Repeat with `net9.0` or `net10.0` to verify those package assets.

## Maintaining Provider Coverage

The repository includes project-local Codex skills for synchronizing each provider API with its official variable reference:

- `.agents/skills/update-gitlab-ci-variables`
- `.agents/skills/update-github-actions-ci-variables`

## License

Licensed under [LGPL-3.0](COPYING.LESSER). See [COPYING](COPYING) and [COPYING.LESSER](COPYING.LESSER) for the full license text.
