# ADR-005: Share stable CI value parsing

- Status: Accepted
- Date: 2026-08-13
- Decision owner: TedToolkit maintainers
- Decision scope: Provider-neutral metadata and typed parsing for current-process CI environment variables.
- Applicable product intent: `../product/README.md` (approved locally; no initial commit SHA exists)
- Applicable principles: None
- Supersedes: The parser-ownership and metadata-interface portions of ADR-003
- Superseded by: ADR-006 for parser ownership, filesystem dependency, enum initialization, and secret representation; ADR-009 for variable-attribute extensibility and provider metadata composition

## 📝 Clarification and decision log

| ID | Question and why it mattered | Recommended answer | User decision and source | Affected sections | Status |
| --- | --- | --- | --- | --- | --- |
| ADR-C-01 | Whether exact variable-name metadata must support non-attribute implementations determines the honest shared abstraction. | Use an abstract property attribute because all current and intended metadata is reflection metadata. | Approved by the repository owner on 2026-08-13. | Decision; consequences | Resolved |
| ADR-C-02 | Whether all typed conversions can be inferred from `T` determines the safe scope of a common parser. | Share only format-stable scalar conversions and explicitly mapped enums; retain provider-specific formats in provider packages. | Approved by the repository owner on 2026-08-13. | Decision; constraints | Resolved |
| ADR-C-03 | Attribute-driven enum mapping adds reflection, so its runtime strategy determines whether maintainability creates unacceptable access cost. | Build each enum map once, cache it by closed generic type, and require benchmark evidence against the current switches. | Approved by the repository owner on 2026-08-13. | Decision; performance constraints | Resolved |

## 📌 Decision at a glance

The common package owns attribute-based variable metadata, invariant shared scalar readers, and cached exact-string enum mapping, while provider-specific formats remain with their provider.

## 🧭 Context and decision question

Both provider packages repeat invariant integer, absolute-URI, and enum-reading infrastructure. Twelve enum parser methods currently contain 49 manually maintained switch arms. The shared metadata interface is implemented only by provider attributes and has no non-attribute consumer. The decision is whether these behaviors can move into the common package without erasing provider semantics or materially slowing live property access.

## 🎯 Decision drivers and constraints

| Type | Driver or constraint | Evidence or source | Priority |
| --- | --- | --- | --- |
| Hard constraint | Missing variables remain `null`, and malformed typed values fail without disclosing the raw value. | Product intent and current provider contracts. | Must |
| Hard constraint | GitLab and GitHub formats that differ for the same CLR type remain provider-owned. | Current boolean, duration, list, and presence-marker implementations. | Must |
| Hard constraint | The common package remains free of provider and filesystem dependencies. | ADR-003 package dependency direction. | Must |
| Decision driver | Official enum tokens have one declarative source beside their enum members. | Current 12 parser methods and 49 switch arms. | High |
| Decision driver | Reflection must not occur on every variable access. | User performance requirement. | High |

## 🔎 Options and evidence

| Option | Evidence and confidence | Meets drivers | Decisive trade-off | Outcome |
| --- | --- | --- | --- | --- |
| Retain provider-local parsers | Current implementation; documented with high confidence. | Partly | Fast switches but repeated mapping and shared conversion policy. | Rejected |
| Infer every conversion from an unconstrained `GetValue<T>` | API analysis; high confidence. | No | A CLR type cannot identify provider-specific boolean, duration, list, or path formats, and absence is ambiguous for unconstrained value types. | Rejected |
| Use runtime reflection for every enum access | API analysis; high confidence. | No | Removes switches but adds repeated reflection and allocation risk to live reads. | Rejected |
| Cache Attribute mappings once per enum type and share only stable conversions | API analysis plus existing integer-parsing benchmark; medium confidence until enum benchmark delivery. | Yes | Adds one lazy reflection cost and a cached lookup to enum access. | Selected |

## ✅ Decision

Replace the provider-neutral metadata interface with an abstract variable Attribute that owns exact-name validation and storage. Add a field Attribute for one or more exact enum tokens and a constrained generic enum reader that returns `null` for absence and rejects undocumented text. Use ordinal comparison.

Move only invariant 32-bit integer, invariant 64-bit integer, and absolute-URI environment readers into the common package. Boolean dialects, presence markers, duration units, list encodings, filesystem types, and other provider-specific conversions remain provider-owned. Do not introduce a universal unconstrained parser, mutable parser registry, or common dependency on provider packages or `TedToolkit.FileSystem`.

Enum metadata is discovered lazily once per closed enum type and retained in a generic static cache. Duplicate tokens and missing member mappings are definition errors rather than malformed environment input.

## 💡 Why this decision now

The second provider exposes concrete duplication and makes manually synchronized enum switches an avoidable maintenance boundary. A deliberately narrow common parser removes actual duplication without claiming that equal CLR types imply equal provider formats. Cached metadata pays reflection cost only when an enum type is first used; benchmark evidence determines whether the steady-state lookup is acceptable or requires generated switches later.

## 🔗 Evidence and links

- Current provider parsing implementations and maintained BenchmarkDotNet project under `benchmarks/TedToolkit.CiContext.GitLab.Benchmarks/`.
- Existing measured environment access and integer parsing results in that benchmark project's README.
- Enum comparison command: `dotnet run --project benchmarks/TedToolkit.CiContext.GitLab.Benchmarks/TedToolkit.CiContext.GitLab.Benchmarks.csproj -c Release -- --filter '*EnumParsingBenchmarks*'`.
- Measured on .NET 10.0.11, Windows 11, and an Intel Core i7-12700H: the manual switch and cached Attribute map both used 64 B per full environment access and measured 165.9 ns and 165.2 ns respectively; map construction measured 10.46 us and 5,872 B once per enum type. `ShortRun` makes this directional design evidence rather than a regression gate.

## ⚖️ Consequences and accepted trade-offs

- Provider enum definitions become the single source for official token mappings.
- Consumers can reflect all provider variable attributes through one abstract Attribute type.
- Removing the interface is a public API break accepted before the repository's initial release.
- First access to each mapped enum type performs reflection; subsequent accesses use a cached ordinal lookup.
- Common error text becomes provider-neutral while preserving the variable name, expected representation, and secret-safe behavior.

## 🛠️ Downstream delivery constraints

- All supported mapped enum members must declare at least one non-empty exact token, and duplicate tokens must be rejected.
- Missing environment variables return `null`; unknown enum tokens and malformed shared scalar values throw `FormatException` without including the raw value.
- The steady-state enum lookup must not perform reflection or add managed allocations beyond the environment access itself.
- Provider-specific parsing semantics and the existing package dependency direction must remain unchanged.
- All existing target frameworks remain supported.

## 🔄 Exit requirements

A broader generic parser requires evidence that type-only inference can preserve absence and provider-specific formats. A source generator may replace runtime map construction if benchmarks or trimming requirements show that the cached implementation is inadequate, while retaining the public Attribute contract.

## 📅 Follow-ups and review triggers

| Item | Owner | Due date or objective trigger | Status |
| --- | --- | --- | --- |
| Reassess cached lookup strategy if steady-state benchmarks show material regression against switches. | TedToolkit maintainers | Enum-parser benchmark evidence | Closed; current evidence shows equivalent steady-state time and allocation. |
| Reassess reflection metadata for NativeAOT or aggressive trimming support. | TedToolkit maintainers | First explicit NativeAOT/trimming requirement | Open |
