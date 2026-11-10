---
name: read-typed-ci-environment
description: >-
  Use TedToolkit.CiContext to read typed CI variables from GitHub Actions, GitLab CI/CD, or the process environment.
---

# Read Typed CI Environment

Identify the CI provider and decide whether the value is predefined or custom. Provider APIs expose their own vocabulary and availability rules.

## Read and validate a value

1. Use the GitHub Actions or GitLab typed accessor for a known predefined value. See [provider accessors](references/providers.md) for the correct namespaces and availability checks.
2. Use `CiEnvironment.GetRawValue("CUSTOM_CI_VALUE")` from `TedToolkit.Ci` when preserving a custom or malformed value. Use the typed reader when the variable has a known contract.
3. Treat a missing value as `null`. A malformed typed value can throw `FormatException`. Read into a local once if several operations need the same snapshot, since each access reads the current process environment.
4. Keep credentials as `CiSecret<T>` until passing them to an API that needs the raw value. Its ordinary string output is redacted. Exercise missing, malformed, and present values without logging the contents.
