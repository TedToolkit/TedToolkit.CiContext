// -----------------------------------------------------------------------
// <copyright file="ParsingBenchmarks.cs" company="TedToolkit">
// Copyright (c) TedToolkit. All rights reserved.
// Licensed under the LGPL-3.0 license. See COPYING, COPYING.LESSER file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------------

using System.Globalization;
using System.Runtime.CompilerServices;

using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Columns;
using BenchmarkDotNet.Jobs;

using TedToolkit.Ci;

namespace TedToolkit.GitLab.Benchmarks;

[MemoryDiagnoser]
[DisassemblyDiagnoser(maxDepth: 3)]
[RankColumn]
[ShortRunJob(RuntimeMoniker.Net10_0)]
public class ParsingBenchmarks
{
    private const string EnvironmentVariableName = "TEDTOOLKIT_GITLABCI_BENCHMARK_ID";

    private const string InvalidText = "not-an-integer";

    private const string ValidInt32Text = "2147483646";

    private const string ValidText = "9223372036854775806";

    private static readonly Func<string, long?> IntegerParser = static text => long.TryParse(
        text,
        NumberStyles.Integer,
        CultureInfo.InvariantCulture,
        out var result) ? result : null;

    private string? _originalEnvironmentValue;

    [GlobalSetup]
    public void SetEnvironmentVariable()
    {
        _originalEnvironmentValue = Environment.GetEnvironmentVariable(EnvironmentVariableName);
        Environment.SetEnvironmentVariable(EnvironmentVariableName, ValidText);
    }

    [GlobalCleanup]
    public void RestoreEnvironmentVariable()
    {
        Environment.SetEnvironmentVariable(EnvironmentVariableName, _originalEnvironmentValue);
    }

    [Benchmark(Baseline = true)]
    public long? GenericDelegateValid()
    {
        return ParseValue(ValidText, IntegerParser);
    }

    [Benchmark]
    public long? DirectTryParseValid()
    {
        return long.TryParse(
            ValidText,
            NumberStyles.Integer,
            CultureInfo.InvariantCulture,
            out var result) ? result : null;
    }

    [Benchmark]
    public long? PublicRawAccessAndGenericParse()
    {
        var value = CiEnvironment.GetRawValue(EnvironmentVariableName);
        return ParseValue(value, IntegerParser);
    }

    [Benchmark]
    public long? DirectEnvironmentAccessAndParse()
    {
        var value = Environment.GetEnvironmentVariable(EnvironmentVariableName);
        if (value is null)
        {
            return null;
        }

        return long.TryParse(
            value,
            NumberStyles.Integer,
            CultureInfo.InvariantCulture,
            out var result) ? result : null;
    }

    [Benchmark]
    public int FactoryValid()
    {
        return ParseWithExceptionFactory(ValidInt32Text);
    }

    [Benchmark]
    public int DirectThrowValid()
    {
        return ParseWithDirectThrow(ValidInt32Text);
    }

    [Benchmark]
    public int ThrowHelperValid()
    {
        return ParseWithThrowHelper(ValidInt32Text);
    }

    [Benchmark]
    public int FactoryInvalid()
    {
        try
        {
            return ParseWithExceptionFactory(InvalidText);
        }
        catch (FormatException)
        {
            return 0;
        }
    }

    [Benchmark]
    public int DirectThrowInvalid()
    {
        try
        {
            return ParseWithDirectThrow(InvalidText);
        }
        catch (FormatException)
        {
            return 0;
        }
    }

    [Benchmark]
    public int ThrowHelperInvalid()
    {
        try
        {
            return ParseWithThrowHelper(InvalidText);
        }
        catch (FormatException)
        {
            return 0;
        }
    }

    private static long? ParseValue(string? value, Func<string, long?> parser)
    {
        return value is null ? null : parser(value);
    }

    private static int ParseWithExceptionFactory(string value)
    {
        return int.TryParse(value, NumberStyles.Integer, CultureInfo.InvariantCulture, out var result)
            ? result
            : throw CreateInvalidValueException();
    }

    private static int ParseWithDirectThrow(string value)
    {
        return int.TryParse(value, NumberStyles.Integer, CultureInfo.InvariantCulture, out var result)
            ? result
            : throw new FormatException("The value must contain a 32-bit integer.");
    }

    private static int ParseWithThrowHelper(string value)
    {
        if (!int.TryParse(value, NumberStyles.Integer, CultureInfo.InvariantCulture, out var result))
        {
            ThrowInvalidValue();
        }

        return result;
    }

    private static FormatException CreateInvalidValueException()
    {
        return new("The value must contain a 32-bit integer.");
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    private static void ThrowInvalidValue()
    {
        throw new FormatException("The value must contain a 32-bit integer.");
    }
}