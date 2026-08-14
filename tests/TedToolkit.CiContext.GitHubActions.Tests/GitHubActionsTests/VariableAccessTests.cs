// -----------------------------------------------------------------------
// <copyright file="VariableAccessTests.cs" company="TedToolkit">
// Copyright (c) TedToolkit. All rights reserved.
// Licensed under the LGPL-3.0 license. See COPYING, COPYING.LESSER file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------------

using System.Reflection;

using TedToolkit.FileSystem;
using TedToolkit.GitHub.Tests;

namespace TedToolkit.CiContext.GitHubActions.Tests.GitHubActionsTests;

internal sealed class VariableAccessTests
{
    private const string PROCESS_ENVIRONMENT = "ProcessEnvironment";

    private static readonly IReadOnlyDictionary<string, string?> _validValues = new Dictionary<string, string?>()
    {
        ["CI"] = "true",
        ["GITHUB_ACTION"] = "build",
        ["GITHUB_ACTION_PATH"] = "/actions/current",
        ["GITHUB_ACTION_REPOSITORY"] = "owner/action",
        ["GITHUB_ACTIONS"] = "true",
        ["GITHUB_ACTOR"] = "octocat",
        ["GITHUB_ACTOR_ID"] = "1234567",
        ["GITHUB_API_URL"] = "https://api.github.example.com",
        ["GITHUB_ARTIFACTS"] = "/commands/artifacts",
        ["GITHUB_ARTIFACTS_LIST"] = "/commands/artifacts-list",
        ["GITHUB_BASE_REF"] = "main",
        ["GITHUB_ENV"] = "/commands/environment",
        ["GITHUB_EVENT_NAME"] = "pull_request",
        ["GITHUB_EVENT_PATH"] = "/workflow/event.json",
        ["GITHUB_GRAPHQL_URL"] = "https://api.github.example.com/graphql",
        ["GITHUB_HEAD_REF"] = "feature/context",
        ["GITHUB_JOB"] = "build",
        ["GITHUB_OUTPUT"] = "/commands/output",
        ["GITHUB_PATH"] = "/commands/path",
        ["GITHUB_REF"] = "refs/pull/42/merge",
        ["GITHUB_REF_NAME"] = "42/merge",
        ["GITHUB_REF_PROTECTED"] = "true",
        ["GITHUB_REF_TYPE"] = "branch",
        ["GITHUB_REPOSITORY"] = "owner/repository",
        ["GITHUB_REPOSITORY_ID"] = "987654321",
        ["GITHUB_REPOSITORY_OWNER"] = "owner",
        ["GITHUB_REPOSITORY_OWNER_ID"] = "7654321",
        ["GITHUB_RETENTION_DAYS"] = "90",
        ["GITHUB_RUN_ATTEMPT"] = "2",
        ["GITHUB_RUN_ID"] = "1658821493",
        ["GITHUB_RUN_NUMBER"] = "314",
        ["GITHUB_SERVER_URL"] = "https://github.example.com",
        ["GITHUB_SHA"] = "ffac537e6cbbf934b08745a378932722df287a53",
        ["GITHUB_STEP_SUMMARY"] = "/commands/summary",
        ["GITHUB_TRIGGERING_ACTOR"] = "rerunner",
        ["GITHUB_WORKFLOW"] = "Build",
        ["GITHUB_WORKFLOW_REF"] = "owner/repository/.github/workflows/build.yml@refs/heads/main",
        ["GITHUB_WORKFLOW_SHA"] = "0123456789abcdef0123456789abcdef01234567",
        ["GITHUB_WORKSPACE"] = "/workspace/repository",
        ["RUNNER_ARCH"] = "X64",
        ["RUNNER_DEBUG"] = "1",
        ["RUNNER_ENVIRONMENT"] = "github-hosted",
        ["RUNNER_NAME"] = "GitHub Actions 2",
        ["RUNNER_OS"] = "Windows",
        ["RUNNER_TEMP"] = "/runner/temp",
        ["RUNNER_TOOL_CACHE"] = "/runner/tool-cache",
    };

    /// <summary>
    /// Verifies that every documented terminal property reads a present current-process value.
    /// </summary>
    [Test]
    [NotInParallel(PROCESS_ENVIRONMENT)]
    public async Task Should_read_every_official_variable()
    {
        using var environment = new EnvironmentVariableScope(_validValues);
        var properties = GetVariableProperties().ToArray();

        await Assert.That(properties).Count().IsEqualTo(OfficialVariableNames.All.Length);

        foreach (var property in properties)
        {
            await Assert.That(property.GetValue(null)).IsNotNull();
        }
    }

