---
name: read-typed-ci-environment
description: >-
  Read raw or typed process environment variables with TedToolkit.CiContext when implementing a custom CI variable or provider-neutral tooling.
---

# Read a Typed CI Environment

- Use `CiEnvironment.GetRawValue` for unknown variables or when preserving the original value. Use the matching typed reader for a known contract.
- Treat a missing variable as `null` and handle `FormatException` as malformed input. Read once into a local variable if the operation needs a consistent snapshot.
- Keep credentials as `CiSecret<T>` until handing them directly to a consumer that needs the value. Exercise missing and malformed inputs without logging their raw contents.
