// -----------------------------------------------------------------------
// <copyright file="Program.cs" company="TedToolkit">
// Copyright (c) TedToolkit. All rights reserved.
// Licensed under the LGPL-3.0 license. See COPYING, COPYING.LESSER file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------------

using System.Runtime.CompilerServices;

using TedToolkit.Ci;
using TedToolkit.GitHub;
using TedToolkit.GitLab;

namespace TedToolkit.CiContext.AotSmoke;

internal static class Program
{
    private const string CUSTOM_VARIABLE = "TEDTOOLKIT_AOT_SMOKE";

    private static void Main()
    {
        Verify(!RuntimeFeature.IsDynamicCodeSupported, "Native AOT execution");
        VerifyProviderEnums();
        VerifyCustomEnums();
        VerifySharedReaders();
        Console.WriteLine("Native AOT smoke checks passed.");
    }

    private static void VerifyProviderEnums()
    {
        Check("CI_PIPELINE_SOURCE", "merge_request_event",
            static () => GitLabCi.PrePipeline.Pipeline.Source == GitLabPipelineSource.MERGE_REQUEST_EVENT);
        Check("CI_ENVIRONMENT_ACTION", "verify",
            static () => GitLabCi.Pipeline.Environment.Action == GitLabEnvironmentAction.VERIFY);
        Check("CI_ENVIRONMENT_TIER", "staging",
            static () => GitLabCi.Pipeline.Environment.Tier == GitLabDeploymentTier.STAGING);
        Check("CI_JOB_STATUS", "canceled",
            static () => GitLabCi.JobOnly.Job.Status == GitLabJobStatus.CANCELED);
        Check("CI_PROJECT_VISIBILITY", "private",
            static () => GitLabCi.PrePipeline.Project.Visibility == GitLabProjectVisibility.PRIVATE);
        Check("CI_MERGE_REQUEST_EVENT_TYPE", "merge_train",
            static () => GitLabCi.PrePipeline.MergeRequest.EventType == GitLabMergeRequestEventType.MERGE_TRAIN);
        Check("CI_SERVER_PROTOCOL", "https",
            static () => GitLabCi.PrePipeline.Server.Protocol == GitLabServerProtocol.HTTPS);
        Check("INCREMENTAL_ROLLOUT_MODE", "timed",
            static () => GitLabCi.PrePipeline.AutoDevOps.IncrementalRolloutMode == GitLabIncrementalRolloutMode.TIMED);
        Check("GITHUB_REF_TYPE", "branch",
            static () => GitHubActions.Ref.Type == GitHubActionsRefType.BRANCH);
        Check("RUNNER_ARCH", "X64",
            static () => GitHubActions.Runner.Architecture == GitHubActionsRunnerArchitecture.X64);
        Check("RUNNER_ENVIRONMENT", "self-hosted",
            static () => GitHubActions.Runner.Environment == GitHubActionsRunnerEnvironment.SELF_HOSTED);
        Check("RUNNER_OS", "Linux",
            static () => GitHubActions.Runner.OperatingSystem == GitHubActionsRunnerOperatingSystem.LINUX);
    }

    private static void VerifyCustomEnums()
    {
        Check(CUSTOM_VARIABLE, "mapped-value",
            static () => CiEnvironment.GetEnumValue<CustomValue>(CUSTOM_VARIABLE) == CustomValue.MAPPED);
        Check(CUSTOM_VARIABLE, "alias",
            static () => CiEnvironment.GetEnumValue<CustomValue>(CUSTOM_VARIABLE) == CustomValue.MAPPED);
        Check(CUSTOM_VARIABLE, null,
            static () => CiEnvironment.GetEnumValue<CustomValue>(CUSTOM_VARIABLE) is null);

        Environment.SetEnvironmentVariable(CUSTOM_VARIABLE, "MAPPED-VALUE");
        VerifyThrows<FormatException>(static () => CiEnvironment.GetEnumValue<CustomValue>(CUSTOM_VARIABLE));

        const string unknownValue = "sensitive-unknown-value";
        Environment.SetEnvironmentVariable(CUSTOM_VARIABLE, unknownValue);
        var exception = VerifyThrows<FormatException>(static () => CiEnvironment.GetEnumValue<CustomValue>(CUSTOM_VARIABLE));
        Verify(!exception.Message.Contains(unknownValue, StringComparison.Ordinal), "Unknown-value redaction");

        Environment.SetEnvironmentVariable(CUSTOM_VARIABLE, "mapped");
        VerifyThrows<InvalidOperationException>(static () => CiEnvironment.GetEnumValue<IncompleteValue>(CUSTOM_VARIABLE));
        Environment.SetEnvironmentVariable(CUSTOM_VARIABLE, "duplicate");
        VerifyThrows<InvalidOperationException>(static () => CiEnvironment.GetEnumValue<DuplicateValue>(CUSTOM_VARIABLE));
    }