    /// <summary>
    /// Verifies representative typed conversions through the provider-specific hierarchy.
    /// </summary>
    [Test]
    [NotInParallel(PROCESS_ENVIRONMENT)]
    public async Task Should_convert_representative_values()
    {
        using var environment = new EnvironmentVariableScope(_validValues);

        await Assert.That(GitHub.GitHubActions.General.Ci).IsTrue();
        await Assert.That(GitHub.GitHubActions.General.Detected).IsTrue();
        await Assert.That(GitHub.GitHubActions.Action.Path).IsEqualTo(new DirectoryPath("/actions/current"));
        await Assert.That(GitHub.GitHubActions.Actor.Id).IsEqualTo(1234567L);
        await Assert.That(GitHub.GitHubActions.Event.PayloadPath).IsEqualTo(new FilePath("/workflow/event.json"));
        await Assert.That(GitHub.GitHubActions.Ref.Protected).IsTrue();
        await Assert.That(GitHub.GitHubActions.Ref.Type).IsEqualTo(GitHubActionsRefType.BRANCH);
        await Assert.That(GitHub.GitHubActions.Repository.Id).IsEqualTo(987654321L);
        await Assert.That(GitHub.GitHubActions.Repository.Workspace).IsEqualTo(new DirectoryPath("/workspace/repository"));
        await Assert.That(GitHub.GitHubActions.Run.Attempt).IsEqualTo(2);
        await Assert.That(GitHub.GitHubActions.Run.Id).IsEqualTo(1658821493L);
        await Assert.That(GitHub.GitHubActions.Run.Number).IsEqualTo(314L);
        await Assert.That(GitHub.GitHubActions.Run.Retention).IsEqualTo(TimeSpan.FromDays(90));
        await Assert.That(GitHub.GitHubActions.Server.ApiUrl).IsEqualTo(new Uri("https://api.github.example.com"));
        await Assert.That(GitHub.GitHubActions.WorkflowCommands.ArtifactsListFile)
            .IsEqualTo(new FilePath("/commands/artifacts-list"));
        await Assert.That(GitHub.GitHubActions.Runner.Architecture).IsEqualTo(GitHubActionsRunnerArchitecture.X64);
        await Assert.That(GitHub.GitHubActions.Runner.DebugEnabled).IsTrue();
        await Assert.That(GitHub.GitHubActions.Runner.Environment).IsEqualTo(GitHubActionsRunnerEnvironment.GIT_HUB_HOSTED);
        await Assert.That(GitHub.GitHubActions.Runner.OperatingSystem).IsEqualTo(GitHubActionsRunnerOperatingSystem.WINDOWS);
    }

    /// <summary>
    /// Verifies that missing values return null and malformed typed values fail without disclosing their text.
    /// </summary>
    [Test]
    [NotInParallel(PROCESS_ENVIRONMENT)]
    public async Task Should_distinguish_absent_and_malformed_values()
    {
        using var environment = new EnvironmentVariableScope(new Dictionary<string, string?>()
        {
            ["GITHUB_RUN_ID"] = null,
            ["GITHUB_REF_TYPE"] = null,
            ["RUNNER_DEBUG"] = null,
        });

        await Assert.That(GitHub.GitHubActions.Run.Id).IsNull();
        await Assert.That(GitHub.GitHubActions.Ref.Type).IsNull();
        await Assert.That(GitHub.GitHubActions.Runner.DebugEnabled).IsNull();

        Environment.SetEnvironmentVariable("GITHUB_RUN_ID", "not-a-run-id");
        Environment.SetEnvironmentVariable("GITHUB_REF_TYPE", "future-ref");
        Environment.SetEnvironmentVariable("RUNNER_DEBUG", "true");

        FormatException? exception = null;
        try
        {
            _ = GitHub.GitHubActions.Run.Id;
        }
        catch (FormatException caughtException)
        {
            exception = caughtException;
        }

        await Assert.That(exception).IsNotNull();
        await Assert.That(exception!.Message.Contains("not-a-run-id", StringComparison.Ordinal)).IsFalse();
        await Assert.That(() => GitHub.GitHubActions.Ref.Type).Throws<FormatException>();
        await Assert.That(() => GitHub.GitHubActions.Runner.DebugEnabled).Throws<FormatException>();
        await Assert.That(CiEnvironment.GetRawValue("GITHUB_RUN_ID")).IsEqualTo("not-a-run-id");
    }

    /// <summary>
    /// Verifies that every terminal access observes the current process environment.
    /// </summary>
    [Test]
    [NotInParallel(PROCESS_ENVIRONMENT)]
    public async Task Should_read_the_environment_on_every_access()
    {
        using var environment = new EnvironmentVariableScope(
            new Dictionary<string, string?>() { ["GITHUB_SHA"] = "before", });

        await Assert.That(GitHub.GitHubActions.Ref.Sha).IsEqualTo("before");

        Environment.SetEnvironmentVariable("GITHUB_SHA", "after");

        await Assert.That(GitHub.GitHubActions.Ref.Sha).IsEqualTo("after");
    }

    private static IEnumerable<PropertyInfo> GetVariableProperties()
    {
        return GetNestedTypes(typeof(GitHub.GitHubActions))
            .SelectMany(static type => type.GetProperties(
                BindingFlags.Static | BindingFlags.Public | BindingFlags.DeclaredOnly))
            .Where(static property => property.IsDefined(typeof(CiVariableAttribute), false));
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
}