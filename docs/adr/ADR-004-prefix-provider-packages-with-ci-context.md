# ADR-004: Prefix provider packages with the CI context family name

- Status: Accepted
- Date: 2026-08-13
- Decision owner: TedToolkit maintainers
- Decision scope: Public project, assembly, and NuGet package names for CI providers.
- Applicable product intent: `../product/README.md` (approved locally; no initial commit SHA exists)
- Applicable principles: None
- Supersedes: The provider-package naming portion of ADR-003
- Superseded by: None

## 📝 Clarification and decision log

| ID | Question and why it mattered | Recommended answer | User decision and source | Affected sections | Status |
| --- | --- | --- | --- | --- | --- |
| ADR-N-01 | Whether provider packages should share the family prefix determines package discovery and naming consistency. | Name providers `TedToolkit.CiContext.GitLab` and `TedToolkit.CiContext.GitHubActions`. | Requested and approved by the repository owner on 2026-08-13. | Decision; consequences | Resolved |
| ADR-N-02 | Whether CLR namespaces should mirror the longer package IDs determines consumer ergonomics and source compatibility. | Keep `TedToolkit.GitLab` and `TedToolkit.GitHub`; unify distribution names only. | Approved through the repository owner's package-naming request and the stated implementation boundary on 2026-08-13. | Decision; compatibility | Resolved |

## 📌 Decision at a glance

Use `TedToolkit.CiContext` as the stable package-family prefix: publish provider projects, assemblies, and packages as `TedToolkit.CiContext.GitLab` and `TedToolkit.CiContext.GitHubActions`, while retaining concise provider namespaces and static API roots.

## 🧭 Context and decision question

ADR-003 established independent provider packages but named them `TedToolkit.GitLabCiContext` and `TedToolkit.GitHubActionsCiContext`. The repository owner requires the provider packages to begin with `TedToolkit.CiContext.` so package search, sorting, and family membership are immediately visible.

The decision is whether distribution identity and CLR namespace identity must both adopt that prefix. They serve different consumers: package IDs organize artifacts, while namespaces optimize code readability and compatibility.

## 🎯 Decision drivers and constraints

| Type | Driver or constraint | Evidence or source | Priority |
| --- | --- | --- | --- |
| Hard constraint | Both provider package names begin with `TedToolkit.CiContext.`. | Repository owner requirement. | Must |
| Hard constraint | Provider packages remain independent siblings that depend only on the foundation and shared infrastructure. | ADR-003. | Must |
| Decision driver | Provider namespaces and entry points remain concise in consumer code. | Existing `TedToolkit.GitLab.GitLabCi` and `TedToolkit.GitHub.GitHubActions` APIs. | High |
| Decision driver | Project, assembly, package, test, solution, and documentation identities remain discoverably consistent. | Repository scaffolding convention. | High |

## 🔎 Options and evidence

| Option | Evidence and confidence | Meets drivers | Decisive trade-off | Outcome |
| --- | --- | --- | --- | --- |
| Keep the existing `*CiContext` suffix | Current implementation; high confidence. | No | Family packages do not sort or read under one prefix. | Rejected |
| Prefix both packages and CLR namespaces with `TedToolkit.CiContext` | Technically valid; high confidence. | Partly | Produces verbose namespaces and needless source migration. | Rejected |
| Prefix project, assembly, and package identities only | .NET permits intentional namespace divergence; high confidence. | Yes | Requires the divergence to remain documented. | Selected |

## ✅ Decision

Use these public distribution identities:

- `TedToolkit.CiContext`
- `TedToolkit.CiContext.GitLab`
- `TedToolkit.CiContext.GitHubActions`

The provider source namespaces remain `TedToolkit.GitLab` and `TedToolkit.GitHub`. The public roots remain `GitLabCi` and `GitHubActions`. Test and benchmark project names follow their corresponding production-project prefix.

## 💡 Why this decision now

The packages have not been published or committed as a release. Renaming now establishes one discoverable family without requiring a later compatibility package or deprecation cycle.

## 🔗 Evidence and links

- `docs/product/README.md`
- `docs/architecture/ci-context-package-family.md`
- `docs/adr/ADR-003-use-provider-packages-over-a-ci-context-foundation.md`

## ⚖️ Consequences and accepted trade-offs

- Package discovery and alphabetical grouping consistently expose the `TedToolkit.CiContext` family.
- Provider packages continue to version and install independently.
- Existing local project paths, assembly names, documentation paths, and build identifiers must migrate together.
- Package IDs intentionally differ from CLR namespaces; project documentation must show both.
- The remote repository URL may temporarily retain its old name until the repository owner performs the external rename.

## 🛠️ Downstream delivery constraints

- Provider project names, assembly names, NuGet IDs, focused test project names, and project directories use the new prefix.
- Provider package dependencies reference `TedToolkit.CiContext` and never one another.
- Preserve all existing public types, namespaces, navigation, variable coverage, and runtime behavior.
- Verify package identities and dependency groups in produced NuGet packages.

## 🔄 Exit requirements

A future naming scheme that removes the `TedToolkit.CiContext` prefix or changes provider namespaces requires a superseding ADR and an explicit package migration strategy after publication.

## 📅 Follow-ups and review triggers

| Item | Owner | Due date or objective trigger | Status |
| --- | --- | --- | --- |
| Rename the remote repository and update its canonical package project URL. | Repository owner | Before public package publication | Open |
| Reassess the suffix when a provider's official product name changes. | TedToolkit maintainers | Provider branding change | Open |
