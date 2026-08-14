# ADR-001: Use a phase-first hierarchical variable API

- Status: Superseded
- Date: 2026-08-13
- Decision owner: TedToolkit maintainers
- Decision scope: The durable public navigation model for strongly typed GitLab CI/CD variables.
- Applicable product intent: `../product/README.md`
- Applicable principles: None
- Supersedes: None
- Superseded by: `ADR-002-use-static-process-environment-access.md`

## Clarification and decision log

| ID | Question and why it mattered | Recommended answer | User decision and source | Affected sections | Status |
| --- | --- | --- | --- | --- | --- |
| ADR-C-01 | Must official availability and category relationships be visible in the API? This determines whether the existing flat surface is acceptable. | Yes. | Required by the user on 2026-08-13. | Decision drivers; Decision | Resolved |
| ADR-C-02 | Should phase or category be the first navigation axis? This determines the canonical consumer path. | Phase first. | Approved by the repository owner on 2026-08-13: the proposed hierarchy was accepted. | Decision | Resolved |

## Decision at a glance

Expose each typed variable once beneath its documented availability phase, followed by a focused domain or official scenario category.

## Context and decision question

The initial prototype generated 195 properties directly on `GitLabCiContext`. It preserved variable metadata but lost the relationships that GitLab communicates through availability phases, scenario sections, and integration subdivisions. The decision was whether consumers should navigate variables phase-first, category-first, or through multiple parallel indexes. The accepted public root is now `GitLabCi`.

GitLab's official reference defines three availability phases in the general table. It separately defines merge request, external pull request, Auto DevOps, deployment, and integration sections. Merge request and Auto DevOps variables are explicitly Pre-pipeline; integration variables are explicitly Job-only; external pull request variables have contextual conditions but no declared phase. The maintained upstream source is Markdown, not a public JSON or YAML manifest.

## Decision drivers and constraints

| Type | Driver or constraint | Evidence or source | Priority |
| --- | --- | --- | --- |
| Hard constraint | Every officially named variable has exactly one typed property. | GitLab predefined-variable reference and coverage tests. | Must |
| Hard constraint | The API must not invent an availability phase. | External pull request section omits a phase. | Must |
| Decision driver | Consumers should see availability before selecting a variable that cannot exist in their current phase. | GitLab documents phase as a primary semantic. | High |
| Decision driver | Large unrelated property lists must be replaced by focused domains and official categories. | User requirement on 2026-08-13. | High |
| Decision driver | One variable should have one canonical path. | Avoids duplicated documentation and divergent generated behavior. | High |
| Constraint | Library-inferred domains must not be presented as official GitLab categories. | GitLab documents sections and prefixes, not a domain taxonomy schema. | Must |

## Options and evidence

| Option | Evidence and confidence | Meets drivers | Decisive trade-off | Outcome |
| --- | --- | --- | --- | --- |
| Keep one flat context | Documented: exact names are accessible, but relationships exist only in attributes. | No | Lowest type count, weakest discoverability. | Rejected |
| Category first, then phase and domain | Documented: mirrors the page's major sections. | Partly | Makes the general predefined category an unnecessary extra step for most calls. | Rejected |
| Phase first, then domain or official category | Documented: mirrors the availability dimension and keeps common paths concise. | Yes | Produces more focused view types and requires a reviewed domain taxonomy. | Accepted |
| Provide both phase and category indexes | Assumed: maximizes browsing choices. | Partly | Duplicates every typed property and creates two public paths that can drift. | Rejected |

## Decision

Use phase-first navigation. General predefined variables are grouped into focused domains beneath `PrePipeline`, `Pipeline`, or `JobOnly`. Fixed-phase official categories are nested beneath that phase. The external pull request category is a direct root child because GitLab documents its conditions without assigning an availability phase. Preserve exact upstream semantics through property metadata and documentation.

## Why this decision now

The repository has not published its first stable API, so correcting navigation now avoids preserving a known usability defect. Phase-first navigation directly exposes the most consequential official relationship—when a variable can exist—while domain and category views prevent any phase from becoming another large flat container.

Category-first navigation adds little information for the large general predefined section, while dual navigation creates duplicated public contracts. The proposed direction is reconsidered if consumer evidence shows category-first navigation better matches real usage or GitLab publishes an authoritative schema with a different hierarchy.

## Evidence and links

- [GitLab predefined CI/CD variables](https://docs.gitlab.com/ci/variables/predefined_variables/)
- [GitLab source Markdown](https://gitlab.com/gitlab-org/gitlab/-/blob/master/doc/ci/variables/predefined_variables.md)
- User requirement recorded on 2026-08-13.

## Consequences and accepted trade-offs

- Consumers gain phase-aware discovery and smaller focused property sets.
- The library exposes more public view types.
- Domain classification is maintained by the project synchronization skill as reviewed library design data.
- The existing flat prototype API is replaced before its first release rather than carried as compatibility aliases.
- Reflection-based tooling continues to use exact variable metadata on terminal properties.

## Downstream delivery constraints

- A generated variable has exactly one terminal property and one canonical navigation path.
- Generated coverage must compare the complete official name set, not only its count.
- Official phases, categories, and integration subdivisions must be represented without inference beyond the documentation.
- Library-inferred domain names must be documented as library taxonomy.
- No compatibility aliases preserve the flat prototype surface unless a published release creates that obligation.

## Exit requirements

A replacement hierarchy must preserve exact official-name coverage, distinguish documented facts from library taxonomy, and provide a deterministic migration path for consumers.

## Follow-ups and review triggers

| Item | Owner | Due date or objective trigger | Status |
| --- | --- | --- | --- |
| Reassess the hierarchy if GitLab publishes a machine-readable schema. | TedToolkit maintainers | Official schema publication | Open |
| Review taxonomy when the synchronization skill detects an unmapped variable. | TedToolkit maintainers | Synchronization failure | Open |
