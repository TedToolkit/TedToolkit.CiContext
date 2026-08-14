---
name: update-github-actions-ci-variables
description: Synchronize TedToolkit.CiContext.GitHubActions with GitHub's current official default environment-variable reference. Use when adding, removing, reviewing, regrouping, or retyping GitHub Actions variable APIs; when GitHub changes its variables documentation; or when validating exact coverage, metadata, and source drift.
---

# Update GitHub Actions CI Variables

Update the typed API from GitHub's official documentation while treating type and domain classification as semantic review, not text generation.

## Establish the source and repository contract

1. Read repository guidance, product intent, the package-family architecture, and the GitHub Actions variable API architecture.
2. Read [api-contract.md](references/api-contract.md) completely before changing the API.
3. Retrieve both the rendered English reference and the maintained Markdown listed in the contract.
4. Use only official GitHub documentation and repository sources to resolve semantics. Follow official links when a description is ambiguous.
5. Treat the rendered reference as the public coverage authority. Treat the maintained Markdown as a structured extraction aid and an independent drift signal.

## Build the change set

Extract exact names, descriptions, table membership, and linked contracts from both official sources. Mechanical extraction is allowed; semantic classification is not delegated to a script.

Compare each source independently against terminal properties carrying `CiVariableAttribute`. Report added, removed, renamed, description-changed, and source-only variables. Reject duplicate names and properties. Do not accept count-only coverage.

If either source changes structure, or the sources disagree in a new way, stop and report upstream drift instead of guessing. The reviewed 2026-08-14 baseline is 46 variables in each source with no name-set difference.

Use official descriptions as evidence for caller-visible XML documentation. Paraphrase them into concise English. Retain material conditions, units, formats, and lifetimes. Do not copy large passages or add placeholder summaries.

## Review changed variables semantically

For every added or materially changed variable, record evidence for its exact name, library domain and canonical path, C# type and parser, sensitivity, and missing/malformed behavior.

Infer types only from explicit descriptions or linked official contracts:

- use `bool?` for documented booleans and exact presence markers;
- use numeric types only for documented numeric values;
- use `TimeSpan?`, `Uri?`, `FilePath?`, `DirectoryPath?`, lists, or enums only when the documented representation supports it;
- keep opaque identifiers, refs, hashes, versions, and composite values as strings unless a stronger contract is stated;
- use `CiSecret<T>?` for values documented as sensitive when a public environment-variable property is in scope.

When evidence is insufficient, keep `string?` and document the uncertainty. Never infer a stronger type from a suffix alone.

## Update the hierarchical API

Give each variable exactly one canonical path under the static `TedToolkit.GitHub.GitHubActions` root. Use focused nested static domains and terminal typed properties. Every terminal property must read the current process environment on demand through `CiEnvironment`; do not create provider-local parsing files.

Preserve the exact environment name by applying the sealed common `CiVariableAttribute` directly to every terminal property. Do not add an empty provider-specific variable attribute. For every closed documented string set, put each exact token on its enum field with `CiEnumValueAttribute` and use the common cached enum reader; do not add a provider-local switch parser. Update `src/TedToolkit.CiContext.GitHubActions/GitHubActions.Variables.cs` and the exact snapshots in `tests/TedToolkit.CiContext.GitHubActions.Tests/OfficialVariableNames.cs` together.

Do not add expression contexts, parse event payloads, expose `GITHUB_TOKEN`, evaluate workflows, write workflow-command files, capture environment dictionaries, or cache values. Those behaviors are outside this package contract.

## Verify with TDD

Before production edits, add or update TUnit contract tests proving:

- the exact rendered-reference name set appears once;
- the maintained-Markdown set and the known difference remain explicit;
- every terminal property carries exact-name metadata;
- representative canonical paths return typed values;
- missing and malformed values preserve their contracts;
- environment-mutating tests are serialized and restore every changed value.

Run:

```shell
dotnet run --project tests/TedToolkit.CiContext.GitHubActions.Tests/TedToolkit.CiContext.GitHubActions.Tests.csproj -c Release
dotnet build TedToolkit.CiContext.slnx -c Release
```

Require zero diagnostics and all intended tests discovered. Update the provider README when a public path or type changes.

## Report synchronization

Report both source URLs and the retrieval date, the two exact source counts, their difference, added or removed variables, evidence-backed type decisions, API migrations, tests, and build result. Identify variables kept as `string?` because GitHub does not document a stronger representation.
