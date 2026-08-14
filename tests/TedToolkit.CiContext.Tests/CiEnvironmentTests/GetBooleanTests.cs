// -----------------------------------------------------------------------
// <copyright file="GetBooleanTests.cs" company="TedToolkit">
// Copyright (c) TedToolkit. All rights reserved.
// Licensed under the LGPL-3.0 license. See COPYING, COPYING.LESSER file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------------

namespace TedToolkit.CiContext.Tests.CiEnvironmentTests;

internal sealed class GetBooleanTests
{
    private const string PROCESS_ENVIRONMENT = "ProcessEnvironment";

    private const string VARIABLE_NAME = "TEDTOOLKIT_CI_CONTEXT_TEST_BOOLEAN";

    /// <summary>
    /// Verifies that common CI boolean representations are accepted without a provider-specific format selector.
    /// </summary>
    [Test]
    [Arguments("true", true)]
    [Arguments("TRUE", true)]
    [Arguments("yes", true)]
    [Arguments("YES", true)]
    [Arguments("1", true)]
    [Arguments("false", false)]
    [Arguments("FALSE", false)]
    [Arguments("no", false)]
    [Arguments("NO", false)]
    [Arguments("0", false)]
    [NotInParallel(PROCESS_ENVIRONMENT)]
    public async Task Should_parse_common_ci_boolean_representations(string value, bool expected)
    {
        using var environment = new EnvironmentVariableScope(VARIABLE_NAME, value);

        await Assert.That(CiEnvironment.GetBoolean(VARIABLE_NAME)).IsEqualTo(expected);
    }

    /// <summary>
    /// Verifies that an absent boolean variable returns null.
    /// </summary>
    [Test]
    [NotInParallel(PROCESS_ENVIRONMENT)]
    public async Task Should_return_null_when_the_variable_is_absent()
    {
        using var environment = new EnvironmentVariableScope(VARIABLE_NAME, null);

        await Assert.That(CiEnvironment.GetBoolean(VARIABLE_NAME)).IsNull();
    }

    /// <summary>
    /// Verifies that unsupported Boolean text is rejected.
    /// </summary>
    [Test]
    [Arguments("2")]
    [Arguments("on")]
    [Arguments(" true ")]
    [NotInParallel(PROCESS_ENVIRONMENT)]
    public async Task Should_reject_unsupported_boolean_text(string invalidValue)
    {
        using var environment = new EnvironmentVariableScope(VARIABLE_NAME, invalidValue);

        await Assert.That(() => CiEnvironment.GetBoolean(VARIABLE_NAME)).Throws<FormatException>();
    }

    /// <summary>
    /// Verifies that malformed Boolean text is not disclosed by the parsing error.
    /// </summary>
    [Test]
    [NotInParallel(PROCESS_ENVIRONMENT)]
    public async Task Should_not_disclose_a_malformed_value()
    {
        const string invalidValue = "sensitive-invalid-boolean";
        using var environment = new EnvironmentVariableScope(VARIABLE_NAME, invalidValue);

        FormatException? exception = null;
        try
        {
            _ = CiEnvironment.GetBoolean(VARIABLE_NAME);
        }
        catch (FormatException caughtException)
        {
            exception = caughtException;
        }

        await Assert.That(exception).IsNotNull();
        await Assert.That(exception!.Message.Contains(invalidValue, StringComparison.Ordinal)).IsFalse();
    }
}