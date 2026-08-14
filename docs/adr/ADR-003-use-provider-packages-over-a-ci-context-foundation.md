# ADR-003: Use provider packages over a CI context foundation

- Status: Accepted
- Date: 2026-08-13
- Decision owner: TedToolkit maintainers
- Decision scope: Package boundaries and dependency direction for the CI context family.
- Applicable product intent: `../product/README.md` (approved locally; no initial commit SHA exists)
- Applicable principles: None
- Supersedes: None
- Superseded by: ADR-004 for provider-package naming; ADR-005 for shared metadata; ADR-006 for parser ownership and the common filesystem dependency

## 📝 Clarification and decision log

| ID | Question and why it mattered | Recommended answer | User decision and source | Affected sections | Status |
| --- | --- | --- | --- | --- | --- |
| ADR-C-01 | Whether GitHub support belongs beside GitLab in one package determines dependency and versioning coupling. | Use separate provider packages. | Approved by the repository owner on 2026-08-13. | Decision; consequences | Resolved |
| ADR-C-02 | Whether shared behavior justifies a package determines the stable dependency root. | Put exact-name access, common metadata, and generic redaction in `TedToolkit.CiContext`. | Approved by the repository owner on 2026-08-13. | Decision; constraints | Resolved |

## 📌 Decision at a glance

Publish `TedToolkit.CiContext` as a provider-neutral foundation and publish GitLab and GitHub Actions support as separate packages that depend on it but never on each other.

## 🧭 Context and decision question

The original repository contains one `TedToolkit.GitLabCiContext` package with a static, phase-aware GitLab API. Adding GitHub Actions raises whether the existing package should expand, whether each provider should live independently, and whether shared contracts deserve a stable public boundary.

GitLab documents phase and scenario relationships. GitHub Actions documents a flat set of default runner environment variables and separately defines expression contexts. The provider models therefore cannot share one honest navigation hierarchy.

## 🎯 Decision drivers and constraints

| Type | Driver or constraint | Evidence or source | Priority |
| --- | --- | --- | --- |
| Hard constraint | Existing GitLab package identity and provider-specific navigation remain available. | Current public API and approved product intent. | Must |
| Hard constraint | Provider packages must not depend on one another. | Independent consumer and release boundaries. | Must |
| Hard constraint | Shared APIs must contain no GitLab or GitHub semantics. | Provider-neutral foundation responsibility. | Must |
| Decision driver | Consumers install only their selected provider surface. | Repository owner requirement. | High |
| Decision driver | Ordinary formatting of sensitive wrapper values remains redacted. | Existing `GitLabCiSecret` behavior and repository owner requirement. | High |

## 🔎 Options and evidence

| Option | Evidence and confidence | Meets drivers | Decisive trade-off | Outcome |
| --- | --- | --- | --- | --- |
| Keep GitLab only | Existing architecture; high confidence. | No | Does not provide the requested GitHub capability. | Rejected |
| Put both providers in one package | Technically possible; high confidence. | Partly | Couples unrelated consumers and provider evolution. | Rejected |
| Create independent repositories without a common package | Preserves provider isolation; high confidence. | Partly | Duplicates public contracts and loses one product-family entry point. | Rejected |
| Use one neutral foundation plus separate provider packages | Directly satisfies approved product boundaries; high confidence. | Yes | Adds a small transitive package and coordinated compatibility responsibility. | Selected |

## ✅ Decision

Use three public package boundaries. ADR-004 supplies their current distribution names. Both provider packages depend on `TedToolkit.CiContext`; the common package has no provider dependency. Do not introduce a common context interface, base navigation type, provider detector, normalized property model, or public parser framework.

The common package owns exact-name environment access, cross-provider variable-name metadata, and `CiSecret<T>`. Provider packages own navigation, provider-specific parsing, enums, availability, grouping, documentation authority, and exact coverage manifests. ADR-005 moves stable scalar and mapped-enum parsing into the common package.

## 💡 Why this decision now

The approved product scope now includes two providers, while the repository has no committed release history. Establishing the dependency direction before the second API is added avoids embedding GitHub semantics in the GitLab package or publishing a false common model.

## 🔗 Evidence and links

- [GitHub Actions variables reference](https://docs.github.com/en/actions/reference/workflows-and-actions/variables)
- [GitHub Actions contexts reference](https://docs.github.com/en/actions/reference/workflows-and-actions/contexts)
- [GitLab predefined variables reference](https://docs.gitlab.com/ci/variables/predefined_variables/)

## ⚖️ Consequences and accepted trade-offs

- Provider consumers gain isolated APIs and dependencies.
- Common contract changes require compatibility review across both providers.
- Some private parsing code may remain duplicated because similar string shapes do not prove identical provider semantics.
- The GitLab sensitive-property type changes from `GitLabCiSecret` to `CiSecret<string>` before the initial committed release.
- Package-family documentation and build orchestration become repository-wide rather than GitLab-specific.

## 🛠️ Downstream delivery constraints

- Preserve the static live-read model and exact raw access on each provider root.
- Keep provider-specific metadata and enums out of `TedToolkit.CiContext`.
- Keep the common package free of provider and filesystem package dependencies.
- Give every officially covered variable exactly one provider-specific typed path.
- Never include a sensitive raw value in formatting, parsing exceptions, or diagnostics.

## 🔄 Exit requirements

A future normalized facade, snapshot model, or provider plug-in system requires a superseding ADR with demonstrated consumer demand. Existing provider packages must remain independently consumable if the common boundary is replaced.

## 📅 Follow-ups and review triggers

| Item | Owner | Due date or objective trigger | Status |
| --- | --- | --- | --- |
| Reassess common metadata if a third provider cannot implement it honestly. | TedToolkit maintainers | Third provider design | Open |
| Reassess package version coordination after independent provider releases create measurable friction. | TedToolkit maintainers | Release evidence | Open |
