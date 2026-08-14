# TedToolkit.CiContext.GitLab Benchmarks

This project measures implementation choices for typed environment-variable parsing without adding benchmark dependencies to the production package.

## Run

Run all benchmarks from the repository root in Release configuration:

```shell
dotnet run --project benchmarks/TedToolkit.CiContext.GitLab.Benchmarks/TedToolkit.CiContext.GitLab.Benchmarks.csproj -c Release
```

Pass BenchmarkDotNet filters after `--` when measuring one group or method. Generated reports are written under `BenchmarkDotNet.Artifacts` and are ignored by Git.

## Comparisons

- Generic delegate parsing versus a specialized `TryParse` path.
- Public raw environment access plus generic parsing versus direct environment access plus specialized parsing.
- Exception-factory, direct-throw, and no-inline throw-helper shapes on valid and invalid inputs.
- Cached Attribute-based enum mapping versus a hand-written switch, plus one-time map construction.
- Reference-wrapper allocation versus `CiSecret<T>` value wrapping, and initialized `Lazy<T>.Value` access versus direct static cache access.

Use the valid-input results to choose the hot-path implementation. Invalid-input results describe malformed-variable diagnostics, which are exceptional rather than ordinary control flow.

## Current Evidence

The following directional results were measured with BenchmarkDotNet 0.15.8 on .NET 10.0.11 and an Intel Core i7-12700H. `ShortRun` keeps the benchmark convenient to rerun, so these numbers are design evidence rather than a performance regression gate.

| Scenario | Mean | Allocated |
| --- | ---: | ---: |
| Generic delegate parse, valid | 21.39 ns | 0 B |
| Direct `TryParse`, valid | 18.41 ns | 0 B |
| Raw accessor and generic parse | 182.68 ns | 64 B |
| Direct environment access and parse | 172.12 ns | 64 B |
| Exception factory, valid | 13.84 ns | 0 B |
| Direct throw, valid | 10.65 ns | 0 B |
| No-inline throw helper, valid | 10.42 ns | 0 B |
| Exception factory, invalid and caught | 1.096 us | 216 B |
| Direct throw, invalid and caught | 1.256 us | 216 B |
| No-inline throw helper, invalid and caught | 1.625 us | 320 B |
| Manual enum switch with environment access | 165.9 ns | 64 B |
| Cached Attribute enum map with environment access | 165.2 ns | 64 B |
| Attribute enum-map construction | 10.46 us | 5,872 B |
| Reference secret wrapper creation | 4.374 ns | 24 B |
| `CiSecret<T>` value wrapping | 0.086 ns | 0 B |
| Initialized `Lazy<T>` map lookup | 5.212 ns | 0 B |
| Direct static map lookup | 5.330 ns | 0 B |

The environment lookup dominates typed-property access. Specializing each parser saves only a few nanoseconds before that lookup while duplicating parsing policy, so production retains named common parsing methods. The cached Attribute enum map measured equivalently to the manual switch and added no steady-state allocation; its roughly 10-microsecond, 5.7-KiB construction cost is paid once per enum type by closed-generic static initialization. Initialized lazy and direct map reads were indistinguishable in this short run, so direct static initialization is selected for simpler ownership and to avoid an unnecessary `Lazy<T>` object, not as a claimed lookup-speed improvement. The readonly secret wrapper eliminated the reference wrapper's measured 24-byte allocation; its sub-nanosecond timing was indistinguishable from benchmark overhead. In the valid-input disassembly, the exception-factory caller was 113 bytes while direct exception construction expanded the caller to 158 bytes. The factory is called only from the failure branch and was the fastest invalid-input shape. Production therefore retains an explicitly named exception factory and does not use a no-inline throwing helper.

Rerun the benchmarks before relying on these values for different hardware, runtimes, or JIT versions.

## Related Documentation

- [Repository overview](../../README.md)
- [Variable API architecture](../../docs/architecture/gitlab-ci-variable-api.md)
