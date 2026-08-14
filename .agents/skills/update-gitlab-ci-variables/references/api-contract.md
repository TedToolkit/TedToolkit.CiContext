# API contract and classification rules

## Authority

- Rendered reference: `https://docs.gitlab.com/ci/variables/predefined_variables/`
- Maintained source: `https://gitlab.com/gitlab-org/gitlab/-/raw/master/doc/ci/variables/predefined_variables.md`
- GitLab currently publishes no complete predefined-variable JSON or YAML manifest in `doc/ci/variables/`.

## Official relationships

GitLab defines Pre-pipeline, Pipeline, and Job-only phases. Merge request and Auto DevOps variables are Pre-pipeline. Integration variables are Job-only and retain their named integration subdivision. External pull request variables have contextual conditions but no declared phase. Deployment variable names are integration-defined rather than a fixed central list.

Do not present library-inferred domains as official GitLab categories.

## Canonical navigation

Use phase-first navigation:

```csharp
using TedToolkit.GitLab;

GitLabCi.PrePipeline.Commit.Sha
GitLabCi.PrePipeline.Project.Id
GitLabCi.PrePipeline.MergeRequest.Source.BranchName
GitLabCi.PrePipeline.AutoDevOps.ExplicitlyEnabled
GitLabCi.Pipeline.Environment.Name
GitLabCi.Pipeline.Job.Name
GitLabCi.Pipeline.Iid
GitLabCi.JobOnly.Job.Id
GitLabCi.JobOnly.Runner.Id
GitLabCi.JobOnly.Integrations.Harbor.Url
GitLabCi.ExternalPullRequest.Source.BranchName
```

General variables use focused library domains such as Commit, Project, Pipeline, Job, Runner, Server, Environment, Registry, Repository, Pages, Trigger, Kubernetes, Deployment, Release, User, Chat, Tracing, and ParallelExecution. Use `General` only for honest cross-cutting markers.

## Type decisions

Names are hints, not proof. `_ID`, `_URL`, and `_FILE` suffixes never override the official description.

- Numeric database identifiers and counters: `long?` or `int?` only when explicitly numeric.
- Opaque IDs, key IDs, issuer IDs, refs, SHAs, versions, architecture strings, and composite hosts: `string?` unless documentation states otherwise.
- Absolute web endpoints: `Uri?`; authenticated clone URLs are secrets.
- Local paths: `FilePath?` or `DirectoryPath?`.
- ISO 8601 instants: `DateTimeOffset?`.
- Durations with documented units: `TimeSpan?`.
- Closed documented sets: nullable enums whose exact official tokens are declared with `CiEnumValueAttribute` and read through the common cached enum parser.
- Documented JSON arrays and comma-separated collections: read-only string lists.
- Credentials, private keys, JSON credentials, access tokens, authenticated repository URLs, and token fragments documented as authentication-capable: `CiSecret<string>?`. Deliberately non-authenticating display fragments remain `string?`.

Use the public `CiEnvironment` readers for booleans, markers, numbers, timestamps, durations, URIs, paths, lists, and mapped enums. Do not create provider-local parsing helpers.

Every property reads the current process environment at access time. Missing values return null. Present malformed typed values throw `FormatException`. Exact static raw access remains available for diagnostics, custom, deployment, and future variables.

## Review checklist

- Every official name appears exactly once.
- Every terminal property directly carries one `CiVariableAttribute` for its exact name and one `GitLabCiVariableCategoryAttribute` for its category; every official phase class has availability metadata.
- Every parsed enum field has exactly one current official `CiEnumValueAttribute` mapping unless the official reference explicitly documents aliases.
- Every changed type has official evidence.
- Every variable has one canonical path.
- No captured environment or mutable library-owned environment source exists.
- README examples compile.
- Every target framework builds with zero diagnostics.
