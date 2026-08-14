// -----------------------------------------------------------------------
// <copyright file="GitLabCiContractTests.cs" company="TedToolkit">
// Copyright (c) TedToolkit. All rights reserved.
// Licensed under the LGPL-3.0 license. See COPYING, COPYING.LESSER file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------------

using System.Globalization;
using System.Reflection;

using TedToolkit.FileSystem;
using TedToolkit.GitLab;
using TedToolkit.GitLab.Tests;

namespace TedToolkit.CiContext.GitLab.Tests;

internal sealed class GitLabCiContractTests
{
    private const string PROCESS_ENVIRONMENT = "ProcessEnvironment";

    private static readonly string[] _expectedRunnerTags = ["linux", "docker",];

    private static readonly string[] _expectedSecretVariables =
    [
        "APP_STORE_CONNECT_API_KEY_KEY",
        "CI_DEPENDENCY_PROXY_PASSWORD",
        "CI_DEPLOY_PASSWORD",
        "CI_JOB_TOKEN",
        "CI_REGISTRY_PASSWORD",
        "CI_REPOSITORY_URL",
        "CI_RUNNER_SHORT_TOKEN",
        "DIFFBLUE_ACCESS_TOKEN",
        "DIFFBLUE_LICENSE_KEY",
        "HARBOR_PASSWORD",
        "SUPPLY_JSON_KEY_DATA",
    ];

    /// <summary>
    /// Verifies the GitLab provider assembly identity and exact official-variable coverage.
    /// </summary>
    [Test]
    public async Task Should_expose_the_exact_official_variable_set_once()
    {
        var metadata = GetVariableProperties()
            .Select<PropertyInfo, string>(property => property.GetCustomAttribute<CiVariableAttribute>()!.Name)
            .Order(StringComparer.Ordinal)
            .ToArray();
        var expected = OfficialVariableNames.All.Order(StringComparer.Ordinal).ToArray();

        await Assert.That(typeof(GitLabCi).IsAbstract && typeof(GitLabCi).IsSealed).IsTrue();
        await Assert.That(typeof(GitLabCi).Assembly.GetName().Name).IsEqualTo("TedToolkit.CiContext.GitLab");
        await Assert.That(metadata.Length).IsEqualTo(expected.Length);
        await Assert.That(metadata.Distinct(StringComparer.Ordinal).Count()).IsEqualTo(expected.Length);
        await Assert.That(metadata.SequenceEqual(expected, StringComparer.Ordinal)).IsTrue();
        await Assert.That(GetVariableTypes().All(type => type.IsAbstract && type.IsSealed)).IsTrue();
        await Assert.That(typeof(GitLabCi).Assembly.GetType("TedToolkit.GitLab.GitLabCiVariableAttribute")).IsNull();
    }

    /// <summary>
    /// Verifies that official authentication-capable and credential values have the secret contract exactly.
    /// </summary>
    [Test]
    public async Task Should_protect_the_exact_official_sensitive_value_set()
    {
        var actual = GetVariableProperties()
            .Where(static property => property.PropertyType == typeof(CiSecret<string>?))
            .Select<PropertyInfo, string>(static property => property.GetCustomAttribute<CiVariableAttribute>()!.Name)
            .Order(StringComparer.Ordinal)
            .ToArray();
        var expected = _expectedSecretVariables.Order(StringComparer.Ordinal).ToArray();

        await Assert.That(actual).IsEquivalentTo(expected);
        await Assert.That(typeof(GitLabCi.JobOnly.Trigger).GetProperty(nameof(GitLabCi.JobOnly.Trigger.ShortToken))!.PropertyType)
            .IsEqualTo(typeof(string));
    }

