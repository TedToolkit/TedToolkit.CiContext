# ADR-006: Centralize CI formats and use value-type secrets

- Status: Accepted
- Date: 2026-08-13
- Decision owner: TedToolkit maintainers
- Decision scope: Common parsing ownership, filesystem dependency, enum-cache initialization, and secret formatting.
- Applicable product intent: `../product/README.md` (approved locally; no initial commit SHA exists)
- Applicable principles: None
- Supersedes: The parsing boundary, filesystem-independence, lazy enum initialization, and class-based secret portions of ADR-003 and ADR-005
- Superseded by: ADR-007 for automatic secret display and removal of configurable masks; ADR-008 for caller-selected boolean formats

## 📝 Clarification and decision log

| ID | Question and why it mattered | Recommended answer | User decision and source | Affected sections | Status |
| --- | --- | --- | --- | --- | --- |
| ADR-C-01 | Whether filesystem values are shared formats determines the common package dependency boundary. | Let `TedToolkit.CiContext` reference `TedToolkit.FileSystem` and own reusable path readers. | Approved by the repository owner on 2026-08-13. | Decision; consequences | Resolved |
| ADR-C-02 | Whether provider parsing files still add semantics determines their continued ownership. | Expose named common readers with explicit format options and remove provider parsing files. | Approved by the repository owner on 2026-08-13. | Decision; constraints | Resolved |
| ADR-C-03 | Whether `CiSecret<T>` needs an object wrapper and fixed display determines allocation and disclosure behavior. | Use a `readonly struct`, implicit wrapping, safe default state, full redaction by default, and explicit edge masks that can never reveal the whole value. | Approved with instruction to implement on 2026-08-13. | Decision; security constraints | Resolved |
| ADR-C-04 | Whether a second lazy-init layer improves enum lookup determines steady-path overhead. | Rely on closed-generic static initialization and unwrap definition errors to preserve the public exception contract. | Approved by the repository owner on 2026-08-13. | Decision; performance constraints | Resolved |

## 📌 Decision at a glance

The common package owns reusable CI value formats, including filesystem values. Secrets are allocation-free wrappers with safe, explicit display policies, and enum metadata uses direct closed-generic static initialization.

## 🧭 Context and decision question

Provider parsing files had become forwarding layers plus small format variations. The common package was already the stable parsing boundary, but its filesystem-independent constraint prevented complete removal of the duplication. `CiSecret<T>` also allocated one wrapper object per access, and `Lazy<T>` duplicated initialization already guaranteed by a closed generic static type.

## 🎯 Decision drivers and constraints

| Type | Driver or constraint | Priority |
| --- | --- | --- |
| Hard constraint | Missing variables return `null`; malformed values never appear in exceptions. | Must |
| Hard constraint | Secret formatting must never expose a complete secret accidentally. | Must |
| Hard constraint | Provider packages retain official semantics and public navigation. | Must |
| Decision driver | Shared value formats have one tested implementation. | High |
| Decision driver | Repeated reads avoid wrapper allocation and unnecessary lazy indirection. | High |

## ✅ Decision

`TedToolkit.CiContext` references `TedToolkit.FileSystem` and exposes named readers for raw text, documented boolean formats, exact presence markers, invariant integers, ISO 8601 timestamps, durations with explicit units, absolute URIs, file and directory paths, comma-separated lists, JSON string arrays, and Attribute-mapped enums. Providers select the documented format at each terminal property. Provider-specific variable names and navigation remain outside the common package.

`CiSecret<T>` is a `readonly struct`. Its constructor rejects null, implicit conversion from `T` applies full redaction, and `Reveal()` is the only value accessor. The default struct value formats as `[REDACTED]` and throws from `Reveal()`. `CiSecretMask` permits explicit prefix, suffix, or edge display; `WithMask` changes policy without revealing the value, and if no hidden character would remain, formatting falls back to full redaction. Provider properties use full redaction.

Security classification follows capability rather than spelling. Credentials, authenticated URLs, and authentication-capable token fragments use `CiSecret<string>?`. Names and deliberately non-authenticating display fragments remain strings.

Enum maps initialize directly in a static field on the closed generic cache. Successful access has no `Lazy<T>` layer. Definition failures are unwrapped to retain the documented `InvalidOperationException` contract.

## ⚖️ Consequences and accepted trade-offs

- Provider `*.Parsing.cs` files disappear, and provider projects receive `TedToolkit.FileSystem` transitively through the common package.
- Consumers of the common package also receive the filesystem dependency.
- Changing `CiSecret<T>` from a class to a struct changes nullable reflection types to `Nullable<CiSecret<T>>`; this pre-release breaking change is accepted.
- Partial masking formats a value only when explicitly configured and may allocate the returned string; normal wrapping itself does not allocate a wrapper object.
- Direct generic static initialization can wrap initialization failures internally, so the public reader explicitly restores the intended definition exception.

## 🔗 Evidence and links

- The maintained `StorageBenchmarks` ShortRun on .NET 10.0.11 measured 24 B for each reference wrapper and 0 B for `CiSecret<T>` value wrapping. Its 0.086 ns value-wrapper result was indistinguishable from empty-method overhead.
- Initialized lazy and direct dictionary reads measured 5.212 ns and 5.330 ns with 0 B respectively, which is not evidence of a meaningful speed difference. Direct initialization is selected because access guarantees initialization and therefore has no deferral benefit.
- [GitLab's maintained predefined-variable Markdown](https://gitlab.com/gitlab-org/gitlab/-/raw/master/doc/ci/variables/predefined_variables.md) documents `CI_RUNNER_SHORT_TOKEN` as used to authenticate new job requests and `CI_TRIGGER_SHORT_TOKEN` as only the first four characters of a trigger token.
- [GitHub's official default environment-variable reference](https://docs.github.com/en/actions/reference/workflows-and-actions/variables) contains no token variable; expression contexts and `GITHUB_TOKEN` remain outside this package.

## 🛠️ Downstream delivery constraints

- Keep Attribute source files under an `Attributes/` directory without changing public namespaces.
- Do not add a generic `GetSecret`; nullable environment access must distinguish absence before implicit wrapping.
- Do not infer sensitivity from a variable name alone; use current official provider semantics.
- Preserve all target frameworks and a zero-warning Release build.

## 🔄 Exit requirements

Introduce a different masking or parser registry only with a new security or extensibility requirement. Replace reflection caching only if measured workloads, trimming, or NativeAOT requirements justify source generation.
