# ADR-008: Use policy-free common environment readers

- Status: Accepted
- Date: 2026-08-13
- Decision owner: TedToolkit maintainers
- Decision scope: Public raw and boolean environment-value access across the CI context package family.
- Applicable product intent: `../product/README.md` (approved locally; no initial commit SHA exists)
- Applicable principles: None
- Supersedes: The caller-selected boolean-format portion of ADR-006 and the GitHub provider raw-access portion of ADR-003
- Superseded by: None

## 📝 Clarification and decision log

| ID | Question and why it mattered | Recommended answer | User decision and source | Affected sections | Status |
| --- | --- | --- | --- | --- | --- |
| ADR-C-01 | Whether callers need to select a boolean spelling set determines the public parsing surface. | Accept the exact union of common CI boolean tokens through one reader. | Approved by the repository owner on 2026-08-13. | Decision; consequences | Resolved |
| ADR-C-02 | Whether a provider root should forward raw access determines ownership and API duplication. | Keep raw access only on the common environment boundary for new providers. | Approved by the repository owner on 2026-08-13. | Decision; compatibility | Resolved |

## 📌 Decision at a glance

The common environment boundary accepts the exact common CI boolean-token union without a format selector, and provider roots do not add semantically identical raw-access forwarders.

## 🧭 Context and decision question

`CiBooleanFormat` required provider properties and common-package consumers to select between overlapping token sets, although accepting their finite union is unambiguous. The GitHub provider also forwarded raw reads to `CiEnvironment` without adding provider semantics. Both contracts exposed ownership plumbing instead of domain behavior.

## 🎯 Decision drivers and constraints

| Type | Driver or constraint | Evidence or source | Priority |
| --- | --- | --- | --- |
| Hard constraint | Missing values return `null`; malformed values fail without disclosing their text. | Product intent and active package-family architecture. | Must |
| Hard constraint | Exact presence markers retain their own stricter reader. | GitHub documents `RUNNER_DEBUG` as absent or exactly `1`. | Must |
| Decision driver | Shared value conversion has one discoverable entry point. | `CiEnvironment` already owns every reusable parser. | High |
| Decision driver | Public APIs represent semantic capability rather than forwarding or dialect-selection mechanics. | Current source inspection. | High |

## 🔎 Options and evidence

| Option | Evidence and confidence | Meets drivers | Decisive trade-off | Outcome |
| --- | --- | --- | --- | --- |
| Retain `CiBooleanFormat` and provider raw forwarders | Documented current API. High confidence. | Preserves strict provider spellings but duplicates concepts and call paths. | More public policy than consumers need. | Rejected |
| Use only `bool.TryParse` | .NET behavior is documented. High confidence. | Removes the selector but rejects documented CI spellings such as `yes`, `no`, `1`, and `0`. | Loses provider compatibility. | Rejected |
| Accept the exact token union and centralize raw access | Current provider contracts and official GitHub reference. High confidence. | Preserves all supported values with one parsing and raw-access boundary. | GitHub boolean properties become deliberately tolerant of the additional exact tokens. | Selected |

## ✅ Decision

`CiEnvironment.GetBoolean(string)` accepts `true`, `false`, `yes`, and `no` without regard to letter case, plus exact `1` and `0`. No other numeric or textual value is accepted. Exact presence markers continue to use `GetPresenceMarker`.

Raw, custom, future, or malformed values are read through `CiEnvironment.GetRawValue`. New provider roots do not repeat that method. The pre-release GitHub provider removes its forwarding member; the existing GitLab forwarding surface is not part of this decision's delivery scope.

## 💡 Why this decision now

The accepted token union is finite and unambiguous, so a caller-selected dialect adds cognitive and compatibility surface without improving conversion correctness. Central raw access follows the package dependency direction: the provider-neutral foundation owns provider-neutral behavior, while provider roots expose only provider semantics.

## 🔗 Evidence and links

- [GitHub's official variables reference](https://docs.github.com/en/actions/reference/workflows-and-actions/variables) documents its default environment variables and exact `RUNNER_DEBUG` marker.
- [GitHub's maintained variables Markdown](https://raw.githubusercontent.com/github/docs/main/content/actions/reference/workflows-and-actions/variables.md) provides the secondary structured source.

## ⚖️ Consequences and accepted trade-offs

- Consumers have one boolean overload and no `CiBooleanFormat` type.
- The GitHub provider root loses its redundant pre-release `GetRawValue` method; callers use the common package directly.
- GitHub boolean properties accept valid common CI spellings beyond the values GitHub currently emits. This is deliberate tolerant input handling, not a claim that GitHub emits every spelling.
- Strict markers remain strict, so `RUNNER_DEBUG=true` is still malformed.

## 🛠️ Downstream delivery constraints

- Preserve live process reads, nullable absence, and secret-safe diagnostics.
- Accept only the six documented common tokens; do not treat arbitrary nonzero integers as true.
- Preserve `GetPresenceMarker` and all official GitHub variable paths, types, and metadata.
- Keep every supported target framework and a zero-warning Release build.

## 🔄 Exit requirements

Introduce a selectable format only if a future protocol uses conflicting meanings or strict validation becomes an observable consumer requirement. Add a provider raw accessor only when it contributes provider-specific behavior.

## 📅 Follow-ups and review triggers

| Item | Owner | Due date or objective trigger | Status |
| --- | --- | --- | --- |
| Reassess the token union. | TedToolkit maintainers | A supported provider documents a conflicting boolean token. | Open |