    /// <summary>
    /// Verifies typed conversion through the static phase-first domain hierarchy.
    /// </summary>
    [Test]
    [NotInParallel(PROCESS_ENVIRONMENT)]
    public async Task Should_convert_representative_hierarchical_values()
    {
        using var environment = new EnvironmentVariableScope(new Dictionary<string, string?>()
        {
            ["CI"] = "true",
            ["CI_JOB_ID"] = "9223372036854775806",
            ["CI_JOB_RETRY_COUNT"] = "2",
            ["CI_JOB_TIMEOUT"] = "90",
            ["CI_COMMIT_TIMESTAMP"] = "2026-08-13T10:20:30Z",
            ["CI_PROJECT_URL"] = "https://gitlab.example.com/group/project",
            ["CI_PROJECT_DIR"] = "/builds/group/project",
            ["CI_SERVER_TLS_CA_FILE"] = "/certs/ca.pem",
            ["CI_RUNNER_TAGS"] = "[\"linux\",\"docker\"]",
            ["CI_PIPELINE_SOURCE"] = "merge_request_event",
            ["CI_PIPELINE_IID"] = "42",
            ["CI_ENVIRONMENT_ACTION"] = "verify",
            ["CI_ENVIRONMENT_TIER"] = "staging",
            ["CI_JOB_STATUS"] = "canceled",
            ["CI_PROJECT_VISIBILITY"] = "private",
            ["CI_MERGE_REQUEST_EVENT_TYPE"] = "merge_train",
            ["CI_SERVER_PROTOCOL"] = "https",
            ["INCREMENTAL_ROLLOUT_MODE"] = "timed",
            ["CI_JOB_TOKEN"] = "top-secret",
            ["CI_RUNNER_SHORT_TOKEN"] = "glrt-auth-fragment",
            ["CUSTOM_VARIABLE"] = "preserved",
        });
        var expectedTimestamp = DateTimeOffset.Parse(
            "2026-08-13T10:20:30Z",
            CultureInfo.InvariantCulture);

        await Assert.That(GitLabCi.PrePipeline.General.Detected).IsTrue();
        await Assert.That(GitLabCi.JobOnly.Job.Id).IsEqualTo(9223372036854775806L);
        await Assert.That(GitLabCi.JobOnly.Job.RetryCount).IsEqualTo(2);
        await Assert.That(GitLabCi.JobOnly.Job.Timeout).IsEqualTo(TimeSpan.FromSeconds(90));
        await Assert.That(GitLabCi.PrePipeline.Commit.Timestamp).IsEqualTo(expectedTimestamp);
        await Assert.That(GitLabCi.PrePipeline.Project.Url).IsEqualTo(new Uri("https://gitlab.example.com/group/project"));
        await Assert.That(GitLabCi.JobOnly.Project.Dir).IsEqualTo(new DirectoryPath("/builds/group/project"));
        await Assert.That(GitLabCi.Pipeline.Server.TlsCaFile).IsEqualTo(new FilePath("/certs/ca.pem"));
        await Assert.That(GitLabCi.JobOnly.Runner.Tags).IsEquivalentTo(_expectedRunnerTags);
        await Assert.That(GitLabCi.PrePipeline.Pipeline.Source).IsEqualTo(GitLabPipelineSource.MERGE_REQUEST_EVENT);
        await Assert.That(GitLabCi.Pipeline.Iid).IsEqualTo(42L);
        await Assert.That(GitLabCi.Pipeline.Environment.Action).IsEqualTo(GitLabEnvironmentAction.VERIFY);
        await Assert.That(GitLabCi.Pipeline.Environment.Tier).IsEqualTo(GitLabDeploymentTier.STAGING);
        await Assert.That(GitLabCi.JobOnly.Job.Status).IsEqualTo(GitLabJobStatus.CANCELED);
        await Assert.That(GitLabCi.PrePipeline.Project.Visibility).IsEqualTo(GitLabProjectVisibility.PRIVATE);
        await Assert.That(GitLabCi.PrePipeline.MergeRequest.EventType).IsEqualTo(GitLabMergeRequestEventType.MERGE_TRAIN);
        await Assert.That(GitLabCi.PrePipeline.Server.Protocol).IsEqualTo(GitLabServerProtocol.HTTPS);
        await Assert.That(GitLabCi.PrePipeline.AutoDevOps.IncrementalRolloutMode)
            .IsEqualTo(GitLabIncrementalRolloutMode.TIMED);
        await Assert.That(GitLabCi.JobOnly.Job.Token!.ToString()).IsEqualTo("[REDACTED]");
        await Assert.That(GitLabCi.JobOnly.Job.Token!.Value.Reveal()).IsEqualTo("top-secret");
        await Assert.That(typeof(GitLabCi.JobOnly.Job).GetProperty(nameof(GitLabCi.JobOnly.Job.Token))!.PropertyType)
            .IsEqualTo(typeof(CiSecret<string>?));
        await Assert.That(GitLabCi.JobOnly.Runner.ShortToken!.Value.ToString()).IsEqualTo("[REDACTED]ment");
        await Assert.That(GitLabCi.JobOnly.Runner.ShortToken!.Value.Reveal()).IsEqualTo("glrt-auth-fragment");
        await Assert.That(typeof(GitLabCi.JobOnly.Runner).GetProperty(nameof(GitLabCi.JobOnly.Runner.ShortToken))!.PropertyType)
            .IsEqualTo(typeof(CiSecret<string>?));
        await Assert.That(CiEnvironment.GetRawValue("CUSTOM_VARIABLE")).IsEqualTo("preserved");
    }

