// -----------------------------------------------------------------------
// <copyright file="GetRawValueTests.cs" company="TedToolkit">
// Copyright (c) TedToolkit. All rights reserved.
// Licensed under the LGPL-3.0 license. See COPYING, COPYING.LESSER file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------------

namespace TedToolkit.CiContext.Tests.CiEnvironmentTests;

internal sealed class GetRawValueTests
{
    private const string PROCESS_ENVIRONMENT = "ProcessEnvironment";

    private const string VARIABLE_NAME = "TEDTOOLKIT_CI_CONTEXT_TEST_VALUE";

    /// <summary>
    /// Verifies that every raw access observes the current process environment value.
    /// </summary>
    [Test]
    [NotInParallel(PROCESS_ENVIRONMENT)]
    public async Task Should_return_the_current_value_on_every_access()
    {
        using var environment = new EnvironmentVariableScope(VARIABLE_NAME, "before");

        await Assert.That(CiEnvironment.GetRawValue(VARIABLE_NAME)).IsEqualTo("before");

        Environment.SetEnvironmentVariable(VARIABLE_NAME, "after");

        await Assert.That(CiEnvironment.GetRawValue(VARIABLE_NAME)).IsEqualTo("after");
    }

    /// <summary>
    /// Verifies that an absent environment variable returns null.
    /// </summary>
    [Test]
    [NotInParallel(PROCESS_ENVIRONMENT)]
    public async Task Should_return_null_when_the_variable_is_absent()
    {
        using var environment = new EnvironmentVariableScope(VARIABLE_NAME, null);

        await Assert.That(CiEnvironment.GetRawValue(VARIABLE_NAME)).IsNull();
    }

    /// <summary>
    /// Verifies that an empty environment variable name is rejected.
    /// </summary>
    [Test]
    public async Task Should_reject_an_empty_name()
    {
        await Assert.That(() => CiEnvironment.GetRawValue("")).Throws<ArgumentException>();
    }
}