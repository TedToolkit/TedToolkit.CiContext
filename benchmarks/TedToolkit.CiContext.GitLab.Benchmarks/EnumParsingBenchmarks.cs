// -----------------------------------------------------------------------
// <copyright file="EnumParsingBenchmarks.cs" company="TedToolkit">
// Copyright (c) TedToolkit. All rights reserved.
// Licensed under the LGPL-3.0 license. See COPYING, COPYING.LESSER file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------------

using System.Reflection;

using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Columns;
using BenchmarkDotNet.Jobs;

using TedToolkit.Ci;

namespace TedToolkit.GitLab.Benchmarks;

[MemoryDiagnoser]
[RankColumn]
[ShortRunJob(RuntimeMoniker.Net10_0)]
public class EnumParsingBenchmarks
{
    private const string EnvironmentVariableName = "TEDTOOLKIT_CI_CONTEXT_BENCHMARK_ENUM";

    private string? _originalEnvironmentValue;

    [GlobalSetup]
    public void SetEnvironmentVariable()
    {
        _originalEnvironmentValue = Environment.GetEnvironmentVariable(EnvironmentVariableName);
        Environment.SetEnvironmentVariable(EnvironmentVariableName, "merge_request_event");
        _ = CiEnvironment.GetEnumValue<GitLabPipelineSource>(EnvironmentVariableName);
    }

    [GlobalCleanup]
    public void RestoreEnvironmentVariable()
    {
        Environment.SetEnvironmentVariable(EnvironmentVariableName, _originalEnvironmentValue);
    }

    [Benchmark(Baseline = true)]
    public GitLabPipelineSource? ManualSwitch()
    {
        return Environment.GetEnvironmentVariable(EnvironmentVariableName) switch
        {
            "api" => GitLabPipelineSource.API,
            "chat" => GitLabPipelineSource.CHAT,
            "external" => GitLabPipelineSource.EXTERNAL,
            "external_pull_request_event" => GitLabPipelineSource.EXTERNAL_PULL_REQUEST_EVENT,
            "merge_request_event" => GitLabPipelineSource.MERGE_REQUEST_EVENT,
            "ondemand_dast_scan" => GitLabPipelineSource.ON_DEMAND_DAST_SCAN,
            "ondemand_dast_validation" => GitLabPipelineSource.ON_DEMAND_DAST_VALIDATION,
            "parent_pipeline" => GitLabPipelineSource.PARENT_PIPELINE,
            "pipeline" => GitLabPipelineSource.PIPELINE,
            "push" => GitLabPipelineSource.PUSH,
            "schedule" => GitLabPipelineSource.SCHEDULE,
            "security_orchestration_policy" => GitLabPipelineSource.SECURITY_ORCHESTRATION_POLICY,
            "trigger" => GitLabPipelineSource.TRIGGER,
            "web" => GitLabPipelineSource.WEB,
            "webide" => GitLabPipelineSource.WEB_IDE,
            _ => null,
        };
    }

    [Benchmark]
    public GitLabPipelineSource? CachedAttributeMap()
    {
        return CiEnvironment.GetEnumValue<GitLabPipelineSource>(EnvironmentVariableName);
    }

    [Benchmark]
    public Dictionary<string, GitLabPipelineSource> AttributeMapConstruction()
    {
        var values = new Dictionary<string, GitLabPipelineSource>(StringComparer.Ordinal);
        foreach (var field in typeof(GitLabPipelineSource).GetFields(BindingFlags.Public | BindingFlags.Static))
        {
            var enumValue = (GitLabPipelineSource)field.GetValue(null)!;
            foreach (var attribute in field.GetCustomAttributes<CiEnumValueAttribute>())
            {
                values.Add(attribute.Value, enumValue);
            }
        }

        return values;
    }
}