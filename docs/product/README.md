# TedToolkit.CiContext product intent

## 📌 Status

Approved

- Product owner or maintainer: TedToolkit maintainers
- Approval owner: Repository owner
- Last reviewed: 2026-08-13

## 📝 Clarification and decision log

| ID | Question and why it mattered | Recommended answer | User decision and source | Affected sections | Status |
| --- | --- | --- | --- | --- | --- |
| PI-01 | Whether provider support belongs in one package or a package family determines consumer dependencies and provider coupling. | Publish provider-specific packages over a small provider-neutral foundation. | Approved by the repository owner on 2026-08-13. | Positioning; value; boundaries | Resolved |
| PI-02 | Whether GitHub Actions means runner environment variables or all expression contexts determines the feasible product boundary. | Model only officially documented default runner environment variables. | Approved by the repository owner on 2026-08-13. | Consumers; problem; boundaries | Resolved |
| PI-03 | Whether sensitive values require a provider-specific wrapper determines ownership of redaction behavior. | Provide a generic `CiSecret<T>` whose ordinary formatting is redacted. | Approved by the repository owner on 2026-08-13. | Value; boundaries; implications | Resolved |

## 🎯 Positioning

TedToolkit.CiContext is a family of .NET packages that gives software running in supported CI jobs a typed, discoverable, live view of the current process environment. Provider packages preserve each CI system's documented semantics while a small provider-neutral package supplies only genuinely shared contracts.

## 👥 Target consumers and situations

The primary consumers are .NET build tools, release automation, and applications running inside GitLab CI or GitHub Actions jobs. Consumers install only the provider package they use; the common package is also useful for exact-name access and accidental-formatting protection for sensitive values.

## ⚠️ Problem and evidence

CI providers expose job metadata as environment-variable strings with provider-specific availability, naming, types, and evolution. Consumers otherwise repeat lookup, parsing, missing-value handling, metadata discovery, and sensitive-value precautions. GitLab and GitHub publish separate variable references with materially different organization, so a normalized lowest-common-denominator model would discard useful provider semantics.

## ✨ Intended value

Consumers should use nullable, strongly typed, on-demand properties for every variable explicitly named by the applicable official provider reference. Exact-name raw access preserves custom, future, deployment-specific, and malformed values. Provider-independent consumers may discover exact variable names through shared metadata and use a generic secret wrapper whose normal string representation is redacted.

## 🚫 Deliberate non-goals and boundaries

- The family does not normalize GitLab and GitHub into one `ICiContext` or provider-neutral variable hierarchy.
- The family does not call provider REST or GraphQL APIs, evaluate pipeline or workflow configuration, execute jobs, or manage variables and secrets.
- GitHub expression contexts such as `github`, `env`, `vars`, `job`, `steps`, `matrix`, and `needs` are outside the process-environment boundary.
- Event payload files and workflow-command files may be exposed as paths but are not parsed or modified by the context packages.
- `CiSecret<T>` prevents ordinary formatting from revealing a value; it is not encrypted storage, a serializer policy, or a general secret-management system.
- The family does not create immutable snapshots or represent multiple environment sets in one process.

## ✅ Success evidence and review triggers

- A versioned provider manifest accounts for every variable explicitly named by each selected official reference.
- Tests prove exact unique coverage, live reads, missing and malformed behavior, representative typed conversions, metadata discovery, and redacted secret formatting.
- Provider packages depend only on the common package and shared infrastructure, never on another provider package.
- Upstream variable additions, removals, renames, semantic changes, or disagreement between official rendered and source documentation trigger review.
- A request for another provider, provider-neutral normalized properties, GitHub expression contexts, event parsing, or command-file writing triggers a product-boundary review.

## 🧭 Downstream implications

- Architecture must preserve a provider-neutral foundation with acyclic provider dependencies.
- Provider-specific navigation may differ when official semantics differ.
- Raw values, invariant parsing, missing-as-normal behavior, secret-safe formatting, live process access, and multi-target support remain cross-package quality constraints.
- Root and package README files must state the process-environment boundary and navigate the package family without duplicating this record.

