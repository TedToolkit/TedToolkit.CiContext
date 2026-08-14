# GitLab CI variable API architecture

- Status: Active
- Owner: TedToolkit maintainers
- Scope and system boundary: The public .NET API that maps documented GitLab CI/CD environment variables to discoverable typed views.
- Applicable product intent: `docs/product/README.md` (approved locally; no commit SHA exists before the initial commit)
- Governing principles: None
- Related ADRs: `docs/adr/ADR-001-hierarchical-variable-api.md`; `docs/adr/ADR-002-use-static-process-environment-access.md`; `docs/adr/ADR-005-share-ci-value-parsing.md`
- Last approved revision: 2026-08-13

## Clarification and decision log

| ID | Question and why it mattered | Recommended answer | User decision and source | Affected sections | Status |
| --- | --- | --- | --- | --- | --- |
| AD-01 | Should the API preserve the official phase and category relationships instead of exposing one large flat property set? This determines the durable public navigation model. | Yes; expose one hierarchy that makes availability visible before domain/category selection. | The user rejected a single large category and required official phases and categories to appear in the API on 2026-08-13. | Current architecture; Constraints for change design | Resolved |
| AD-02 | Which relationship should be the first navigation axis? Choosing phase-first or category-first changes every consumer call site. | Phase-first, because availability determines whether a variable can exist at the point where it is consumed. | Approved by the repository owner on 2026-08-13: the proposed hierarchy was accepted. | Current architecture | Resolved |
| AD-03 | Should navigation views be instantiated and snapshot-backed? This determines state ownership and call shape. | No; use nested static classes and read the current process environment on every terminal-property access. | Required by the repository owner on 2026-08-13. | Current architecture; constraints | Resolved |
| AD-04 | Should every terminal variable repeat its availability, or should availability describe the phase navigation boundary? Repetition obscures that availability is documentation metadata rather than current runtime state. | Mark only the three official phase classes with `GitLabCiVariableAvailabilityAttribute`; keep terminal metadata focused on the variable name and official category. | Approved by the repository owner on 2026-08-13. | Current architecture; constraints | Resolved |

## Current architecture

The public boundary is the static `GitLabCi` type. Its nested static classes expose phase-first navigation, and every terminal property reads the current process environment when accessed:

```text
GitLabCi
├── PrePipeline
│   ├── Commit
│   ├── Project
│   ├── Server
│   ├── MergeRequest
│   └── AutoDevOps
├── Pipeline
│   ├── Environment
│   ├── Job
│   ├── Chat
│   └── User
├── JobOnly
│   ├── Job
│   ├── Runner
│   ├── Registry
│   └── Integrations
│       ├── Harbor
│       ├── AppStoreConnect
│       ├── GooglePlay
│       └── DiffblueCover
└── ExternalPullRequest
    ├── Source
    └── Target
```

The domain set is maintained as reviewed library design data. An official scenario category is nested under its documented phase. External pull request variables remain context-specific because the official reference does not assign them one of the three availability phases.

Every terminal property carries `CiVariableAttribute` directly for its exact environment name and a separate `GitLabCiVariableCategoryAttribute` for its official category. Closed documented values declare exact `CiEnumValueAttribute` mappings and use the common cached enum reader. The `PrePipeline`, `Pipeline`, and `JobOnly` navigation classes each carry `GitLabCiVariableAvailabilityAttribute`, so phase metadata is declared once at the same boundary that expresses it. `ExternalPullRequest` is a direct root category without availability metadata because GitLab does not assign those variables an official phase. A variable has one canonical typed navigation path; the library does not duplicate the same property in parallel phase and category indexes.

`CiEnvironment.GetRawValue(name)` performs the same on-demand lookup for custom, deployment-specific, future, and malformed variables. No context object, environment snapshot, raw dictionary, or mutable library-owned global state exists. Consumers that mutate process variables own synchronization and restoration.

When a phase and domain would have the same C# name, variables in that identity domain are direct members of the phase class. For example, `CI_PIPELINE_IID` is `GitLabCi.Pipeline.Iid`, avoiding the illegal and redundant `Pipeline.Pipeline` declaration.

The official GitLab documentation page and its repository Markdown source are the upstream authority. GitLab does not publish a public machine-readable manifest in the documentation source tree, so the project skill extracts the Markdown structure, requires semantic review for types and domains, and fails on unrecognized structure.

## Constraints for change design

- The generated surface must cover every explicitly named variable from the current official reference exactly once.
- The three documented availability phases must remain distinct: Pre-pipeline, Pipeline, and Job-only.
- Merge request and Auto DevOps variables must be modeled as Pre-pipeline categories.
- Integration variables must be modeled as a Job-only category and retain their documented integration subdivisions.
- External pull request variables must not be assigned a phase that GitLab does not document.
- Deployment variables cannot have a fixed typed list because integrations define their names; they remain accessible through the explicit raw-name accessor.
- Domain grouping inferred by this library must be distinguishable from official GitLab categories in metadata and documentation.
- Synchronization must fail on unrecognized upstream document structure, duplicate variable names, missing variables, or unassigned domain mappings.
- The API must not parse `.gitlab-ci.yml`, call GitLab APIs, or execute pipelines.
- Every terminal lookup must read `Environment.GetEnvironmentVariable` on demand; no captured variable dictionary is permitted.
- Nested navigation types must be static and allocate no context or view objects.

## Decision links and exceptions

`ADR-001` selected the phase-first hierarchy. `ADR-002` supersedes its instance-view consequences with static process-environment access. `ADR-005` owns shared variable metadata and stable parsing. No principle exception is currently required.

## Review triggers

- GitLab publishes an official machine-readable predefined-variable schema.
- GitLab adds or changes availability phases or category semantics.
- A variable cannot be assigned to one unambiguous domain without duplicating it.
- Consumer evidence shows phase-first navigation obscures rather than improves common use cases.
- A stable release requires compatibility with the current flat property surface.
- Consumers require simultaneous independent variable sets within one process.