    /// <summary>
    /// Verifies official scenario and integration relationships in the static API.
    /// </summary>
    [Test]
    [NotInParallel(PROCESS_ENVIRONMENT)]
    public async Task Should_preserve_scenario_and_integration_hierarchy()
    {
        using var environment = new EnvironmentVariableScope(new Dictionary<string, string?>()
        {
            ["CI_MERGE_REQUEST_SOURCE_BRANCH_NAME"] = "feature/api",
            ["CI_EXTERNAL_PULL_REQUEST_TARGET_BRANCH_SHA"] = "abc123",
            ["AUTO_DEVOPS_EXPLICITLY_ENABLED"] = "1",
            ["HARBOR_URL"] = "https://harbor.example.com",
            ["APP_STORE_CONNECT_API_KEY_ISSUER_ID"] = "opaque-issuer-id",
        });

        await Assert.That(GitLabCi.PrePipeline.MergeRequest.Source.BranchName).IsEqualTo("feature/api");
        await Assert.That(GitLabCi.ExternalPullRequest.Target.BranchSha).IsEqualTo("abc123");
        await Assert.That(GitLabCi.PrePipeline.AutoDevOps.ExplicitlyEnabled).IsTrue();
        await Assert.That(GitLabCi.JobOnly.Integrations.Harbor.Url).IsEqualTo(new Uri("https://harbor.example.com"));
        await Assert.That(GitLabCi.JobOnly.Integrations.AppStoreConnect.ApiKeyIssuerId).IsEqualTo("opaque-issuer-id");
    }

    /// <summary>
    /// Verifies that missing variables remain null and malformed current values are rejected.
    /// </summary>
    [Test]
    [NotInParallel(PROCESS_ENVIRONMENT)]
    public async Task Should_distinguish_absent_and_malformed_values()
    {
        using var environment = new EnvironmentVariableScope(
            new Dictionary<string, string?>()
            {
                ["CI_PROJECT_ID"] = null,
                ["CI_PIPELINE_SOURCE"] = null,
            });

        await Assert.That(GitLabCi.PrePipeline.Project.Id).IsNull();
        await Assert.That(GitLabCi.PrePipeline.Pipeline.Source).IsNull();

        Environment.SetEnvironmentVariable("CI_PROJECT_ID", "not-an-id");
        Environment.SetEnvironmentVariable("CI_PIPELINE_SOURCE", "not-a-source");

        FormatException? exception = null;
        try
        {
            _ = GitLabCi.PrePipeline.Project.Id;
        }
        catch (FormatException caughtException)
        {
            exception = caughtException;
        }

        await Assert.That(exception).IsNotNull();
        await Assert.That(exception!.Message.Contains("not-an-id", StringComparison.Ordinal)).IsFalse();
        await Assert.That(() => GitLabCi.PrePipeline.Pipeline.Source).Throws<FormatException>();
        await Assert.That(CiEnvironment.GetRawValue("CI_PROJECT_ID")).IsEqualTo("not-an-id");
    }

    /// <summary>
    /// Verifies that every property access observes the current process environment value.
    /// </summary>
    [Test]
    [NotInParallel(PROCESS_ENVIRONMENT)]
    public async Task Should_read_the_environment_on_every_access()
    {
        using var environment = new EnvironmentVariableScope(
            new Dictionary<string, string?>() { ["CI_COMMIT_SHA"] = "before", });

        await Assert.That(GitLabCi.PrePipeline.Commit.Sha).IsEqualTo("before");

        Environment.SetEnvironmentVariable("CI_COMMIT_SHA", "after");

        await Assert.That(GitLabCi.PrePipeline.Commit.Sha).IsEqualTo("after");
    }

    private static IEnumerable<PropertyInfo> GetVariableProperties()
    {
        return GetVariableTypes()
            .SelectMany(type => type.GetProperties(BindingFlags.Static | BindingFlags.Public | BindingFlags.DeclaredOnly))
            .Where(property => property.IsDefined(typeof(CiVariableAttribute), false));
    }

    private static IEnumerable<Type> GetVariableTypes()
    {
        return GetNestedTypes(typeof(GitLabCi));
    }

    private static IEnumerable<Type> GetNestedTypes(Type type)
    {
        foreach (var nestedType in type.GetNestedTypes(BindingFlags.Public))
        {
            yield return nestedType;

            foreach (var descendant in GetNestedTypes(nestedType))
            {
                yield return descendant;
            }
        }
    }

    private sealed class EnvironmentVariableScope : IDisposable
    {
        private readonly IReadOnlyDictionary<string, string?> _originalValues;

        internal EnvironmentVariableScope(IReadOnlyDictionary<string, string?> values)
        {
            _originalValues = values.Keys.ToDictionary(
                static name => name,
                Environment.GetEnvironmentVariable,
                StringComparer.Ordinal);

            foreach (var pair in values)
            {
                Environment.SetEnvironmentVariable(pair.Key, pair.Value);
            }
        }

        public void Dispose()
        {
            foreach (var pair in _originalValues)
            {
                Environment.SetEnvironmentVariable(pair.Key, pair.Value);
            }
        }
    }
}