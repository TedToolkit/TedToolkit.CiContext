# ADR-007: Use automatic secret identification

- Status: Accepted
- Date: 2026-08-13
- Decision owner: TedToolkit maintainers
- Decision scope: Ordinary string formatting for `CiSecret<T>`.
- Applicable product intent: `../product/README.md` (approved locally; no initial commit SHA exists)
- Applicable principles: None
- Supersedes: The configurable secret-mask portion of ADR-006
- Superseded by: None

## 📝 Clarification and decision log

| ID | Question and why it mattered | Recommended answer | User decision and source | Affected sections | Status |
| --- | --- | --- | --- | --- | --- |
| ADR-C-01 | Whether callers need configurable masks determines the public API size and consistency of accidental formatting. | Remove `CiSecretMask`; use one automatic suffix-only rule. | Approved by the repository owner on 2026-08-13. | Decision; consequences | Resolved |
| ADR-C-02 | Which fragment can identify a generated secret without explaining its structure determines the disclosure rule. | Show the final four characters only when at least eight characters remain hidden. | Delegated to and selected by the maintainer on 2026-08-13. | Decision; security constraints | Resolved |
| ADR-C-03 | Whether generic formatting needs invariant conversion determines complexity and possible boxing. | Call the wrapped value's own `ToString()` directly because the wrapper masks an opaque representation rather than serializing a canonical value. | Approved by the repository owner on 2026-08-13. | Decision; consequences | Resolved |

## 📌 Decision at a glance

`CiSecret<T>.ToString()` automatically returns `[REDACTED]` for representations shorter than 12 characters and `[REDACTED]` followed by the final four characters otherwise. No configurable masking API exists.

## 🧭 Context and decision question

Configurable prefix, suffix, and edge masks made every caller responsible for a security-display policy even though CI credentials are normally generated and need only a stable human identifier. The decision is whether that flexibility creates enough value to justify its public concepts and inconsistent output.

## 🎯 Decision drivers and constraints

| Type | Driver or constraint | Priority |
| --- | --- | --- |
| Hard constraint | Ordinary formatting never exposes a complete secret. | Must |
| Hard constraint | Partial output leaves at least eight characters hidden. | Must |
| Decision driver | A holder can distinguish generated credentials without learning their structure. | High |
| Decision driver | Callers need no display-policy knowledge or configuration. | High |

## 🔎 Options and evidence

| Option | Meets drivers | Decisive trade-off | Outcome |
| --- | --- | --- | --- |
| Keep configurable masks | Partly | Flexible, but expands the API and permits inconsistent disclosure choices. | Rejected |
| Show prefix and suffix automatically | Partly | Identifies token families but reveals more structure than credential disambiguation requires. | Rejected |
| Show only the final four characters above a safe length | Yes | One fixed rule; property context identifies the credential kind while the suffix distinguishes instances. | Selected |

## ✅ Decision

Remove configurable secret-display policies. Obtain the opaque formatted representation from `T.ToString()` directly. If it contains fewer than 12 characters, return `[REDACTED]`; otherwise append its final four characters to `[REDACTED]`. This threshold guarantees that a partial representation hides at least eight characters.

The direct virtual/constrained call represents the wrapped type's own ordinary text and avoids routing generic value types through `Convert.ToString(object, IFormatProvider)`. Culture-invariant canonical serialization is not a responsibility of secret masking.

## ⚖️ Consequences and accepted trade-offs

- `CiSecretMask`, the masking constructor overload, and `WithMask` are removed before initial release.
- Every ordinary formatting path produces the same predictable representation.
- Short secrets cannot be distinguished through formatting and require explicit `Reveal()` where authorized.
- The final four characters disclose a small stable identifier for sufficiently long generated secrets.

## 🛠️ Downstream delivery constraints

- Preserve safe behavior for `default(CiSecret<T>)` and a null constructor argument.
- Test the 11/12-character boundary and interpolated formatting.
- Do not add an automatic prefix or a caller-configurable override without a superseding decision.

## 🔄 Exit requirements and review triggers

Revisit the rule if an official provider documents credentials shorter than 12 characters that require identification, or if a security policy prohibits displaying any credential fragment.