    private static void VerifySharedReaders()
    {
        Check(CUSTOM_VARIABLE, "42", static () => CiEnvironment.GetInt32(CUSTOM_VARIABLE) == 42);
        Check(CUSTOM_VARIABLE, "9223372036854775806",
            static () => CiEnvironment.GetInt64(CUSTOM_VARIABLE) == 9223372036854775806L);
        Check(CUSTOM_VARIABLE, "yes", static () => CiEnvironment.GetBoolean(CUSTOM_VARIABLE) is true);
        Check(CUSTOM_VARIABLE, "2026-08-13T10:20:30Z",
            static () => CiEnvironment.GetDateTimeOffset(CUSTOM_VARIABLE) == new DateTimeOffset(2026, 8, 13, 10, 20, 30, TimeSpan.Zero));
        Check(CUSTOM_VARIABLE, "90",
            static () => CiEnvironment.GetTimeSpanFromSeconds(CUSTOM_VARIABLE) == TimeSpan.FromSeconds(90));
        Check(CUSTOM_VARIABLE, "https://example.com",
            static () => CiEnvironment.GetAbsoluteUri(CUSTOM_VARIABLE)?.Host == "example.com");
        Check(CUSTOM_VARIABLE, "[\"linux\",\"docker\"]",
            static () => CiEnvironment.GetJsonStringArray(CUSTOM_VARIABLE) is { Count: 2 } tags
                && tags[0] == "linux" && tags[1] == "docker");
        Check(CUSTOM_VARIABLE, "one, two",
            static () => CiEnvironment.GetCommaSeparatedList(CUSTOM_VARIABLE) is { Count: 2 } values
                && values[0] == "one" && values[1] == "two");
        Check(CUSTOM_VARIABLE, "workspace", static () => CiEnvironment.GetDirectoryPath(CUSTOM_VARIABLE) is not null);
        Check(CUSTOM_VARIABLE, "config.json", static () => CiEnvironment.GetFilePath(CUSTOM_VARIABLE) is not null);

        CiSecret<string> secret = "example-secret-value";
        Verify(secret.ToString() == "[REDACTED]alue", "Secret redaction");
        Verify(secret.Reveal() == "example-secret-value", "Explicit secret access");
    }

    private static void Check(string name, string? value, Func<bool> assertion)
    {
        Environment.SetEnvironmentVariable(name, value);
        Verify(assertion(), name);
    }

    private static void Verify(bool condition, string name)
    {
        if (!condition)
        {
            throw new InvalidOperationException($"Native AOT check failed: {name}.");
        }
    }

    private static TException VerifyThrows<TException>(Action action)
        where TException : Exception
    {
        try
        {
            action();
        }
        catch (TException exception)
        {
            return exception;
        }

        throw new InvalidOperationException($"Expected {typeof(TException).Name} under Native AOT.");
    }

    private enum CustomValue
    {
        [CiEnumValue("mapped-value")]
        [CiEnumValue("alias")]
        MAPPED = 17,
    }

    private enum IncompleteValue
    {
        [CiEnumValue("mapped")]
        MAPPED = 0,

        UNMAPPED = 1,
    }

    private enum DuplicateValue
    {
        [CiEnumValue("duplicate")]
        FIRST = 0,

        [CiEnumValue("duplicate")]
        SECOND = 1,
    }
}