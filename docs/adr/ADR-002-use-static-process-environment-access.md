# ADR-002: Use static process-environment access

- Status: Accepted
- Date: 2026-08-13
- Decision owner: TedToolkit maintainers
- Decision scope: State ownership and access shape for typed GitLab CI/CD variables.
- Applicable product intent: `../product/README.md`
- Applicable principles: None
- Supersedes: `ADR-001-hierarchical-variable-api.md` for instance-view and snapshot consequences
- Superseded by: None

## Clarification and decision log

| ID | Question and why it mattered | Recommended answer | User decision and source | Affected sections | Status |
| --- | --- | --- | --- | --- | --- |
| ADR-C-01 | Is an instantiated snapshot necessary when the library represents the current CI process? | No; make the root and hierarchy static and read `Environment` on demand. | Required by the repository owner on 2026-08-13. | Decision; consequences | Resolved |

## Decision at a glance

Expose `GitLabCi` and every phase, domain, and subdivision as nested static classes. Each terminal property reads the current process environment at access time.

## Context and decision question

The prior model created an immutable dictionary snapshot and zero-allocation instance navigation views. The product boundary has been narrowed to the current GitLab CI process, so representing multiple supplied environments is unnecessary complexity. The decision is whether state belongs in instantiated contexts or in the process environment already supplied by GitLab Runner.

## Decision drivers and constraints

| Type | Driver or constraint | Evidence or source | Priority |
| --- | --- | --- | --- |
| Product constraint | The library represents the current process environment only. | Repository owner decision on 2026-08-13. | Must |
| API driver | Navigation should require no capture or context instantiation. | Repository owner decision on 2026-08-13. | High |
| Compatibility constraint | Exact variable metadata, typing, missing behavior, and secret handling remain unchanged. | Existing public contract and official GitLab reference. | Must |
| Test constraint | Process environment mutation is global and must be serialized and restored. | .NET process environment semantics. | Must |

## Options and evidence

| Option | Evidence and confidence | Meets drivers | Decisive trade-off | Outcome |
| --- | --- | --- | --- | --- |
| Immutable dictionary-backed contexts | Existing implementation supports independent snapshots. | No | Adds construction and state for a capability outside the revised product boundary. | Superseded |
| Nested static classes reading `Environment` | Directly models GitLab Runner's process contract. | Yes | Cannot represent two independent environments in one process; tests must coordinate global state. | Accepted |
| Static cached snapshot | Avoids repeated lookups. | Partly | Values become stale and still allocate a full dictionary. | Rejected |

## Decision

Use nested static classes and on-demand `Environment.GetEnvironmentVariable` access. Retain a static exact-name raw accessor. Do not expose `Capture`, `FromVariables`, `RawVariables`, context constructors, or instance navigation aliases.

## Consequences and accepted trade-offs

- Calls are concise, such as `GitLabCi.PrePipeline.Commit.Sha`.
- Access reflects the value present at the time of each property read.
- No context, view, or full-environment snapshot is allocated.
- Independent variable sets cannot coexist within one process.
- Tests and consumers that change environment variables must serialize mutation and restore prior values.
- Repeated reads perform repeated operating-system environment lookups and parsing.

## Downstream delivery constraints

- Preserve the accepted phase-first hierarchy and exact official-name coverage.
- Keep parsing invariant and sensitive values redacted by implicit formatting.
- Keep only stateless static parsing helpers and immutable enum lookup data.
- Do not add a compatibility layer for the unpublished instance prototype.

## Exit requirements

All currently documented variables remain uniquely discoverable through static terminal properties, raw on-demand access remains available, environment-mutating tests restore state, and every target framework builds without diagnostics.

## Follow-ups and review triggers

| Item | Owner | Due date or objective trigger | Status |
| --- | --- | --- | --- |
| Reconsider snapshots if consumers require simultaneous independent contexts. | TedToolkit maintainers | Confirmed multi-context use case | Open |
| Measure repeated lookup cost if profiling identifies environment access as material. | TedToolkit maintainers | Production performance evidence | Open |
