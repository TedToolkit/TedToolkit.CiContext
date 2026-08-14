// -----------------------------------------------------------------------
// <copyright file="GetPresenceMarkerTests.cs" company="TedToolkit">
// Copyright (c) TedToolkit. All rights reserved.
// Licensed under the LGPL-3.0 license. See COPYING, COPYING.LESSER file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------------

namespace TedToolkit.CiContext.Tests.CiEnvironmentTests;

internal sealed class GetPresenceMarkerTests
{
    private const string PROCESS_ENVIRONMENT = "ProcessEnvironment";

    private const string VARIABLE_NAME = "TEDTOOLKIT_CI_CONTEXT_TEST_MARKER";

    /// <summary>
    /// Verifies that only the exact documented marker is accepted.
    /// </summary>
    [Test]
    [NotInParallel(PROCESS_ENVIRONMENT)]
    public async Task Should_return_true_when_the_exact_marker_is_present()
    {
        using var environment = new EnvironmentVariableScope(VARIABLE_NAME, "1");

        await Assert.That(CiEnvironment.GetPresenceMarker(VARIABLE_NAME, "1")).IsTrue();
    }

    /// <summary>
    /// Verifies that an absent presence marker returns null.
    /// </summary>
    [Test]
    [NotInParallel(PROCESS_ENVIRONMENT)]
    public async Task Should_return_null_when_the_marker_is_absent()
    {
        using var environment = new EnvironmentVariableScope(VARIABLE_NAME, null);

        await Assert.That(CiEnvironment.GetPresenceMarker(VARIABLE_NAME, "1")).IsNull();
    }

    /// <summary>
    /// Verifies that a mismatched marker is rejected without revealing its value.
    /// </summary>
    [Test]
    [NotInParallel(PROCESS_ENVIRONMENT)]
    public async Task Should_reject_a_mismatched_marker_without_disclosing_it()
    {
        const string invalidValue = "sensitive-marker";
        using var environment = new EnvironmentVariableScope(VARIABLE_NAME, invalidValue);

        var exception = await Assert.That(() => CiEnvironment.GetPresenceMarker(VARIABLE_NAME, "1"))
            .Throws<FormatException>();

        await Assert.That(exception!.Message.Contains(invalidValue, StringComparison.Ordinal)).IsFalse();
    }

    /// <summary>
    /// Verifies that an empty expected marker is rejected before reading the environment.
    /// </summary>
    [Test]
    [Arguments("")]
    [Arguments(null)]
    [NotInParallel(PROCESS_ENVIRONMENT)]
    public async Task Should_reject_an_empty_expected_marker(string? expectedValue)
    {
        using var environment = new EnvironmentVariableScope(VARIABLE_NAME, "1");

        await Assert.That(() => CiEnvironment.GetPresenceMarker(VARIABLE_NAME, expectedValue!))
            .Throws<ArgumentException>();
    }
}