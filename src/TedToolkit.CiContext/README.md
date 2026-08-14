# TedToolkit.CiContext

`TedToolkit.CiContext` supplies the provider-neutral primitives shared by the CI provider packages.

## Install

```shell
dotnet add package TedToolkit.CiContext
```

## Raw Environment Access

```csharp
using TedToolkit.Ci;

string? value = CiEnvironment.GetRawValue("CUSTOM_CI_VALUE");
```

`GetRawValue` reads the current process environment on every call. It returns `null` when the variable is absent and rejects a null or empty name.

## Stable Typed Values

The common package owns the reusable environment-value formats used by provider packages:

```csharp
int? attempt = CiEnvironment.GetInt32("BUILD_ATTEMPT");
long? runId = CiEnvironment.GetInt64("BUILD_RUN_ID");
Uri? server = CiEnvironment.GetAbsoluteUri("CI_SERVER_URL");
bool? enabled = CiEnvironment.GetBoolean("CI_FEATURE");
DateTimeOffset? startedAt = CiEnvironment.GetDateTimeOffset("CI_STARTED_AT");
TimeSpan? timeout = CiEnvironment.GetTimeSpanFromSeconds("CI_TIMEOUT");
DirectoryPath? workspace = CiEnvironment.GetDirectoryPath("CI_WORKSPACE");
```

`GetBoolean` accepts `true`, `false`, `yes`, and `no` without regard to letter case, plus exact `1` and `0`. Missing values return `null`. Malformed values throw `FormatException` without including their original text.

Map a documented closed string set directly on its enum members:

```csharp
public enum DeploymentTier
{
    [CiEnumValue("staging")]
    Staging,

    [CiEnumValue("production")]
    Production,
}

DeploymentTier? tier = CiEnvironment.GetEnumValue<DeploymentTier>("DEPLOYMENT_TIER");
```

Enum matching is ordinal and case-sensitive. Mapping metadata is initialized once by each closed generic cache and reused for later reads; no extra `Lazy<T>` layer is used. Every member must have at least one mapping, and one enum type cannot map the same string to multiple members.

## Redacting Sensitive Values

```csharp
using TedToolkit.Ci;

CiSecret<string> secret = "glpat-1234567890";

Console.WriteLine(secret);       // [REDACTED]7890
string value = secret.Reveal(); // glpat-1234567890
```

`CiSecret<T>` is a `readonly struct`, so implicit conversion does not allocate a wrapper object. `ToString()` calls the wrapped value's own `ToString()` and shows only the last four characters when its representation is at least 12 characters long. Shorter values and an uninitialized default wrapper return `[REDACTED]`. The type is not encryption and does not protect values passed to serializers, reflection, logging destructurers, or code that calls `Reveal()`.

## Variable Metadata

Provider properties use the sealed `CiVariableAttribute` directly, whose `Name` property exposes the exact environment-variable name. Provider-specific facts use separate attributes. The common package deliberately does not define a normalized provider context. Providers select the appropriate named reader, such as an exact presence marker or an explicit duration unit.

## Target Frameworks

`net6.0`, `net7.0`, `net8.0`, `net9.0`, `net10.0`, `net472`, `net48`, `netstandard2.0`, and `netstandard2.1`.

## Related Documentation

- [Repository overview](../../README.md)
- [Package-family architecture](../../docs/architecture/ci-context-package-family.md)

## License

Licensed under LGPL-3.0.
