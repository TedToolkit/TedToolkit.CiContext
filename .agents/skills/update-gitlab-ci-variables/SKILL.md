---
name: update-gitlab-ci-variables
description: Synchronize TedToolkit.CiContext.GitLab with GitLab's current official predefined CI/CD variable Markdown. Use when adding, removing, reviewing, regrouping, or retyping GitLab CI variable APIs; when GitLab changes its predefined-variable documentation; or when validating that the repository still covers the official variable set, phases, scenario categories, and integration groups.
---

# Update GitLab CI Variables

Update the typed API from GitLab's maintained Markdown while treating type and domain classification as semantic review, not text generation.

## Establish the source and repository contract

1. Read repository guidance, product intent, the variable API architecture, and the accepted hierarchy ADR.
2. Read [api-contract.md](references/api-contract.md) completely before changing the API.
3. Download `https://gitlab.com/gitlab-org/gitlab/-/raw/master/doc/ci/variables/predefined_variables.md`.
4. Use only GitLab official documentation and repository sources to resolve semantics. Follow official links when a description is ambiguous.
5. Treat the maintained Markdown as authoritative over rendered HTML, search results, third-party articles, and the existing C# surface.

## Build the change set

Extract exact names, descriptions, availability text, section headings, and integration subdivisions. Mechanical extraction is allowed; semantic classification is not delegated to a script.

Use the official descriptions as semantic evidence for caller-facing XML documentation. Paraphrase them into concise English rather than copying Markdown verbatim. Give every terminal property a meaningful summary and value contract; retain material availability conditions, units, limits, formats, and lifetimes in remarks. Add exception documentation only for actual typed parsing failures. Convert useful upstream links to absolute `https://docs.gitlab.com/` links and omit links that merely repeat the main reference page.

Compare the official set against terminal properties carrying `CiVariableAttribute`. Report added, removed, renamed, description-changed, and availability-changed variables. Validate official category through `GitLabCiVariableCategoryAttribute` on each terminal property and official availability through `GitLabCiVariableAvailabilityAttribute` on the owning phase class. Reject duplicate names and properties. Do not accept count-only coverage.

If tables, sections, or integration lists no longer match the expected document structure, stop and report upstream drift instead of guessing.

## Review changed variables semantically

For every added or materially changed variable, record evidence for its exact name, official phase, official category or integration, library domain and canonical path, C# type and parser, sensitivity, and missing/malformed behavior.

Infer types only from explicit descriptions or linked official contracts:

- use `bool?` for documented booleans or presence flags;
- use numeric types only for documented numeric values, never merely because a name ends in `ID`;
- use `DateTimeOffset?`, `TimeSpan?`, `Uri?`, `FilePath?`, `DirectoryPath?`, lists, or enums only when the documented representation supports it;
- keep opaque identifiers, tokens, refs, hashes, versions, and composite values as strings unless a stronger contract is stated;
- use `CiSecret<string>?` for credentials, authentication-capable tokens or fragments, and sensitive payloads; keep deliberately non-authenticating display fragments as strings.

When evidence is insufficient, keep `string?` and document the uncertainty. Never invent a type to make the API appear stronger.

## Update the hierarchical API

Give each variable exactly one canonical path: static root, availability phase, focused library domain or official category, optional documented subdivision, and terminal typed property.

Use nested static classes. Every terminal property must read the current process environment on demand through `CiEnvironment`; select an explicit common format when CLR type alone is insufficient. Do not introduce provider-local parsing files, contexts, captured dictionaries, instance views, static value caches, or mutable library-owned global state.

Preserve the exact name by applying the sealed common `CiVariableAttribute` directly, and preserve the official category in the separate sealed `GitLabCiVariableCategoryAttribute`. Keep every provider Attribute source file under the project's `Attributes/` directory without changing its namespace. Do not add an empty provider-specific variable attribute. For every closed documented string set, put each exact token on its enum field with `CiEnumValueAttribute` and use the common cached enum reader; do not add a provider-local switch parser. Preserve each official phase once in `GitLabCiVariableAvailabilityAttribute` on the `PrePipeline`, `Pipeline`, or `JobOnly` static class. Keep `ExternalPullRequest` directly under `GitLabCi` without an availability attribute, because GitLab does not declare an official phase for those variables. Keep library domains distinct from official categories. Do not retain duplicate flat aliases unless a published compatibility contract requires them.

Update the checked-in phase files matching `src/TedToolkit.CiContext.GitLab/GitLabCi.Variables.*.cs` and the exact upstream-name snapshot in `tests/TedToolkit.CiContext.GitLab.Tests/OfficialVariableNames.cs`. The public root is the static `TedToolkit.GitLab.GitLabCi`; do not restore the obsolete `GitLabCiContext` type or instance-based `GitLabCi`. Do not restore a standalone type-inference generator. The skill owns semantic review; mechanical formatting and set comparison may still use local tooling.

Keep the XML documentation in those checked-in variable files synchronized with each reviewed description. Reject placeholder summaries such as `Gets the typed value of ...`, because they do not communicate the GitLab contract.

## Verify with TDD

Before production edits, add or update TUnit contract tests proving:

- the exact official name set appears once;
- phase and category metadata match the reviewed source;
- representative canonical paths return typed values;
- absent, malformed, and sensitive values preserve their contracts;
- no large flat terminal-property surface remains.
- environment-mutating tests are serialized and restore every changed value.

Run:

```shell
dotnet run --project tests/TedToolkit.CiContext.GitLab.Tests/TedToolkit.CiContext.GitLab.Tests.csproj -c Release
dotnet build TedToolkit.CiContext.slnx -c Release
```

Require zero diagnostics and all intended tests discovered. Update both README files when a public path or type changes.

## Report synchronization

Report source URL and retrieval date, exact added/removed/changed variables, evidence-backed semantic decisions, API migrations, tests, and build result. Identify variables kept as `string?` because GitLab does not document a stronger representation.
