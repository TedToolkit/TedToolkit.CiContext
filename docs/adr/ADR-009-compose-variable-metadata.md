# ADR-009: Compose variable metadata

- Status: Accepted
- Date: 2026-08-13
- Decision owner: TedToolkit maintainers
- Decision scope: Exact-name and provider-specific metadata on CI variable properties.
- Applicable product intent: `../product/README.md` (approved locally; no initial commit SHA exists)
- Applicable principles: None
- Supersedes: The variable-attribute extensibility portion of ADR-005
- Superseded by: None

## 📝 Clarification and decision log

| ID | Question and why it mattered | Recommended answer | User decision and source | Affected sections | Status |
| --- | --- | --- | --- | --- | --- |
| ADR-C-01 | Whether provider attributes should inherit the exact-name contract determines whether the common Attribute must remain extensible. | Apply one sealed common name Attribute directly and compose separate provider metadata. | Approved by the repository owner on 2026-08-13. | Decision; consequences | Resolved |

## 📌 Decision at a glance

Every terminal CI variable property directly carries the sealed `CiVariableAttribute`. Provider-specific facts use separate sealed attributes; GitLab categories use `GitLabCiVariableCategoryAttribute`.

## 🧭 Context and decision question

GitHub needed no metadata beyond the exact environment name, so `GitHubActionsVariableAttribute` was an empty wrapper. GitLab combined the shared name and its documentation category through inheritance. Keeping `CiVariableAttribute` extensible solely for these wrappers enlarged the public model and triggered the .NET unsealed-Attribute diagnostic.

## 🔎 Options and evidence

| Option | Meets the contract | Decisive trade-off | Outcome |
| --- | --- | --- | --- |
| Keep an abstract common Attribute and provider subclasses | Yes | Requires an empty GitHub type and binds unrelated GitLab facts into inheritance. | Rejected |
| Make the common Attribute concrete and extensible | Yes | Preserves unnecessary inheritance and requires suppressing the unsealed-Attribute diagnostic. | Rejected |
| Seal the common Attribute and compose provider metadata | Yes | GitLab reflection reads two attributes instead of one. | Selected |

## ✅ Decision

`CiVariableAttribute` is sealed, validates the exact name, and is applied directly to every provider terminal property. Do not add provider-specific subclasses or empty aliases.

GitLab declares its official documentation category independently with the sealed `GitLabCiVariableCategoryAttribute`. Availability remains class-level metadata through `GitLabCiVariableAvailabilityAttribute`. All Attribute source files remain in each project's `Attributes/` directory.

## ⚖️ Consequences and accepted trade-offs

- Cross-provider discovery depends on one exact concrete Attribute contract.
- Provider-specific metadata can evolve independently without weakening the common type.
- The empty GitHub wrapper and combined GitLab wrapper are removed before initial release.
- Attribute construction occurs only during reflection; normal variable reads have no additional runtime cost.

## 🛠️ Downstream delivery constraints

- Preserve official variable sets, navigation, types, parsing, availability, and secret handling.
- Prove that every GitHub variable has one common name Attribute.
- Prove that every GitLab variable has one common name Attribute and one category Attribute.
- Keep a zero-warning multi-target Release build without analyzer suppression.

## 🔄 Exit requirements

Introduce another metadata Attribute only when it represents an independent provider fact used by consumers or synchronization tooling. Do not create provider wrappers for naming alone.
