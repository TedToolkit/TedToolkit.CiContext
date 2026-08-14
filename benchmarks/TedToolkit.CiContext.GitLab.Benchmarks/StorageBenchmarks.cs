// -----------------------------------------------------------------------
// <copyright file="StorageBenchmarks.cs" company="TedToolkit">
// Copyright (c) TedToolkit. All rights reserved.
// Licensed under the LGPL-3.0 license. See COPYING, COPYING.LESSER file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------------

using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Columns;
using BenchmarkDotNet.Jobs;

using TedToolkit.Ci;

namespace TedToolkit.GitLab.Benchmarks;

[MemoryDiagnoser]
[RankColumn]
[ShortRunJob(RuntimeMoniker.Net10_0)]
public class StorageBenchmarks
{
    private const string Value = "glpat-1234567890";

    private static readonly Dictionary<string, int> DirectMap = CreateMap();

    private static readonly Lazy<Dictionary<string, int>> LazyMap = new(CreateMap);

    [GlobalSetup]
    public void InitializeLazyMap()
    {
        _ = LazyMap.Value;
    }

    [Benchmark(Baseline = true)]
    public ReferenceSecret<string> AllocateReferenceWrapper()
    {
        return new(Value);
    }

    [Benchmark]
    public CiSecret<string> CreateValueWrapper()
    {
        return Value;
    }

    [Benchmark]
    public int ReadLazyMap()
    {
        return LazyMap.Value[Value];
    }

    [Benchmark]
    public int ReadDirectMap()
    {
        return DirectMap[Value];
    }

    private static Dictionary<string, int> CreateMap()
    {
        return new(StringComparer.Ordinal) { [Value] = 42, };
    }
}