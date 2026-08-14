// -----------------------------------------------------------------------
// <copyright file="GetInt32Tests.cs" company="TedToolkit">
// Copyright (c) TedToolkit. All rights reserved.
// Licensed under the LGPL-3.0 license. See COPYING, COPYING.LESSER file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------------

namespace TedToolkit.CiContext.Tests.CiEnvironmentTests;

internal sealed class GetInt32Tests
{
    private const string PROCESS_ENVIRONMENT = "ProcessEnvironment";

    private const string VARIABLE_NAME = "TEDTOOLKIT_CI_CONTEXT_TEST_INT32";

    /// <summary>
    /// Verifies invariant integer conversion, missing-value handling, and secret-safe failure.
    /// </summary>
    [Test]
    [NotInParallel(PROCESS_ENVIRONMENT)]
    public async Task Should_read_valid_missing_and_malformed_values()
    {
        using var environment = new EnvironmentVariableScope(VARIABLE_NAME, "42");

        await Assert.That(CiEnvironment.GetInt32(VARIABLE_NAME)).IsEqualTo(42);

        Environment.SetEnvironmentVariable(VARIABLE_NAME, null);
        await Assert.That(CiEnvironment.GetInt32(VARIABLE_NAME)).IsNull();

        const string malformed = "not-an-integer-secret";
        Environment.SetEnvironmentVariable(VARIABLE_NAME, malformed);
        var exception = Assert.Throws<FormatException>(() => CiEnvironment.GetInt32(VARIABLE_NAME));
        await Assert.That(exception.Message.Contains(VARIABLE_NAME, StringComparison.Ordinal)).IsTrue();
        await Assert.That(exception.Message.Contains(malformed, StringComparison.Ordinal)).IsFalse();
    }
}