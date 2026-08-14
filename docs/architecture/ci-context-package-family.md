# CI context package-family architecture

- Status: Active
- Owner: TedToolkit maintainers
- Scope and system boundary: Public .NET packages that expose supported CI providers' current-process environment variables.
- Applicable product intent: `docs/product/README.md` (approved locally; no initial commit SHA exists)
- Governing principles: None
- Related ADRs: `docs/adr/ADR-003-use-provider-packages-over-a-ci-context-foundation.md`; `docs/adr/ADR-004-prefix-provider-packages-with-ci-context.md`; `docs/adr/ADR-005-share-ci-value-parsing.md`; `docs/adr/ADR-006-centralize-ci-formats-and-value-secrets.md`; `docs/adr/ADR-007-use-automatic-secret-identification.md`; `docs/adr/ADR-008-use-policy-free-common-environment-readers.md`; `docs/adr/ADR-009-compose-variable-metadata.md`
- Last approved revision: Approved locally on 2026-08-13; no initial commit SHA exists

## 📝 Clarification and decision log

| ID | Question and why it mattered | Recommended answer | User decision and source | Affected sections | Status |
| --- | --- | --- | --- | --- | --- |
| AD-01 | Which package owns cross-provider behavior determines dependency direction. | A provider-neutral foundation owns only stable shared contracts. | Approved by the repository owner on 2026-08-13. | Current architecture; constraints | Resolved |
| AD-02 | Whether provider navigation must be symmetric determines how accurately official semantics can be represented. | Preserve provider-specific navigation instead of structural symmetry. | Approved by the repository owner on 2026-08-13. | Current architecture; constraints | Resolved |
| AD-03 | Whether distribution names visibly belong to one family determines package discovery and consistency. | Prefix provider packages with `TedToolkit.CiContext.` while preserving concise CLR namespaces. | Approved by the repository owner on 2026-08-13. | Current architecture; constraints | Resolved |
| AD-04 | Whether stable parsing belongs in each provider determines maintenance and performance boundaries. | Share invariant scalars and cached exact-token enums; retain provider-specific formats. | Approved by the repository owner on 2026-08-13. | Current architecture; constraints | Resolved |
| AD-05 | Whether shared paths and explicit format dialects justify a common dependency determines whether provider parsing files remain useful. | Let the foundation depend on `TedToolkit.FileSystem`, expose explicit reusable formats, and remove provider parsing files. | Approved by the repository owner on 2026-08-13. | Current architecture; constraints | Resolved |
| AD-06 | Secret representation controls allocation and accidental disclosure. | Use a readonly value wrapper with implicit full-redaction conversion and opt-in safe partial masks. | Approved by the repository owner on 2026-08-13. | Current architecture; constraints | Resolved |
| AD-07 | Whether callers should configure secret display determines the public API size and consistency of accidental formatting. | Remove configurable masks and automatically show only the final four characters when at least eight characters remain hidden. | Approved by the repository owner on 2026-08-13. | Current architecture; constraints | Resolved |
| AD-08 | Whether overlapping Boolean dialects and provider raw forwarders add domain value determines the shared public API boundary. | Accept the exact common Boolean-token union and keep provider-neutral raw access on `CiEnvironment`. | Approved by the repository owner on 2026-08-13. | Current architecture; constraints | Resolved |
| AD-09 | Whether provider metadata should inherit from the common name contract determines API size and analyzer compliance. | Seal and apply `CiVariableAttribute` directly; represent orthogonal provider facts with separate attributes. | Approved by the repository owner on 2026-08-13. | Current architecture; constraints | Resolved |

## Current architecture

```text
TedToolkit.CiContext.GitLab ---------┐
                                    ├──> TedToolkit.CiContext
TedToolkit.CiContext.GitHubActions ┘
```

`TedToolkit.CiContext` owns `CiEnvironment`, `CiVariableAttribute`, `CiEnumValueAttribute`, reusable environment-value formats, direct closed-generic enum caching, and `CiSecret<T>`. It references `TedToolkit.FileSystem`, has no provider dependency or public provider model, and reads the current process environment on every accessor call.

`TedToolkit.CiContext.GitLab` preserves the `TedToolkit.GitLab` namespace and phase-first `GitLabCi` hierarchy. `TedToolkit.CiContext.GitHubActions` preserves the `TedToolkit.GitHub` namespace and uses provider-specific semantic groups beneath `GitHubActions`; these groups are library navigation, not official GitHub categories.

Provider packages own semantic reader selection, enums, provider-specific metadata, official-source synchronization, coverage manifests, and consumer documentation. Every terminal property uses the common sealed `CiVariableAttribute` directly for its exact name. Provider-specific facts such as GitLab documentation categories use separate sealed attributes. Provider packages do not own parsing helper files or add provider-neutral raw-access forwarders. Tests reference their production project and may reference the common package; production code never references tests or another provider.

## Constraints for change design

- Dependency direction must remain acyclic and provider packages must remain independently installable.
- The common package may reference `TedToolkit.FileSystem` but must not encode provider-specific variable names, phases, categories, enums, or detection.
- Public parsing errors must identify the variable and expected representation without including its value.
- Missing variables return `null`; typed properties read live process values and malformed values throw `FormatException`.
- Every terminal variable property carries `CiVariableAttribute` directly; provider-specific metadata uses separate attributes rather than inheritance.
- The common package parses reusable booleans, markers, invariant numbers, timestamps, explicit duration units, absolute URIs, paths, lists, and exact Attribute-mapped enums. Its Boolean reader accepts only `true`, `false`, `yes`, `no`, `1`, and `0`; providers use exact presence markers when their contract is stricter.
- Enum mapping reflection occurs only during direct closed-generic static initialization; steady-state access uses an ordinal cache without `Lazy<T>` indirection.
- `CiSecret<T>` is a `readonly struct`. Its ordinary formatting fully redacts representations shorter than 12 characters and otherwise shows only the final four characters, leaving at least eight hidden. It requires `Reveal()` for value access and exposes no configurable display policy.
- GitHub coverage authority is the current English rendered variables reference. The official source Markdown is a secondary structured source; disagreement requires human review.
- GitHub expression contexts, secrets contexts, event payload parsing, workflow-command writing, and API access remain outside the package boundary.

## Decision links and exceptions

ADR-003 establishes the three-package boundary. ADR-004 establishes distribution naming while preserving concise namespaces. ADR-005 introduced shared metadata and parsing; ADR-006 supersedes its narrower dependency, ownership, initialization, and secret-representation decisions; ADR-007 supersedes ADR-006's configurable secret display; ADR-008 supersedes caller-selected Boolean formats and redundant provider raw access for new providers; ADR-009 supersedes provider-specific variable-attribute inheritance. ADR-001 and ADR-002 continue to govern only the GitLab provider API. No principle exception is required.

## Review triggers

- A provider package requires a reverse or cross-provider dependency.
- A common API would need provider-specific behavior or dependency weight.
- A third CI provider cannot use the shared metadata or secret contracts honestly.
- Consumers require snapshots, normalized cross-provider properties, GitHub expression contexts, or command-file operations.
- Official rendered and source variable references cannot be reconciled reproducibly.
